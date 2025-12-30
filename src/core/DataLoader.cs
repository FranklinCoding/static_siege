using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Text.Json.Serialization;
using Godot;
using StaticSiege.Cards;
using StaticSiege.Combat;
using StaticSiege.Entities;
using StaticSiege.Effects;
using StaticSiege.Run;

namespace StaticSiege.Core;

/// <summary>
    /// Loads JSON data for cards, enemies, and waves/encounters. Intended to be called at boot.
/// </summary>
public static class DataLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter() }
    };

    public static IReadOnlyList<CardDef> LoadCards(string path)
    {
        var json = ReadFile(path);
        var defs = JsonSerializer.Deserialize<List<CardDefDto>>(json, Options) ?? new List<CardDefDto>();
        return defs.Select(d => d.ToCardDef()).ToList();
    }

    public static IReadOnlyList<EnemyDef> LoadEnemies(string path)
    {
        var json = ReadFile(path);
        return JsonSerializer.Deserialize<List<EnemyDef>>(json, Options) ?? new List<EnemyDef>();
    }

    public static IReadOnlyList<WaveDef> LoadWaves(string path)
    {
        var json = ReadFile(path);
        return JsonSerializer.Deserialize<List<WaveDef>>(json, Options) ?? new List<WaveDef>();
    }

        public static IReadOnlyList<EncounterDef> LoadEncounters(string path)
        {
            var json = ReadFile(path);
            return JsonSerializer.Deserialize<List<EncounterDef>>(json, Options) ?? new List<EncounterDef>();
        }

        public static (string Seed, IReadOnlyList<MapNodeDef> Nodes) LoadMap(string path)
        {
            var json = ReadFile(path);
            var dto = JsonSerializer.Deserialize<MapDto>(json, Options) ?? new MapDto();
            return (dto.Seed ?? string.Empty, dto.Nodes ?? new List<MapNodeDef>());
        }

    private static string ReadFile(string path)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        return file.GetAsText();
    }

    private sealed class CardDefDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Cost { get; set; }
        public string Type { get; set; } = "Utility";
        public string Description { get; set; } = string.Empty;
        public bool Exiles { get; set; }
        public List<EffectDef> Effects { get; set; } = new();

        public CardDef ToCardDef()
        {
            var parsedType = Enum.TryParse<CardType>(Type, true, out var t) ? t : CardType.Utility;
            return new CardDef
            {
                Id = Id,
                Name = Name,
                Cost = Cost,
                Type = parsedType,
                Description = Description,
                Exiles = Exiles,
                Effects = Effects
            };
        }
    }

    private sealed class MapDto
    {
        public string? Seed { get; set; }
        public List<MapNodeDef>? Nodes { get; set; }
    }
}

