using UnityEngine;
using MatchTree.Core.Configs;
using MatchTree.Core.Application.CommandBus;
using MatchTree.Core.Application.EventBus;
using MatchTree.Core.Application.TurnManager;
using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.MatchFinder;
using MatchTree.Core.Domain.MoveValidator;
using MatchTree.Core.Domain.Resolver;
using MatchTree.Core.Domain.Spawner;

namespace MatchTree.Core.Tools
{
    public class SimulationRunner : MonoBehaviour
    {
        public LevelDef Level;

        private CommandBus commandBus;
        private EventBus eventBus;
        private Board board;
        private TurnManager.TurnManager turnManager;

        public Board Board => board;

        void Start()
        {
            if (Level == null)
            {
                Debug.LogError("SimulationRunner requires a LevelDef assigned.");
                enabled = false;
                return;
            }

            commandBus = new CommandBus();
            eventBus = new EventBus();

            board = new Board(Level.Width, Level.Height);
            var matchFinder = new SimpleLineMatchFinder();
            var moveValidator = new AdjacentSwapValidator(matchFinder);
            var gravity = new VerticalGravity();
            var spawner = new UniformColorSpawner(Level.Seed, Level.Colors);
            var resolver = new ResolverPipeline(matchFinder, gravity, spawner);

            // Initial fill
            for (var x = 0; x < Level.Width; x++)
            {
                for (var y = 0; y < Level.Height; y++)
                {
                    board.SetAt(x, y, spawner.CreateTile(x, y));
                }
            }

            // Resolve any accidental initial matches
            resolver.Resolve(board);

            turnManager = new TurnManager.TurnManager(commandBus, eventBus, board, moveValidator, resolver);
        }

        void Update()
        {
            turnManager.Tick();
        }

        public void EnqueueSwap(int fromX, int fromY, int toX, int toY)
        {
            commandBus.Enqueue(new SwapTilesCommand { From = new BoardPosition(fromX, fromY), To = new BoardPosition(toX, toY) });
        }
    }
}


