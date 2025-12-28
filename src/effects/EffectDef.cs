using System.Collections.Generic;

namespace StaticSiege.Effects;

public enum EffectKind
{
	ModifyWeaponStat,
	AddResource,
	AddScrap,
	AddShield,
	DamageAll,
	ApplyStatus,
	DrawCards,
	DiscardHand,
	ModifyHandSize
}

/// <summary>
/// Data-only definition loaded from JSON. Parameters are flexible and validated at load time.
/// </summary>
public sealed class EffectDef
{
	public EffectKind Kind { get; init; }
	public string? Stat { get; init; }
	public float Magnitude { get; init; }
	public float Duration { get; init; }
	public int Count { get; init; }
	public string? Status { get; init; }
	public string Stacking { get; init; } = "stack";
	public bool UseDiscardedCount { get; init; }
	public bool Permanent { get; init; }
	public Dictionary<string, string> Parameters { get; init; } = new();
}
