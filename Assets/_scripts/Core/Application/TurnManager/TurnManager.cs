using MatchTree.Core.Application.CommandBus;
using MatchTree.Core.Application.EventBus;
using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.MatchFinder;
using MatchTree.Core.Domain.MoveValidator;
using MatchTree.Core.Domain.Resolver;
using MatchTree.Core.Domain.Spawner;

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
        private readonly ResolverPipeline resolver;

        private int turnIndex;

        public TurnManager(CommandBus.CommandBus commandBus, EventBus.EventBus eventBus, Board board, IMoveValidator moveValidator, ResolverPipeline resolver)
        {
            this.commandBus = commandBus;
            this.eventBus = eventBus;
            this.board = board;
            this.moveValidator = moveValidator;
            this.resolver = resolver;
            this.turnIndex = 0;
        }

        public void Tick()
        {
            if (!commandBus.TryDequeue(out var cmd)) return;

            if (cmd is SwapTilesCommand swap)
            {
                var valid = moveValidator.IsValidSwap(board, swap.From, swap.To);
                eventBus.Publish(new MoveAttemptedEvent { TurnIndex = turnIndex, From = swap.From, To = swap.To, Valid = valid });
                if (!valid) return;

                board.Swap(swap.From, swap.To);
                var result = resolver.Resolve(board);
                turnIndex++;
            }
        }
    }
}


