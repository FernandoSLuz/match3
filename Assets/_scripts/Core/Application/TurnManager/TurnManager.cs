using System.Collections;
using MatchTree.Core.Application.CommandBus;
using MatchTree.Core.Application.EventBus;
using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.MatchFinder;
using MatchTree.Core.Domain.MoveValidator;
using MatchTree.Core.Domain.Resolver;

namespace MatchTree.Core.Application.TurnManager
{
    public class SwapTilesCommand : ICommand
    {
        public BoardPosition From;
        public BoardPosition To;
    }

    public class TurnManager
    {
        private readonly CommandBus.CommandBus commandBus;
        private readonly EventBus.EventBus eventBus;
        private readonly Board board;
        private readonly IMoveValidator moveValidator;
        private readonly IMatchFinder matchFinder;
        private readonly IGravity gravity;
        private readonly Domain.Spawner.ISpawner spawner;

        private int turnIndex;
        private bool isProcessing;

        public bool IsProcessing => isProcessing;

        public TurnManager(
            CommandBus.CommandBus commandBus,
            EventBus.EventBus eventBus,
            Board board,
            IMoveValidator moveValidator,
            IMatchFinder matchFinder,
            IGravity gravity,
            Domain.Spawner.ISpawner spawner)
        {
            this.commandBus = commandBus;
            this.eventBus = eventBus;
            this.board = board;
            this.moveValidator = moveValidator;
            this.matchFinder = matchFinder;
            this.gravity = gravity;
            this.spawner = spawner;
            this.turnIndex = 0;
            this.isProcessing = false;
        }

        public bool TryDequeueCommand(out ICommand command)
        {
            if (isProcessing)
            {
                command = null;
                return false;
            }
            return commandBus.TryDequeue(out command);
        }

        public IEnumerator ProcessSwap(SwapTilesCommand swap, System.Func<IEnumerator> waitCallback)
        {
            isProcessing = true;

            // Validate
            var valid = moveValidator.IsValidSwap(board, swap.From, swap.To);
            eventBus.Publish(new MoveAttemptedEvent { TurnIndex = turnIndex, From = swap.From, To = swap.To, Valid = valid });

            if (!valid)
            {
                // Visual swap-out-and-back only
                eventBus.Publish(new SwapVisualEvent { TurnIndex = turnIndex, From = swap.From, To = swap.To, Revert = true });
                yield return waitCallback();
                isProcessing = false;
                yield break;
            }

            // Visual swap and commit board
            eventBus.Publish(new SwapVisualEvent { TurnIndex = turnIndex, From = swap.From, To = swap.To, Revert = false });
            yield return waitCallback();
            board.Swap(swap.From, swap.To);

            // Process cascades one at a time
            var cascadeIndex = 0;
            while (true)
            {
                var matches = matchFinder.FindMatches(board);
                if (matches.Count == 0) break;

                // Remove matched tiles
                var removals = new System.Collections.Generic.List<Domain.BoardState.TileRemovalInfo>();
                foreach (var group in matches)
                {
                    removals.AddRange(board.RemovePositionsWithInfo(group));
                }

                if (removals.Count > 0)
                {
                    var removedPositions = new System.Collections.Generic.List<BoardPosition>();
                    foreach (var r in removals) removedPositions.Add(r.Position);
                    eventBus.Publish(new TilesRemovedEvent { TurnIndex = turnIndex, CascadeIndex = cascadeIndex, Positions = removedPositions });
                    yield return waitCallback();
                }

                // Apply gravity
                var moves = gravity.Apply(board);
                if (moves.Count > 0)
                {
                    var moveList = new System.Collections.Generic.List<(BoardPosition from, BoardPosition to)>();
                    foreach (var m in moves) moveList.Add((m.From, m.To));
                    eventBus.Publish(new TilesMovedEvent { TurnIndex = turnIndex, CascadeIndex = cascadeIndex, Moves = moveList });
                    yield return waitCallback();
                }

                // Spawn new tiles
                var spawns = new System.Collections.Generic.List<BoardPosition>();
                for (var x = 0; x < board.Width; x++)
                {
                    for (var y = 0; y < board.Height; y++)
                    {
                        if (board.GetAt(x, y) == null)
                        {
                            var tile = spawner.CreateTile(x, y);
                            board.SetAt(x, y, tile);
                            spawns.Add(new BoardPosition(x, y));
                        }
                    }
                }

                if (spawns.Count > 0)
                {
                    eventBus.Publish(new TilesSpawnedEvent { TurnIndex = turnIndex, CascadeIndex = cascadeIndex, Positions = spawns });
                    yield return waitCallback();
                }

                eventBus.Publish(new CascadeEndEvent { Index = cascadeIndex, TilesFallen = moves.Count, TimeMs = 0 });
                cascadeIndex++;
            }

            turnIndex++;
            isProcessing = false;
        }
    }
}


