using System.Collections.Generic;

namespace StaticSiege.Combat;

public sealed class EncounterSpawn
{
    public string Enemy { get; init; } = string.Empty;
    public int Count { get; init; }
    public float Cadence { get; init; } = 1.0f;
    public float ArcDegrees { get; init; } = 360f; // full circle by default
    public float Radius { get; init; } = 12f;      // spawn distance from core
}

public sealed class EncounterDef
{
    public string Id { get; init; } = string.Empty;
    public float Difficulty { get; init; } = 1.0f;
    public IReadOnlyList<EncounterSpawn> Spawns { get; init; } = new List<EncounterSpawn>();
}

