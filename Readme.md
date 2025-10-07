# MatchTree (Unity) – Modular Match-3 Engine

A modular, testable, and extensible Match-3 engine for Unity. This repository contains a cleanly layered implementation with a deterministic simulation core, an event-driven application layer, and a minimal presentation to get you playing quickly.

## Highlights

- Engine-first, modular architecture (Domain, Application, Infrastructure, Presentation)
- Deterministic simulation with seed support
- Strategy-based MatchFinder, MoveValidator, Gravity, Spawner
- EventBus for decoupled UI/VFX/Analytics
- Minimal visual layer: SimpleBoardView, click-to-swap Input, CameraFit2D

## Quickstart

1) Open the project in Unity 2022+.

2) Create a level configuration:
- Right-click in Project window → Create → MatchTree → LevelDef
- Adjust `Width`, `Height`, `Seed`, and `Colors` (the 5 presets are fine)

3) Create a scene setup:
- Add an empty GameObject named `Runner` and attach `SimulationRunner`
- Assign your newly created `LevelDef` to `SimulationRunner.Level`
- Add a GameObject `BoardView` with `SimpleBoardView` and assign `Runner`
- Set 5 Sprites in `SimpleBoardView` (Red, Blue, Green, Yellow, Purple)
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

## License

MIT (or your preferred license). Update this section as needed.

# 🎮 Modular MatchTree Engine Architecture (Unity)

## 1. Princípios

* **Engine independente:** O módulo de tabuleiro não deve depender do jogo principal.
* **Configuração por dados:** Regras e comportamentos definidos por ScriptableObjects ou JSONs.
* **Eventos desacoplados:** Tudo comunica via um Event Bus.
* **Determinismo opcional:** Permite reproduzir partidas via seed.
* **Camadas limpas:** Separar lógicas de domínio, aplicação e apresentação.

---

## 2. Camadas & Responsabilidades

### 🔹 Domain

* **BoardState:** Representa o estado da grade, células, peças e obstáculos.
* **TileModel:** Dados de cada peça (cor, tipo, tags, estado).
* **MatchFinder (Strategy):** Detecta combinações válidas.
* **MoveValidator (Strategy):** Define se um movimento é permitido.
* **Resolver Pipeline:** Aplica o ciclo completo de resolução (input → match → gravity → refill).
* **Spawner (Factory):** Decide o tipo de tile que nasce em cada célula.
* **RuleEngine:** Lê regras declarativas e executa efeitos.
* **ObjectiveSystem:** Avalia condições de vitória/derrota.
* **PowerUpSystem:** Cria e aplica efeitos de power-ups.

### 🔹 Application

* **TurnManager:** Controla o loop de turnos (Idle → Swap → Resolve → End).
* **Command Bus:** Fila de comandos e undo/redo.
* **Event Bus:** Distribui eventos para UI, VFX, áudio e analytics.

### 🔹 Infrastructure

* **Telemetry / Analytics:** Captura e envia eventos de jogo.
* **ConfigLoader:** Carrega LevelDefs, RuleSets, TileSets.
* **Persistence:** Salva estados, seeds e progresso.

### 🔹 Presentation

* **Views:** Interface visual que consome eventos.
* **Animators:** Controlam transições visuais e efeitos.
* **Input Handler:** Converte toques/cliques em comandos.

---

## 3. Estrutura de Pastas (`Assets/`)

```
Assets/
  Scripts/
    Core/
      Domain/
        BoardState/
        TileModel/
        MatchFinder/
        MoveValidator/
        Resolver/
        Spawner/
        RuleEngine/
        Objectives/
        PowerUps/
      Application/
        TurnManager/
        CommandBus/
        EventBus/
      Infrastructure/
        Telemetry/
        ConfigLoader/
        Persistence/
      Presentation/
        Views/
        Animations/
        Input/
  Configs/
    TileSets/
    RuleSets/
    LevelDefs/
    GravityDefs/
    ObjectiveDefs/
  Art/
    Sprites/
    UI/
    Effects/
  Audio/
    SFX/
    Music/
  Tests/
    Unit/
    Integration/
  Tools/
    Editors/
    Debug/
  Docs/
```

---

## 4. Fluxo de um Turno

1. **Input** → `Command(SwapTiles)` → entra na fila.
2. **MoveValidator** verifica se o movimento é válido.
3. **MatchFinder** identifica grupos.
4. **ResolverPipeline** processa remoções, power-ups e cascatas.
5. **Gravity** reposiciona e **Spawner** repovoa.
6. **ObjectiveSystem** verifica progresso.
7. **EventBus** emite atualizações para UI/VFX/Analytics.

---

## 5. Telemetria & Analytics

### Identificadores

* SessionId, LevelId, RunId, TurnIndex, CascadeIndex, ActionId

### Eventos principais

* `level_start(level_id, seed, layout, player_meta)`
* `move_attempted(turn, swap_from, swap_to, valid)`
* `match_resolved(turn, cascade, color_counts, total_tiles_removed)`
* `tile_spawned(color, type)`
* `powerup_created(type, source_group)`
* `objective_progress(id, delta, total)`
* `cascade_end(index, tiles_fallen, time_ms)`
* `level_end(result, turns_used, time_total)`

### Dimensões e métricas

* Tiles removidos por cor e tipo.
* Quantidade média de cascatas.
* Conversões de power-ups.
* Duração média de turno.
* Taxa de sucesso por seed.

---

## 6. Extensibilidade

* **MatchFinder** como Strategy → diferentes tipos de match (linha, grupo, link).
* **Gravity** como Strategy → direções customizáveis (vertical, radial, diagonal).
* **Spawner Policies** → ajusta probabilidades conforme objetivos.
* **RuleEngine** com Conditions/Effets modulares.
* **Board Topologies** → suportar grids recortadas ou hexagonais.

---

## 7. Exemplo de Configurações (conceitual)

```yaml
# LevelDef.yaml
id: level_001
width: 8
height: 8
seed: 934523
objectives:
  - type: collect
    color: blue
    amount: 50
ruleSet: base_ruleset
spawner:
  weights:
    red: 0.2
    blue: 0.2
    green: 0.2
    yellow: 0.2
    purple: 0.2
```

```yaml
# RuleSet.yaml
rules:
  - condition: match_size >= 5 and color == "blue"
    effect: create_powerup
    params:
      type: lightning
  - condition: match_shape == "T"
    effect: create_powerup
    params:
      type: bomb
```

---

## 8. Testabilidade e QA

* **Runner determinístico:** garante reprodução com seed fixa.
* **Simulador headless:** executa partidas automáticas e gera CSVs de métricas.
* **Snapshots:** salva BoardState antes/depois de cada resolução.
* **Contract Tests:** validam regras, spawns e gravidades.

---

## 9. Roadmap de Implementação

1. MVP: BoardState + MatchFinder simples + Resolver básico + Gravity vertical + Spawner padrão.
2. Telemetria v1: `level_start`, `match_resolved`, `level_end`.
3. RuleEngine declarativo.
4. Power-ups e cascatas.
5. Telemetria v2 com mapas térmicos e A/B testing.
6. Ferramentas de editor (validação e replay de seeds).

---

## 10. Diagrama de Dependências

```mermaid
flowchart LR
  Input -->|Commands| CommandBus --> TurnManager
  TurnManager --> ResolverPipeline
  ResolverPipeline --> MatchFinder
  ResolverPipeline --> Gravity
  ResolverPipeline --> Spawner
  ResolverPipeline --> RuleEngine
  ResolverPipeline --> ObjectiveSystem
  BoardState <-->|read/write| ResolverPipeline
  EventBus <-- ResolverPipeline
  UI --- EventBus
  VFX --- EventBus
  Analytics --- EventBus
  Configs --> RuleEngine
  Configs --> Spawner
  RNGSeed --> Spawner
```

---

## 11. Esquema de Dados Coletados

| Dimensão               | Tipo   | Descrição                      |
| ---------------------- | ------ | ------------------------------ |
| session_id             | string | Identificador da sessão        |
| level_id               | string | Nível atual                    |
| seed                   | int    | RNG usado para replicabilidade |
| turn_index             | int    | Número do turno                |
| cascade_index          | int    | Número da cascata atual        |
| tiles_removed_total    | int    | Total de peças removidas       |
| tiles_removed_by_color | dict   | Quantidade por cor             |
| powerups_created       | int    | Número de power-ups criados    |
| duration_ms            | int    | Tempo de resolução             |

---

## 12. Conclusão

Essa arquitetura garante **modularidade, fácil instrumentação, testabilidade e extensibilidade**. O módulo pode evoluir para suportar novos tipos de puzzle, análises complexas e até machine learning para balanceamento dinâmico de níveis.
