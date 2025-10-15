# Monster Rendering & Depth Management System

## Overview
A dynamic depth-sorting system for tower defense monsters that move from point A to point B, ensuring proper visual layering when monsters overtake each other due to different speeds.

## Core Requirements
- Monsters move left-to-right from spawn point to goal point
- Visual depth order based on distance to goal (closer = in front)
- Dynamic depth swapping when monsters pass each other
- Performance-optimized for 50-100+ simultaneous monsters
- Integration with existing Match-3 energy system

## Technical Architecture

### 1. Data Structures

#### EnemyDef (ScriptableObject)
```csharp
[CreateAssetMenu(fileName = "EnemyDef", menuName = "TowerDefense/EnemyDef")]
public class EnemyDef : ScriptableObject
{
    public string Id;
    public TileColor Color;
    public Sprite BodySprite;
    public float Speed; // units per second
    public Vector2Int HpRange; // min/max HP
    public float Weight; // for spawn probability
}
```

#### EnemySpawnerConfig (ScriptableObject)
```csharp
[CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "TowerDefense/EnemySpawnerConfig")]
public class EnemySpawnerConfig : ScriptableObject
{
    [System.Serializable]
    public struct SpawnEntry
    {
        public EnemyDef Enemy;
        public Vector2Int HpRange;
        public float Weight;
    }
    
    public SpawnEntry[] SpawnEntries;
    public float StartInterval = 2.0f;
    public float EndInterval = 0.5f;
    public float RampDurationSeconds = 60.0f;
    public AnimationCurve IntervalCurve; // alternative to linear ramp
}
```

### 2. Core Systems

#### EnemySpawnController (Application Layer)
- **Purpose**: Manages spawn timing and enemy instantiation
- **Dependencies**: SimulationRunner, EnemySpawnerConfig, LevelDef, PathFitter2D
- **Key Methods**:
  - `StartSpawning()`: Begin spawn coroutine
  - `CalculateCurrentInterval()`: Get current spawn interval based on time
  - `SelectEnemyEntry()`: Weighted random selection
  - `SpawnEnemy(EnemyDef, int hp)`: Instantiate and initialize enemy

#### PathFitter2D (Presentation Layer)
- **Purpose**: Calculate spawn/exit points with camera-aware fitting
- **Features**: Padding support, multiple lanes, aspect ratio adaptation
- **Key Methods**:
  - `CalculatePathPoints()`: Returns start/end positions for each lane
  - `OnCameraChanged()`: Recalculate on resolution/aspect changes
- **Configuration**:
  - `float PaddingLeft/Right/Top/Bottom`
  - `int LaneCount`
  - `float LaneSpacing`
  - `float SpawnDistance` (off-screen left)
  - `float ExitDistance` (off-screen right)

#### EnemyView (Presentation Layer)
- **Purpose**: Visual representation and movement logic
- **Components**: SpriteRenderer, HealthBar, Collider2D
- **Key Methods**:
  - `Init(EnemyDef def, int hp, LevelDef level)`: Setup visual and data
  - `UpdateMovement(float deltaTime)`: Move towards goal
  - `UpdateDepth()`: Set sorting order based on distance to goal
  - `SetHealth(int current, int max)`: Update health bar
  - `TakeDamage(int amount)`: Handle damage and death

### 3. Depth Management System

#### DepthSortingManager (Presentation Layer)
- **Purpose**: Centralized depth sorting for all active enemies
- **Architecture**: Singleton MonoBehaviour attached to spawn parent
- **Key Features**:
  - Maintains sorted list of enemies by distance to goal
  - Updates sorting orders when enemies change positions
  - Optimized for frequent updates (every frame)

#### Depth Calculation Algorithm
```csharp
public class DepthSortingManager : MonoBehaviour
{
    private List<EnemyView> activeEnemies = new List<EnemyView>();
    private Vector3 goalPoint;
    private int baseSortingOrder = 1000;
    
    public void RegisterEnemy(EnemyView enemy)
    {
        activeEnemies.Add(enemy);
        UpdateAllDepths();
    }
    
    public void UnregisterEnemy(EnemyView enemy)
    {
        activeEnemies.Remove(enemy);
        UpdateAllDepths();
    }
    
    private void UpdateAllDepths()
    {
        // Sort by distance to goal (descending = closer to goal = higher sorting order)
        activeEnemies.Sort((a, b) => 
        {
            float distA = Vector3.Distance(a.transform.position, goalPoint);
            float distB = Vector3.Distance(b.transform.position, goalPoint);
            return distB.CompareTo(distA); // Descending order
        });
        
        // Assign sorting orders
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].SetSortingOrder(baseSortingOrder + i);
        }
    }
}
```

#### EnemyView Depth Integration
```csharp
public class EnemyView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private DepthSortingManager depthManager;
    private Vector3 goalPoint;
    private float speed;
    
    public void Init(EnemyDef def, int hp, Vector3 goal, DepthSortingManager manager)
    {
        // ... initialization code ...
        goalPoint = goal;
        depthManager = manager;
        depthManager.RegisterEnemy(this);
    }
    
    void Update()
    {
        UpdateMovement();
        // Depth is automatically managed by DepthSortingManager
    }
    
    void OnDestroy()
    {
        depthManager?.UnregisterEnemy(this);
    }
}
```

### 4. Health Bar System

#### HealthBarView (Presentation Layer)
- **Purpose**: Visual health representation using color-matched sprites
- **Components**: RectTransform, HorizontalLayoutGroup, ContentSizeFitter
- **Implementation**:
  - Pre-allocate max HP number of Image components
  - Use `LevelDef.GetSprite(enemy.Color)` for health pip sprites
  - Enable/disable images to show current HP
  - Avoid runtime allocation/deallocation

#### Health Bar Layout
```csharp
public class HealthBarView : MonoBehaviour
{
    private List<Image> healthPips = new List<Image>();
    private TileColor enemyColor;
    private LevelDef levelDef;
    
    public void Initialize(int maxHp, TileColor color, LevelDef level)
    {
        enemyColor = color;
        levelDef = level;
        
        // Create health pip images
        for (int i = 0; i < maxHp; i++)
        {
            var pip = CreateHealthPip();
            pip.sprite = levelDef.GetSprite(color);
            healthPips.Add(pip);
        }
    }
    
    public void SetHealth(int currentHp)
    {
        for (int i = 0; i < healthPips.Count; i++)
        {
            healthPips[i].gameObject.SetActive(i < currentHp);
        }
    }
}
```

### 5. Event System Integration

#### Tower Defense Events
```csharp
namespace Lighthouse.TowerDefense.Events
{
    public struct EnemySpawnedEvent : IGameEvent
    {
        public string EnemyId;
        public int InstanceId;
        public TileColor Color;
        public Vector3 Position;
    }
    
    public struct EnemyDamagedEvent : IGameEvent
    {
        public int InstanceId;
        public int DamageAmount;
        public int RemainingHp;
        public TileColor Color;
    }
    
    public struct EnemyDiedEvent : IGameEvent
    {
        public int InstanceId;
        public TileColor Color;
        public Vector3 DeathPosition;
    }
    
    public struct EnemyReachedGoalEvent : IGameEvent
    {
        public int InstanceId;
        public TileColor Color;
    }
}
```

### 6. Performance Optimizations

#### Object Pooling
- **EnemyPool**: Pool of EnemyView prefabs
- **HealthPipPool**: Pool of health pip Image components
- **ParticlePool**: For death/impact effects

#### Update Optimization
- **DepthSortingManager**: Only update depths when positions change significantly
- **Batch Updates**: Group multiple enemy updates per frame
- **LOD System**: Reduce update frequency for distant enemies

#### Memory Management
- **Pre-allocated Arrays**: Fixed-size arrays for enemy lists
- **Component Reuse**: Reuse Image components for health bars
- **Texture Atlasing**: Combine enemy sprites into atlases

### 7. Scene Setup Requirements

#### Required GameObjects
1. **EnemySpawnController** (MonoBehaviour)
   - References: SimulationRunner, EnemySpawnerConfig, LevelDef
   - SpawnParent: Transform for enemy instantiation
   - EnemyPrefab: Prefab with EnemyView component

2. **PathFitter2D** (MonoBehaviour)
   - Camera reference
   - Padding configuration
   - Lane settings

3. **DepthSortingManager** (MonoBehaviour)
   - Attached to spawn parent
   - Goal point reference
   - Base sorting order setting

#### Prefab Structure
```
EnemyPrefab
├── EnemyView (MonoBehaviour)
├── SpriteRenderer (Body)
├── HealthBarRoot (RectTransform)
│   ├── HorizontalLayoutGroup
│   └── ContentSizeFitter
└── Collider2D (for future combat)
```

### 8. Integration Points

#### Match-3 Energy System
- **MagicEnergyService**: Query energy by TileColor
- **Energy Consumption**: Tower abilities consume energy
- **Color Matching**: Damage enemies matching tower's color, when player clicks on the button corresponding charging color tile

#### Future Combat System
- **Tower Placement**: Grid-based tower positioning
- **Projectile System**: Visual projectiles with color matching
- **Damage Calculation**: Based on energy spent and color matching

### 9. Configuration Examples

#### EnemySpawnerConfig Asset
```yaml
SpawnEntries:
  - Enemy: RedEnemyDef
    HpRange: [2, 4]
    Weight: 0.4
  - Enemy: BlueEnemyDef
    HpRange: [3, 6]
    Weight: 0.3
  - Enemy: GreenEnemyDef
    HpRange: [1, 3]
    Weight: 0.3

Timing:
  StartInterval: 2.0
  EndInterval: 0.5
  RampDurationSeconds: 60.0
```

#### PathFitter2D Settings
```yaml
Padding:
  Left: 1.0
  Right: 1.0
  Top: 0.5
  Bottom: 0.5

Lanes:
  Count: 1
  Spacing: 0.0

Distances:
  Spawn: 2.0
  Exit: 2.0
```

### 10. Testing & Validation

#### Unit Tests
- Depth sorting algorithm correctness
- Spawn timing calculations
- Health bar updates
- Event publishing

#### Performance Tests
- 100+ simultaneous enemies
- Memory allocation tracking
- Frame rate consistency
- Memory leak detection

#### Integration Tests
- Match-3 energy consumption
- Event bus communication
- Camera fitting with different aspect ratios
- Multiple lane support

### 11. Future Extensions

#### Advanced Features
- **Multiple Lanes**: Vertical lane separation
- **Enemy Types**: Flying vs ground enemies
- **Formation Movement**: Grouped enemy movement
- **Dynamic Difficulty**: Adaptive spawn rates

#### Visual Enhancements
- **Trail Effects**: Movement trails
- **Death Animations**: Explosion effects
- **Health Bar Animations**: Smooth HP transitions
- **Depth Shadows**: Visual depth cues

#### Performance Scaling
- **LOD System**: Detail reduction for distant enemies
- **Culling**: Off-screen enemy management
- **Batching**: Render batching for similar enemies
- **GPU Instancing**: For large enemy counts

## Implementation Priority

1. **Phase 1**: Core enemy spawning and movement
2. **Phase 2**: Depth sorting system
3. **Phase 3**: Health bar system
4. **Phase 4**: Performance optimizations
5. **Phase 5**: Advanced features and polish

## Dependencies

- **Existing Systems**: SimulationRunner, EventBus, MagicEnergyService
- **Unity Components**: SpriteRenderer, RectTransform, HorizontalLayoutGroup
- **Custom Systems**: PathFitter2D, DepthSortingManager
- **ScriptableObjects**: EnemyDef, EnemySpawnerConfig, LevelDef
