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
  combat/          // encounters (arena), legacy lanes/spawner kept for reference
  run/             // minimal run/map orchestration (PR #3)
  entities/        // turret, enemies, weapons
  ui/              // UI scripts (placeholder)
  meta/            // meta progression stubs
data/              // JSON for cards/enemies/encounters (waves kept for reference)
scenes/            // Godot scenes/prefabs
tests/             // unit/integration (headless)
docs/              // design docs, prompts
docs/proto/        // HTML prototype reference
```

## Core loop (run state)
- States (legacy): wave loop. Actively migrating to encounter-based real-time arena.
- `RunState` owns wave number (will become node/act), resources, RNG, deck state, run modifiers.
- `EncounterManager` consumes `EncounterDef` data and spawns enemies around the core; legacy `WaveSpawner`/`LaneManager` kept but not wired.
- `RunManager`/`MapView` provide a minimal linear map to chain encounters (PR #3).
- `TurretController` auto-targets nearest enemy; cards modify turret/weapons/status.
- `EffectResolver` processes card effects and timed effects; supports stacking rules in `StatusEffect`.

## Data-driven systems
- `data/cards.json`: card defs, costs, type, `effects[]` with `kind`, `magnitude`, `duration`, `count`, `tags`.
- `data/enemies.json`: hp, speed, damage, armor, reward scrap.
- `data/encounters.json`: per-encounter spawn groups (arena). `data/map.json`: tiny map skeleton. `data/waves.json` kept for reference only.

## Testing and sandboxes
- Unit-testable logic lives in plain C# (no scene dependencies) inside `src/core`, `src/cards`, `src/effects`, `src/combat`.
- Add small sandbox scenes for encounter spawning, hand UI, and turret targeting.
- Guardrails: assertions on resource floors, deck counts, bounds; JSON validation on load.

## Migration notes
- Keep `index.html` as design reference (`docs/proto/index.html` points to it).
- Do not port code line-for-line; use the data models and systems above.

