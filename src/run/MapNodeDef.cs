using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StaticSiege.Run;

public enum MapNodeType { Battle, Shop, Rest, Boss }

public sealed class MapNodeDef
{
    public string Id { get; init; } = string.Empty;
    public MapNodeType Type { get; init; } = MapNodeType.Battle;

    [JsonPropertyName("encounter")]
    public string? EncounterId { get; init; }

    [JsonPropertyName("next")]
    public List<string> Next { get; init; } = new List<string>();
}

