# Static Siege (Godot 4 + C#) Architecture Scaffold

This repo keeps the original HTML prototype as reference and layers a Godot 4 C# project for the Steam-ready build.

## High-level layout
```
project.godot
StaticSiege.csproj
src/               // C# logic
  core/            // run-level state, resources
  cards/           // card defs, deck/hand/discard
  effects/         // effect defs + resolver, statuses
  combat/          // lanes, spawner, waves
  entities/        // turret, enemies, weapons
  ui/              // UI scripts (placeholder)
  meta/            // meta progression stubs
data/              // JSON for cards/enemies/waves
scenes/            // Godot scenes/prefabs
tests/             // unit/integration (headless)
docs/              // design docs, prompts
docs/proto/        // HTML prototype reference
```

## Core loop (run state)
- States: `RunStart -> WavePrep (shop/draft) -> WavePlay -> WaveEnd -> Shop/Next -> RunEnd`.
- `RunState` owns wave number, resources, RNG, deck state, run modifiers.
- `WaveSpawner` consumes `WaveDef` data and emits spawn events into `LaneManager`.
- `LaneManager` ticks enemies; collisions damage the core via `Resources`.
- `TurretController` auto-targets per cadence; cards modify turret/weapons/status.
- `EffectResolver` processes card effects and timed effects; supports stacking rules in `StatusEffect`.

## Data-driven systems
- `data/cards.json`: card defs, costs, type, `effects[]` with `kind`, `magnitude`, `duration`, `count`, `tags`.
- `data/enemies.json`: hp, speed, damage, armor, reward scrap.
- `data/waves.json`: per-wave spawn groups and scaling hooks.

## Testing and sandboxes
- Unit-testable logic lives in plain C# (no scene dependencies) inside `src/core`, `src/cards`, `src/effects`, `src/combat`.
- Add small sandbox scenes (not yet created) for lane sim, hand UI, and spawner timing.
- Guardrails: assertions on resource floors, deck counts, lane bounds; JSON validation on load.

## Migration notes
- Keep `index.html` as design reference (`docs/proto/index.html` points to it).
- Do not port code line-for-line; use the data models and systems above.

