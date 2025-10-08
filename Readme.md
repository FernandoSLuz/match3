# Lighthouse (Unity) – Modular Match-3 Engine

A modular, testable, and extensible Match-3 engine for Unity. This repository contains a cleanly layered implementation with a deterministic simulation core, an event-driven application layer, and a minimal presentation to get you playing quickly.

## Highlights

- Engine-first, modular architecture (Domain, Application, Infrastructure, Presentation)
- Deterministic simulation with seed support
- Strategy-based MatchFinder, MoveValidator, Gravity, Spawner
- EventBus for decoupled UI/VFX/Analytics
- Minimal visual layer: AnimatedBoardView, click-to-swap Input, CameraFit2D

## Quickstart

1) Open the project in Unity 2022+.

2) Create a level configuration:
- Right-click in Project window → Create → Lighthouse → LevelDef
- Adjust `Width`, `Height`, `Seed`, and `Colors` (the 5 presets are fine)

3) Create a scene setup:
- Add an empty GameObject named `Runner` and attach `SimulationRunner`
- Assign your newly created `LevelDef` to `SimulationRunner.Level`
- Add a GameObject `BoardView` with `AnimatedBoardView` and assign `Runner`
- Assign `TransitionConfig` and set 5 Sprites in `AnimatedBoardView` (Red, Blue, Green, Yellow, Purple)
- Add `SimpleClickInput` to any GameObject and assign `Runner`
- Add `CameraFit2D` to the Main Camera and assign `Runner` and `BoardView`

4) (Optional) Event tracing:
- Add `DebugEventTracer` to any GameObject and assign `Runner`
- Observe ordered event logs in Console: level_start, move_attempted, cascade_end, etc.

5) Press Play and click two tiles to swap.

## Project Structure (selected)

```
Assets/_scripts/Core/
  Domain/          // Board, Tile, MatchFinder, MoveValidator, Resolver, Spawner
  Application/     // CommandBus, EventBus, TurnManager
  Infrastructure/  // Telemetry, RNG
  Presentation/    // Views, Input, Camera
  Configs/         // LevelDef, SpawnerConfig
  Tools/           // SimulationRunner, DebugEventTracer
```

## Documentation

Start with the docs index for details, diagrams, and extensibility notes:

- Docs/README: high-level overview and links to all sections
- Docs/architecture: layers, responsibilities, and flow
- Docs/usage: how to wire a scene and play
- Docs/domain, Docs/application, Docs/infrastructure, Docs/presentation

See `Docs/` folder for the full documentation set.

## Roadmap (short)

- Power-ups and cascades enhancements
- Declarative RuleEngine and Objectives
- Telemetry v2 and editor tooling