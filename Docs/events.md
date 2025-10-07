# Events & Telemetry

## Game Events

- **LevelStartEvent**: `LevelId`, `Seed`, `Width`, `Height`
- **MoveAttemptedEvent**: `TurnIndex`, `From`, `To`, `Valid`
- **SwapPerformedEvent**: `TurnIndex`, `From`, `To` — fired after successful swap
- **TilesRemovedEvent**: `TurnIndex`, `CascadeIndex`, `Positions[]` — matched tiles about to be removed
- **TilesMovedEvent**: `TurnIndex`, `CascadeIndex`, `Moves[(from,to)][]` — gravity moving tiles
- **TilesSpawnedEvent**: `TurnIndex`, `CascadeIndex`, `Positions[]` — new tiles appearing
- **AnimationCompleteEvent**: (no data) — signals animation step finished
- **CascadeEndEvent**: `Index`, `TilesFallen`, `TimeMs` — cascade finished
- **LevelEndEvent**: `Result`, `TurnsUsed`, `TimeTotalMs`

## Event Flow per Turn

1. `MoveAttemptedEvent` (validation)
2. `SwapPerformedEvent` → `AnimationCompleteEvent` (swap animation)
3. For each cascade:
   - `TilesRemovedEvent` → `AnimationCompleteEvent` (shake + fade)
   - `TilesMovedEvent` → `AnimationCompleteEvent` (gravity fall)
   - `TilesSpawnedEvent` → `AnimationCompleteEvent` (spawn)
   - `CascadeEndEvent`

Example subscription:
```csharp
bus.Subscribe<TilesRemovedEvent>(e => {
    Debug.Log($"Removed {e.Positions.Count} tiles in cascade {e.CascadeIndex}");
});
```

## Telemetry Interface

Implemented by `UnityDebugTelemetry`:
- level_start(level_id, seed, width, height)
- move_attempted(turn, from, to, valid)
- match_resolved(turn, cascade, total_tiles_removed)
- level_end(result, turns_used, time_total_ms)
