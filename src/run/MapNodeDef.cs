using System.Collections.Generic;

namespace StaticSiege.Run;

public enum MapNodeType { Battle, Shop, Rest, Boss }

public sealed class MapNodeDef
{
    public string Id { get; init; } = string.Empty;
    public MapNodeType Type { get; init; } = MapNodeType.Battle;
    public string? EncounterId { get; init; }
    public IReadOnlyList<string> Next { get; init; } = new List<string>();
}

