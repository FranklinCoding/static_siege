using System.Collections.Generic;

namespace StaticSiege.Effects;

public enum StatusStacking
{
    Stack,
    Refresh,
    Ignore
}

public sealed class StatusEffect
{
    public string Id { get; init; } = string.Empty;
    public float Duration { get; set; }
    public float Magnitude { get; set; }
    public int Stacks { get; set; } = 1;
    public StatusStacking Stacking { get; init; } = StatusStacking.Stack;
    public bool IsPierceCounter => Id == "pierce";
}

/// <summary>
/// Aggregates statuses on the turret/run. Enemy statuses would live per enemy instance.
/// </summary>
public sealed class StatusBucket
{
    private readonly List<StatusEffect> _effects = new();

    public IReadOnlyList<StatusEffect> Effects => _effects;

    public void Tick(float delta)
    {
        for (var i = _effects.Count - 1; i >= 0; i--)
        {
            _effects[i].Duration -= delta;
            if (_effects[i].Duration <= 0) _effects.RemoveAt(i);
        }
    }

    public void Add(StatusEffect effect)
    {
        var existing = _effects.Find(e => e.Id == effect.Id);
        switch (effect.Stacking)
        {
            case StatusStacking.Stack when existing != null:
                existing.Stacks += effect.Stacks;
                existing.Magnitude += effect.Magnitude;
                existing.Duration = System.Math.Max(existing.Duration, effect.Duration);
                break;
            case StatusStacking.Refresh when existing != null:
                existing.Duration = effect.Duration;
                existing.Magnitude = effect.Magnitude;
                existing.Stacks = effect.Stacks;
                break;
            case StatusStacking.Ignore when existing != null:
                break;
            default:
                _effects.Add(effect);
                break;
        }
    }

    public void Clear() => _effects.Clear();
    public void OnWaveStart() => Clear();
}

