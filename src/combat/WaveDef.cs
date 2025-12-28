using System;
using System.Collections.Generic;

namespace StaticSiege.Combat;

public sealed class SpawnGroup
{
    public string Enemy { get; init; } = string.Empty;
    public int Count { get; init; }
    public float Cadence { get; init; } = 1.0f;
    public string Lane { get; init; } = "any";
}

public sealed class WaveDef
{
    public int Wave { get; init; }
    public IReadOnlyList<SpawnGroup> Groups { get; init; } = Array.Empty<SpawnGroup>();
    public bool RewardShop { get; init; }
}

