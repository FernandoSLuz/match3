# Configs

## LevelDef
- Id, Width, Height, Seed, Colors[]

## SpawnerConfig (optional)
- Colors[] or future weights

## TransitionConfig
- SwapDuration (s): time for a direct swap.
- FallDurationPerCell (s): time per grid cell moved during gravity.
- FadeOutDuration (s): time to fade tiles when removed.
- ShakeDuration (s): length of shake before fade.
- ShakeMagnitude (units): amplitude of shake.

Create via: Create → Lighthouse → TransitionConfig. Assign it to `AnimatedBoardView`.