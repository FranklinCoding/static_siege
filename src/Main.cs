using System;
using Godot;
using StaticSiege.Cards;
using StaticSiege.Combat;
using StaticSiege.Core;
using StaticSiege.Effects;
using StaticSiege.Entities;
using StaticSiege.UI;
using StaticSiege.Run;

namespace StaticSiege;

/// <summary>
/// Bootstrap node that wires core systems together. Visuals are minimal; focus is on loop and data loading.
/// </summary>
public partial class Main : Node
{
    [Export] public NodePath? EncounterManagerPath;
    [Export] public NodePath? TurretPath;
    [Export] public NodePath? RunManagerPath;
    [Export] public NodePath? MapViewPath;
    [Export] public NodePath? RewardViewPath;
    [Export] public NodePath? RunCompleteLabelPath;

    private RunState _runState = new();
    private EffectResolver? _resolver;
    private RunManager? _runManager;
    private MapView? _mapView;
    private RewardView? _rewardView;
    private Label? _runCompleteLabel;

    public override void _Ready()
    {
        var encounterManager = GetNode<EncounterManager>(EncounterManagerPath ?? string.Empty);
        var turret = GetNode<TurretController>(TurretPath ?? string.Empty);
        _runManager = GetNode<RunManager>(RunManagerPath ?? string.Empty);
        var debugView = GetNodeOrNull<DebugLaneView>("DebugLaneView");
        _mapView = GetNodeOrNull<MapView>(MapViewPath ?? string.Empty);
        _rewardView = GetNodeOrNull<RewardView>(RewardViewPath ?? string.Empty);
        _runCompleteLabel = GetNodeOrNull<Label>(RunCompleteLabelPath ?? string.Empty);

        // Load data
        var cards = DataLoader.LoadCards("res://data/cards.json");
        var enemies = DataLoader.LoadEnemies("res://data/enemies.json");
        var encounters = DataLoader.LoadEncounters("res://data/encounters.json");
        var (mapSeed, mapNodes) = DataLoader.LoadMap("res://data/map.json");

        // Starter deck: take first N cards from data.
        var starterDeck = cards.Count > 0 ? cards : Array.Empty<CardDef>();

        _runState = new RunState();
        _runState.StartRun(starterDeck);

        // Wire systems
        encounterManager.Init(enemies, turret, _runState.Resources);
        if (encounters.Count > 0)
        {
            // Encounter start is driven by map selection now.
        }

        _resolver = new EffectResolver(_runState, turret);
        turret.Bind(encounterManager, _runState.Statuses);
        debugView?.Bind(_runState.Resources, _runState);

        // Run/map setup
        _runManager?.Init(mapNodes, mapSeed);
        if (_mapView != null) WireMapCallbacks(_mapView, _runManager, encounterManager, encounters);
        if (_rewardView != null) WireRewardCallbacks(_rewardView, encounterManager);
        _mapView?.Refresh();
    }

    public override void _Process(double delta)
    {
        // Future: handle wave completion, shop state, card input.
    }

    private void WireMapCallbacks(MapView mapView, RunManager? runManager, EncounterManager encounterManager, IReadOnlyList<EncounterDef> encounters)
    {
        if (runManager == null) return;

        mapView.OnBattleSelected = node =>
        {
            runManager.SetCurrent(node.Id);
            runManager.MarkBattleStart();
            var enc = encounters.FirstOrDefault(e => e.Id == node.EncounterId);
            if (enc != null) encounterManager.StartEncounter(enc);
        };

        mapView.OnBossSelected = node =>
        {
            runManager.SetCurrent(node.Id);
            runManager.MarkBattleStart();
            var enc = encounters.FirstOrDefault(e => e.Id == node.EncounterId);
            if (enc != null) encounterManager.StartEncounter(enc);
        };

        mapView.OnShopSelected = node =>
        {
            runManager.SetCurrent(node.Id);
            runManager.MarkComplete(); // stub; shops not implemented
            mapView.Refresh();
        };

        mapView.OnRestSelected = node =>
        {
            runManager.SetCurrent(node.Id);
            _runState.Resources.Heal(10);
            mapView.Refresh();
        };
    }

    private void WireRewardCallbacks(RewardView rewardView, EncounterManager encounterManager)
    {
        encounterManager.OnEncounterComplete += () =>
        {
            _runManager?.MarkReward();
            rewardView.Visible = true;
        };

        rewardView.OnRewardChosen = () =>
        {
            // Apply placeholder reward
            _runState.Resources.Heal(10);
            _runState.Resources.AddScrap(5);

            if (_runManager == null)
            {
                rewardView.Visible = false;
                return;
            }

            var next = _runManager.GetAvailableNext().FirstOrDefault();
            if (next == null)
            {
                _runManager.MarkComplete();
                _runCompleteLabel?.Show();
            }
            else
            {
                _runManager.SetCurrent(next.Id);
            }

            rewardView.Visible = false;
            _mapView?.Refresh();
        };
    }
}

