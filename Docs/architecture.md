# Architecture

## Layers

- **Domain**: Board, Tiles, MatchFinder, MoveValidator, Gravity, Spawner
- **Application**: TurnManager (coroutine-based), CommandBus, EventBus
- **Infrastructure**: Telemetry, RNG, Configs
- **Presentation**: AnimatedBoardView, Input, Camera

## Sequenced Turn Flow

1. **Input** → `SwapTilesCommand` enqueued
2. **TurnManager** (coroutine):
   - Validates swap with `MoveValidator`
   - Emits `SwapPerformedEvent` → waits for animation
   - **For each cascade**:
     - `MatchFinder` finds groups
     - Board removes tiles → `TilesRemovedEvent` → wait
     - `Gravity` moves tiles → `TilesMovedEvent` → wait
     - `Spawner` fills gaps → `TilesSpawnedEvent` → wait
     - Repeat until no more matches
3. **AnimatedBoardView** plays animations, publishes `AnimationCompleteEvent` after each step
4. **TurnManager** proceeds to next cascade

## Extensibility

- **MatchFinder** strategies (lines, groups, links)
- **Gravity** directions (vertical, radial, diagonal)
- **Spawner** policies & color weights
- **TransitionConfig** for animation timings
- **RuleEngine** and Objectives (future)
