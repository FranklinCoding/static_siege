using System;
using System.Collections.Generic;
using Godot;
using StaticSiege.Cards;
using StaticSiege.Combat;
using StaticSiege.Effects;

namespace StaticSiege.Core;

/// <summary>
/// Holds run-level state (wave, resources, deck, modifiers). This stays free of scene logic.
/// </summary>
public sealed class RunState
{
    public int Wave { get; private set; } = 1;
    public Resources Resources { get; } = new();
    public DeckState Deck { get; } = new();
    public StatusBucket Statuses { get; } = new();
    public RandomNumberGenerator Rng { get; } = new();

    public void StartRun(IEnumerable<CardDef> starterDeck)
    {
        Rng.Randomize();
        Wave = 1;
        Deck.Init(starterDeck, Rng);
        Statuses.Clear();
    }

    public void NextWave()
    {
        Wave += 1;
        Statuses.OnWaveStart();
        Deck.OnWaveStart();
    }

    public void ResetShields() => Resources.AddShield(0);
}

