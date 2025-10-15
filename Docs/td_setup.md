## Tower Defense System Setup

### Required GameObjects
- EnemySpawnController (add to an empty `TD/Controllers` object)
  - Assign: `SimulationRunner`, `EnemySpawnerConfig`, `LevelDef`, `PathFitter2D`, `DepthSortingManager`, `SpawnParent`, `EnemyPrefab` (with EnemyView)
- PathFitter2D (add to a `TD/Views` object). Assign camera and padding/lane config
- DepthSortingManager (on the SpawnParent) set BaseSortingOrder and GoalPoint
- TowerDefenseIntegration: assign `SimulationRunner`, `MagicEnergyService`, `EnemySpawnController`

### Prefab Structure
EnemyPrefab
├── EnemyView (MonoBehaviour)
├── SpriteRenderer (Body)
├── HealthBarRoot (RectTransform)
│   ├── HorizontalLayoutGroup
│   └── ContentSizeFitter
└── Collider2D (optional)

### Notes
- HealthBarView needs a `pipPrefab` Image and `container` assigned.
- `LevelDef.GetSprite(color)` is used for pip sprite resolution.
- Spawning uses weighted random from `EnemySpawnerConfig` and lane points from `PathFitter2D`.
- Depth is auto-managed by `DepthSortingManager` using distance to goal.
- Use `TowerDefenseIntegration.TrySpendEnergyAndDamage(color, amount)` to test damage flow.


