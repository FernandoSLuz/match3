# Domain Layer

## Board & Tiles
- `Board`: grid of `Tile` references, width/height, swap, remove, iterate.
- `Tile`: `TileColor`, `TileType` (Normal, Bomb, Lightning placeholder).

## MatchFinder
- `IMatchFinder` and `SimpleLineMatchFinder` (horizontal/vertical runs >= 3).

## MoveValidator
- `IMoveValidator` and `AdjacentSwapValidator` (adjacent check + produces matches?).

## Resolver
- `ResolverPipeline`: loop of find-matches → remove → gravity → fill → repeat.
- `IGravity` and `VerticalGravity`.

## Spawner
- `ISpawner` and `UniformColorSpawner` using level seed/colors.
