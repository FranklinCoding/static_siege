using System;
using System.Collections.Generic;
using StaticSiege.Cards;
using StaticSiege.Core;
using StaticSiege.Entities;

namespace StaticSiege.Effects;

/// <summary>
/// Interprets EffectDefs against the current run state. Scene-specific visuals should subscribe to events around these mutations.
/// </summary>
public sealed class EffectResolver
{
    private readonly RunState _runState;
    private readonly TurretController _turret;

    public EffectResolver(RunState runState, TurretController turret)
    {
        _runState = runState;
        _turret = turret;
    }

    public void Resolve(CardInstance card, IReadOnlyList<EffectDef> effects, int discardedCount = 0)
    {
        foreach (var effect in effects)
        {
            switch (effect.Kind)
            {
                case EffectKind.ModifyWeaponStat:
                    ApplyWeaponMod(effect);
                    break;
                case EffectKind.AddResource:
                    _runState.Resources.AddEnergy((int)effect.Magnitude);
                    break;
                case EffectKind.AddScrap:
                    _runState.Resources.AddScrap((int)effect.Magnitude);
                    break;
                case EffectKind.AddShield:
                    _runState.Resources.AddShield((int)effect.Magnitude);
                    break;
                case EffectKind.DamageAll:
                    _turret.EmitAreaDamage(effect.Magnitude);
                    break;
                case EffectKind.ApplyStatus:
                    ApplyStatus(effect);
                    break;
                case EffectKind.DrawCards:
                    var drawCount = effect.UseDiscardedCount ? discardedCount : Math.Max(1, effect.Count);
                    _runState.Deck.TryDraw(drawCount, _runState.Rng);
                    break;
                case EffectKind.DiscardHand:
                    _runState.Deck.DiscardHand();
                    break;
                case EffectKind.ModifyHandSize:
                    _runState.Deck.ModifyHandSize((int)effect.Magnitude, effect.Permanent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effect.Kind), effect.Kind, null);
            }
        }
    }

    private void ApplyWeaponMod(EffectDef effect)
    {
        if (effect.Stat is null) return;
        _turret.ApplyWeaponModifier(effect.Stat, effect.Magnitude, effect.Duration);
    }

    private void ApplyStatus(EffectDef effect)
    {
        var stacking = effect.Stacking switch
        {
            "stack" => StatusStacking.Stack,
            "refresh" => StatusStacking.Refresh,
            _ => StatusStacking.Ignore
        };

        _runState.Statuses.Add(new StatusEffect
        {
            Id = effect.Status ?? "unknown",
            Duration = effect.Duration > 0 ? effect.Duration : 8,
            Magnitude = effect.Magnitude,
            Stacking = stacking,
            Stacks = Math.Max(1, effect.Count)
        });
    }
}

