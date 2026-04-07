# Dominion

A Monte Carlo simulator and strategy optimizer for the Dominion card game, written in C# (.NET Standard 2.0 / .NET 9).

For full architecture details see [`docs/architecture.md`](docs/architecture.md).

---

## Building & Running

Each executable has a pair of scripts in the repo root:

```bash
./build.sh && ./run.sh                                              # TestAllCardsWithBigMoney
./build-ComparePlayers.sh && ./run-ComparePlayers.sh
./build-TestStrategyOptimizer.sh && ./run-TestStrategyOptimizer.sh
./build-TestCompareStrategyVsAllKnownStrategies.sh && ./run-TestCompareStrategyVsAllKnownStrategies.sh
./build-WebService.sh && ./run-WebService.sh
```

All use `dotnet build` / `dotnet run`. Requires .NET 9 SDK.

WebService runs at `http://localhost:8081/dominion`.

---

## Key Conventions

- **`BuiltInStrategies/Strategies/`** is the single source of truth for all strategies. Do not add strategy files anywhere else — `DynamicStrategyLoader` embeds them automatically at build time.
- **`Dominion/`** (core engine) must stay pure — no I/O, no strategy logic.
- All library projects target `netstandard2.0`; all executables target `net9.0`.
- Use `System.Text.Json` for serialization (not `Newtonsoft.Json` or `JavaScriptSerializer`).

---

## Solutions

| Solution | Contains |
|---|---|
| `Dominion.sln` | Core libraries + test programs |
| `WebService.sln` | WebService + its dependencies |
| `Win8Client.sln` | Legacy Windows 8 UWP client |
