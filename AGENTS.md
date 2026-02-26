# AGENTS.md

## Cursor Cloud specific instructions

### Project overview

Static Siege is a card roguelike game with two implementations:

1. **HTML Prototype** (`index.html`) — standalone browser game (vanilla JS + HTML5 Canvas). Open in any browser; no build step.
2. **Godot 4.5 + C# Port** (`src/`, `scenes/`, `data/`) — the main production build using Godot.NET.Sdk 4.5.1 targeting .NET 8.

### System dependencies (pre-installed on the VM)

- **.NET 8 SDK** — installed at `/usr/local/share/dotnet`, available as `dotnet` on PATH.
- **Godot 4.5 (.NET/mono)** — installed at `/opt/godot/Godot_v4.5-stable_mono_linux_x86_64/`, symlinked to `/usr/local/bin/godot`.

### Build

- `dotnet build` — compiles the C# project. Output goes to `.godot/mono/temp/bin/Debug/`.
- `godot --headless --build-solutions --quit` — triggers Godot's own build pipeline (generates API assemblies + builds C#). Needed if `.godot/` cache is missing.

### Run

- **Godot project (headless):** `godot --headless` from the repo root. The game initializes and starts the encounter loop. Exit errors about unreferenced static strings are normal Godot shutdown noise.
- **HTML prototype:** serve the repo root via any HTTP server (e.g., `python3 -m http.server 8080`) and open `http://localhost:8080/index.html`.

### Lint / Test

- No dedicated linter is configured. Use `dotnet build` (which surfaces C# compiler warnings) as the primary code quality check.
- No automated tests exist yet. `tests/README.md` describes the planned test strategy (xUnit/NUnit via `dotnet test`).

### Gotchas

- The `.godot/` directory is generated and should not be committed. If it's missing, run `godot --headless --build-solutions --quit` to regenerate before `dotnet build`.
- Godot checks `/usr/share/dotnet` first for the .NET host; the harmless error "The host fxr folder does not exist" appears because we install to `/usr/local/share/dotnet`. This does not affect functionality — Godot falls back correctly.
- The project has no `.sln` file; `dotnet build` operates directly on `StaticSiege.csproj`.
