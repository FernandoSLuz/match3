# Usage (Scene Setup)

## Create Configs

1. **LevelDef**: Create → MatchTree → LevelDef
   - Configure Width, Height, Seed, Colors

2. **TransitionConfig**: Create → MatchTree → TransitionConfig
   - Set SwapDuration, FallDurationPerCell, FadeOutDuration, ShakeDuration, ShakeMagnitude

## Scene Wiring

1. GameObject `Runner` → add `SimulationRunner` → assign LevelDef
2. GameObject `BoardView` → add `AnimatedBoardView` → assign Runner, 5 Sprites, and TransitionConfig
3. Any GameObject → add `SimpleClickInput` → assign Runner
4. Main Camera → add `CameraFit2D` → assign Runner and BoardView
5. (Optional) Any GameObject → add `DebugEventTracer` → assign Runner

Press Play and click two tiles to swap. Animations will sequence automatically.
