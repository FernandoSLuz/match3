# Events & Telemetry

## Game Events

- LevelStartEvent: `LevelId`, `Seed`, `Width`, `Height`
- MoveAttemptedEvent: `TurnIndex`, `From`, `To`, `Valid`
- CascadeEndEvent: `Index`, `TilesFallen`, `TimeMs`
- LevelEndEvent: `Result`, `TurnsUsed`, `TimeTotalMs`

Example subscription:
```csharp
bus.Subscribe<MoveAttemptedEvent>(e => {
    Debug.Log($"move_attempted turn={e.TurnIndex} valid={e.Valid}");
});
```

## Telemetry Interface

Implemented by `UnityDebugTelemetry`:
- level_start(level_id, seed, width, height)
- move_attempted(turn, from, to, valid)
- match_resolved(turn, cascade, total_tiles_removed)
- level_end(result, turns_used, time_total_ms)
