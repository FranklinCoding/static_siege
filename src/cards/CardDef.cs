using System;
using System.Collections.Generic;
using StaticSiege.Effects;

namespace StaticSiege.Cards;

public enum CardType
{
    Attack,
    Defense,
    Utility,
    Permanent
}

public sealed class CardDef
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Cost { get; init; }
    public CardType Type { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool Exiles { get; init; }
    public IReadOnlyList<EffectDef> Effects { get; init; } = Array.Empty<EffectDef>();
}

