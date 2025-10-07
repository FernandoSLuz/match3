# Architecture

## Layers

- Domain: Board, Tiles, MatchFinder, MoveValidator, ResolverPipeline, Gravity, Spawner
- Application: TurnManager, CommandBus, EventBus
- Infrastructure: Telemetry, RNG
- Presentation: Views, Input, Camera

## Turn Flow

1. Input → Command(SwapTiles)
2. MoveValidator validates swap
3. MatchFinder finds groups
4. ResolverPipeline removes, applies gravity, refills, repeats cascades
5. EventBus emits updates

## Extensibility

- MatchFinder strategies (lines, groups, links)
- Gravity directions
- Spawner policies & color weights
- RuleEngine and Objectives (future)
