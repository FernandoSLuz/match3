# Presentation Layer

## Views
- `SimpleBoardView`: renders tiles as Sprites based on `TileColor`.

## Input
- `SimpleClickInput`: click two tiles to enqueue a swap on the `SimulationRunner`.

## Camera
- `CameraFit2D`: fits orthographic camera to the board using `SimpleBoardView.TileSize` and centers.
