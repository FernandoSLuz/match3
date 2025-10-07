# Usage (Scene Setup)

## Create a Level

- Create → MatchTree → LevelDef
- Configure Width, Height, Seed, Colors

## Scene Wiring

- GameObject `Runner` → add `SimulationRunner` → assign LevelDef
- GameObject `BoardView` → add `SimpleBoardView` → assign Runner and 5 Sprites
- Any GameObject → add `SimpleClickInput` → assign Runner
- Main Camera → add `CameraFit2D` → assign Runner and BoardView
- (Optional) Any GameObject → add `DebugEventTracer` → assign Runner

Press Play and click two tiles to swap.
