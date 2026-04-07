# Dominion Repository Architecture

A **Monte Carlo simulator and strategy optimizer** for the Dominion card game, written in **C# (.NET Standard 2.0 / .NET 9)**.

---

## Top-Level Directory Structure

| Directory | Purpose |
|---|---|
| `Dominion/` | Core game engine — rules, state, cards |
| `Dominion.Strategy/` | Strategy abstraction layer and DSL |
| `BuiltInStrategies/` | ~92 hand-coded strategy implementations |
| `DynamicStrategyLoader/` | Runtime compilation and loading of strategies |
| `DataGathering/` | Statistics collection during simulation |
| `HtmlRenderer/` | HTML/PDF report generation |
| `TestOutput/` | Result formatting and ranking output |
| `Resources/` | Shared resource loader utility |
| `Dominion.StrategyOptimizer/` | Genetic algorithm for strategy optimization |
| `TestPrograms/` | CLI programs for running simulations |
| `WebService/` | REST API and web UI for strategy comparison |
| `Win8Client/` | Legacy Windows 8 UWP desktop client |
| `docs/` | Documentation |

---

## Languages & Frameworks

- **Primary language:** C# (.NET)
- **Library targets:** `netstandard2.0` (cross-platform)
- **Executable targets:** `net9.0`
- **Build system:** MSBuild / `dotnet` SDK-style `.csproj` files
- **Three solution files:** `Dominion.sln`, `WebService.sln`, `Win8Client.sln`

---

## Layered Architecture

```
┌─────────────────────────────────────────────────────┐
│                  Presentation Layer                  │
│         WebService (REST API + web UI)               │
│         Win8Client (UWP desktop, legacy)             │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│                 Application Layer                    │
│   TestPrograms, DataGathering, HtmlRenderer          │
│   Dominion.StrategyOptimizer                         │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│                  Strategy Layer                      │
│   Dominion.Strategy (interfaces + DSL)               │
│   BuiltInStrategies (~92 implementations)            │
│   DynamicStrategyLoader (runtime compilation)        │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│                   Domain Layer                       │
│         Dominion (game engine — pure, no I/O)        │
└─────────────────────────────────────────────────────┘
```

---

## Core Game Engine (`Dominion/`)

The game engine is **pure** — no I/O, no strategy logic. All other projects depend on it.

| File / Folder | Role |
|---|---|
| `Game.cs` | Main game orchestration loop |
| `GameState.cs` | Immutable snapshot of the full game state |
| `PlayerState.cs` | Per-player state (hand, deck, counters) |
| `Card.cs` / `Cards.cs` | Card definitions and the master card registry |
| `GameConfig.cs` | Game setup parameters |
| `Cards/` | Card implementations across 18 expansions (BaseSet, Intrigue, Seaside, Alchemy, Prosperity, Cornucopia, Hinterlands, DarkAges, Guilds, Adventures, Empires, Nocturne, Renaissance, SecondEdition, Promo, …) |
| `Collections/` | `PileOfCards`, `SetOfCards`, `BagOfCards`, `MapOfCards` |
| `GameLogs/` | `DebugGameLog`, `HumanReadableGameLog`, `EmptyGameLog`, `GameLogMultiplexer` |
| `SimpleDefinitions/` | Enums, delegates, constants |

---

## Strategy Framework (`Dominion.Strategy/`)

Provides the interfaces and a declarative DSL for describing AI player strategies.

| File / Folder | Role |
|---|---|
| `Strategy.cs` | Core strategy interface |
| `PlayerAction.cs` | Decision context passed to strategies |
| `DefaultPlayerAction.cs` | Sensible default behaviors for all decisions |
| `Description/` | DSL: `StrategyDescription`, `PickByPriorityDescription`, `CardAcceptanceDescription` |
| `DefaultPlayRules/` | Per-card default behaviors (`CardOverrides.cs`, `Cards/`) |
| `CardPicker/` | Card selection logic |
| `CardsWithoutDefaultBehaviors.cs` | Registry of cards that require explicit strategy handling |

---

## Built-In Strategies (`BuiltInStrategies/`)

~92 concrete strategy implementations (e.g. `BigMoney.cs`, `BigMoneySingleSmithy.cs`, `Rebuild.cs`). Each implements a specific Dominion strategy variant using the `Dominion.Strategy` DSL.

`StrategyLoader.cs` — reflects over an assembly to discover all `PlayerAction` implementations.

**This is the single source of truth for strategies.** `DynamicStrategyLoader` embeds these files directly.

---

## Dynamic Strategy Loader (`DynamicStrategyLoader/`)

Enables the WebService to load and hot-swap strategies at runtime.

**How it works:**
1. All `.cs` files from `BuiltInStrategies/Strategies/` are embedded into `Dominion.DynamicStrategyLoader.dll` as binary resources at build time (via `<EmbeddedResource>` with a `<Link>` alias)
2. On startup, the loader extracts them to `{system temp}/DominulatorStrategies/`
3. It uses `CSharpCodeProvider` to dynamically compile each `.cs` file
4. Compiled assemblies are reflected on to get `PlayerAction` instances

**Lifecycle:**
- If `{temp}/DominulatorStrategies/` already exists on launch → error with fix instructions (likely crash remnant)
- On clean exit (`ProcessExit` / `CancelKeyPress`) → temp directory is deleted

---

## Data Gathering (`DataGathering/`)

Cross-cutting statistics collected during game simulations.

| File | Role |
|---|---|
| `StrategyComparison.cs` | Orchestrates multi-game comparison runs |
| `StrategyComparisonResults.cs` | Win/loss/tie statistics |
| `StatsPerTurnGameLog.cs` | Per-turn counters logged during a game |
| `PerTurnPlayerCounters.cs` | Tracks coins, cards, VP per turn |
| `HistogramData.cs` | Statistical distributions |

---

## Web Service (`WebService/`)

An HTTP server exposing the simulator as a REST API with a web UI.

**Entry point:** `Program.cs` → `WebService.cs`

**Startup sequence:**
1. Check all card images are present (warnings only)
2. Load strategies via `DynamicStrategyLoader`
3. Start `HttpListener` on `http://localhost:8081/dominion/`

**API responses** (`Responses/`):

| Handler | Purpose |
|---|---|
| `GetAvailableStrategies` | List all loaded strategies |
| `GetAvailableCards` | List all kingdom cards |
| `StrategyComparisonRequest` | Run a simulation and return results |
| `ComparisonDescription` | Describe a comparison setup |
| `GetGameLog` | Return a human-readable game log |
| `GetStrategyText` | Return strategy source code |
| `CheckStrategyCode` | Compile-check user-submitted strategy code |
| `GetKingdomCardImages` | Serve card images |
| `GetAvailableGraphs` | List available graph types |
| `Graphs/` | Generate chart data (VP per turn, coin spend, point spread, etc.) |

**Serialization:** `System.Text.Json` (`JsonSerializer`).

**Static assets** (`Resources/`) are embedded into the assembly at build time and served from memory. Includes `Dominulator.html` (the web UI), JS/CSS, card images.

---

## Test Programs (`TestPrograms/`)

All target `net9.0`. Each has a `build-<name>.sh` / `run-<name>.sh` script in the repo root.

| Program | Purpose |
|---|---|
| `TestAllCardsWithBigMoney` | Regression test — runs every implemented card against a BigMoney baseline |
| `ComparePlayers` | Compare win rates between specific strategy pairs |
| `TestStrategyOptimizer` | Validate the genetic algorithm optimizer |
| `TestCompareStrategyVsAllKnownStrategies` | Run a strategy against all 92 built-in strategies |

---

## Strategy Optimizer (`Dominion.StrategyOptimizer/`)

Uses a **genetic algorithm** to search for strong strategies:
- `GeneticAlgorithm.cs` — mutation, crossover, selection
- `StrategyOptimizer.cs` — orchestration
- `BigMoneyWithCard/` — GA operators specialized for BigMoney-style strategies
- `PickByPriorityDescription/` — DSL-based strategy representation for the optimizer

---

## Build & Run Scripts

Each executable has a pair of scripts in the repo root:

```
build.sh / run.sh                                        # TestAllCardsWithBigMoney
build-ComparePlayers.sh / run-ComparePlayers.sh
build-TestStrategyOptimizer.sh / run-TestStrategyOptimizer.sh
build-TestCompareStrategyVsAllKnownStrategies.sh / run-TestCompareStrategyVsAllKnownStrategies.sh
build-WebService.sh / run-WebService.sh
```

All use `dotnet build` / `dotnet run`.

---

## Key Design Decisions

| Decision | Rationale |
|---|---|
| Pure `Dominion/` core | Game logic is completely isolated from I/O and strategy — easy to test, reusable across all interfaces |
| `netstandard2.0` for libraries | All library projects target netstandard2.0 for maximum compatibility |
| Strategy DSL in `Dominion.Strategy` | Strategies are described declaratively via `PickByPriorityDescription` rather than raw imperative code |
| `BuiltInStrategies` as single source of truth | `DynamicStrategyLoader` embeds from `BuiltInStrategies/Strategies/` at build time — no separate copy to maintain |
| Runtime strategy compilation | WebService supports user-submitted custom strategy code compiled on the fly |
| Genetic algorithm optimizer | Discovers strong strategies by evolving strategy parameters over thousands of simulated games |
