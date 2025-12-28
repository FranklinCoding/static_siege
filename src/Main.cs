using System;
using Godot;
using StaticSiege.Cards;
using StaticSiege.Combat;
using StaticSiege.Core;
using StaticSiege.Effects;
using StaticSiege.Entities;
using StaticSiege.UI;

namespace StaticSiege;

/// <summary>
/// Bootstrap node that wires core systems together. Visuals are minimal; focus is on loop and data loading.
/// </summary>
public partial class Main : Node
{
    [Export] public NodePath? LaneManagerPath;
    [Export] public NodePath? WaveSpawnerPath;
    [Export] public NodePath? TurretPath;

    private RunState _runState = new();
    private EffectResolver? _resolver;

    public override void _Ready()
    {
        var laneManager = GetNode<LaneManager>(LaneManagerPath ?? string.Empty);
        var waveSpawner = GetNode<WaveSpawner>(WaveSpawnerPath ?? string.Empty);
        var turret = GetNode<TurretController>(TurretPath ?? string.Empty);
        var debugView = GetNodeOrNull<DebugLaneView>("DebugLaneView");

        // Load data
        var cards = DataLoader.LoadCards("res://data/cards.json");
        var enemies = DataLoader.LoadEnemies("res://data/enemies.json");
        var waves = DataLoader.LoadWaves("res://data/waves.json");

        // Starter deck: take first N cards from data.
        var starterDeck = cards.Count > 0 ? cards : Array.Empty<CardDef>();

        _runState = new RunState();
        _runState.StartRun(starterDeck);

        // Wire systems
        laneManager.Init(_runState.Resources);
        waveSpawner.LoadEnemies(enemies);
        waveSpawner.StartWave(waves.Count > 0 ? waves[0] : new WaveDef { Wave = 1 });

        _resolver = new EffectResolver(_runState, turret);
        turret.Bind(laneManager, _runState.Statuses);
        debugView?.Bind(_runState.Resources, _runState);
    }

    public override void _Process(double delta)
    {
        // Future: handle wave completion, shop state, card input.
    }
}

