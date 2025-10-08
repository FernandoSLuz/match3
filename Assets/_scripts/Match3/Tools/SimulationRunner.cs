using System.Collections;
using UnityEngine;
using Lighthouse.Match3.Configs;
using Lighthouse.Match3.Application.Events;
using Lighthouse.Shared.CommandBus;
using Lighthouse.Shared.EventBus;
using Lighthouse.Match3.Application.TurnManager;
using Lighthouse.Match3.Domain.BoardState;
using Lighthouse.Match3.Domain.MatchFinder;
using Lighthouse.Match3.Domain.MoveValidator;
using Lighthouse.Match3.Domain.Resolver;
using Lighthouse.Match3.Domain.Spawner;

namespace Lighthouse.Match3.Tools
{
    public class SimulationRunner : MonoBehaviour
    {
        public LevelDef Level;

        private CommandBus commandBus;
        private EventBus eventBus;
        private Board board;
        private TurnManager turnManager;
        private IMatchFinder matchFinder;
        private IGravity gravity;
        private ISpawner spawner;
        private bool animationComplete;

        public Board Board => board;
        public EventBus Bus => eventBus;

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
            matchFinder = new SimpleLineMatchFinder();
            var moveValidator = new AdjacentSwapValidator(matchFinder);
            gravity = new VerticalGravity();
            spawner = new UniformColorSpawner(Level.Seed, Level.Colors);

            // Initial fill
            for (var x = 0; x < Level.Width; x++)
            {
                for (var y = 0; y < Level.Height; y++)
                {
                    board.SetAt(x, y, spawner.CreateTile(x, y));
                }
            }

            // Resolve any accidental initial matches silently
            var resolver = new ResolverPipeline(matchFinder, gravity, spawner);
            resolver.Resolve(board);

            turnManager = new TurnManager(commandBus, eventBus, board, moveValidator, matchFinder, gravity, spawner);

            // Subscribe to animation completion
            eventBus.Subscribe<AnimationCompleteEvent>(e => animationComplete = true);

            // Emit level start
            eventBus.Publish(new LevelStartEvent { LevelId = Level.Id, Seed = Level.Seed, Width = Level.Width, Height = Level.Height });
        }

        void Update()
        {
            if (turnManager.IsProcessing) return;

            if (turnManager.TryDequeueCommand(out var cmd))
            {
                if (cmd is SwapTilesCommand swap)
                {
                    StartCoroutine(turnManager.ProcessSwap(swap, WaitForAnimation));
                }
            }
        }

        private IEnumerator WaitForAnimation()
        {
            animationComplete = false;
            while (!animationComplete)
            {
                yield return null;
            }
        }

        public void EnqueueSwap(int fromX, int fromY, int toX, int toY)
        {
            commandBus.Enqueue(new SwapTilesCommand { From = new BoardPosition(fromX, fromY), To = new BoardPosition(toX, toY) });
        }
    }
}


