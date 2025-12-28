# Tests

- Logic in `src/core`, `src/cards`, `src/effects`, and `src/combat` is designed to be unit-testable without scene dependencies.
- When ready, create a `StaticSiege.Tests` project referencing `StaticSiege.csproj` and add xUnit/NUnit tests for:
  - Deck draw/reshuffle/hand limit invariants.
  - Effect resolution (cost checks, status stacking, temporary vs permanent modifiers).
  - Wave spawning cadence and lane assignment.
  - Resource floors (no negative fuel/scrap/health).
- Godot can run headless tests via `godot --headless --run-tests` if you author GDScript-based tests; C# tests should run through `dotnet test`.

