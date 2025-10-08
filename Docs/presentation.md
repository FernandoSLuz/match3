# Presentation Layer

## Views
- `AnimatedBoardView`: Subscribes to game events and plays sequenced transitions (swap, shake, fade, fall, spawn) using `TransitionConfig`.

## Input
- `SimpleClickInput`: Click two tiles to enqueue a swap command.

## Camera
- `CameraFit2D`: Fits orthographic camera to the board using `AnimatedBoardView.TileSize`.

## Animation Flow

The system now sequences animations **step-by-step** with the domain layer:

1. **Swap Animation** → Visual swap, then domain validates and swaps
2. **Match Check** → Domain finds matches
3. **Shake + Fade** → Tiles shake, then fade out on removal
4. **Gravity Animation** → Tiles fall to fill gaps
5. **Spawn Animation** → New tiles appear from above
6. **Repeat 2-5** until no more matches

Each animation step publishes `AnimationCompleteEvent` when finished, allowing the `TurnManager` to proceed to the next cascade.

All timings and magnitudes are configured in `TransitionConfig`.
