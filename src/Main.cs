using System;
using System.Collections.Generic;
using System.Linq;
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
	[Export] public NodePath? HandViewPath;
	[Export] public NodePath? HudViewPath;

	private RunState _runState = new();
	private EffectResolver? _resolver;
	private RunManager? _runManager;
	private MapView? _mapView;
	private RewardView? _rewardView;
	private Label? _runCompleteLabel;
	private CombatHandView? _handView;
	private HudView? _hudView;
	private ProjectileManager? _projectiles;
	private EncounterManager? _encounterManager;

	private void StartBattleForNode(MapNodeDef node, IReadOnlyList<EncounterDef> encounters)
	{
		if (_encounterManager == null) return;
		var enc = encounters.FirstOrDefault(e => e.Id == node.EncounterId);
		if (enc != null)
		{
			GD.Print($"[Main] Starting battle for node {node.Id} encounter={enc.Id}");
			_encounterManager.StartEncounter(enc);
		}
		else
		{
			GD.PrintErr($"[Main] No encounter found for node {node.Id} encounterId={node.EncounterId}");
		}
		_mapView?.Refresh();
		if (_handView != null)
		{
			_runState.Deck.HandLimit = 6;
			_runState.Deck.StartingHandSize = 6;
			_runState.Deck.OnWaveStart();
			_runState.Deck.TryDraw(_runState.Deck.StartingHandSize, _runState.Rng);
			_handView.Visible = true;
			_handView.Refresh();
		}
	}

	public override void _Ready()
	{
		_encounterManager = GetNode<EncounterManager>(EncounterManagerPath ?? string.Empty);
		var turret = GetNode<TurretController>(TurretPath ?? string.Empty);
		_runManager = GetNode<RunManager>(RunManagerPath ?? string.Empty);
		var debugView = GetNodeOrNull<DebugLaneView>("DebugLaneView");
		_mapView = GetNodeOrNull<MapView>(MapViewPath ?? string.Empty);
		_rewardView = GetNodeOrNull<RewardView>(RewardViewPath ?? string.Empty);
		_runCompleteLabel = GetNodeOrNull<Label>(RunCompleteLabelPath ?? string.Empty);
		_handView = GetNodeOrNull<CombatHandView>(HandViewPath ?? string.Empty);
		_hudView = GetNodeOrNull<HudView>(HudViewPath ?? string.Empty);
		_projectiles = GetNodeOrNull<ProjectileManager>("ProjectileManager");

		// Load data
		var cards = DataLoader.LoadCards("res://data/cards.json");
		var enemies = DataLoader.LoadEnemies("res://data/enemies.json");
		var encounters = DataLoader.LoadEncounters("res://data/encounters.json");
		var (mapSeed, mapNodes) = DataLoader.LoadMap("res://data/map.json");
		if (mapNodes.Count == 0)
		{
			GD.Print("Map load returned 0 nodes; using fallback inline map.");
			mapNodes = new List<StaticSiege.Run.MapNodeDef>
			{
				new() { Id = "n1", Type = MapNodeType.Battle, EncounterId = "encounter_basic_1", Next = new List<string>{ "n2" } },
				new() { Id = "n2", Type = MapNodeType.Rest, Next = new List<string>{ "n3" } },
				new() { Id = "n3", Type = MapNodeType.Battle, EncounterId = "encounter_basic_1", Next = new List<string>{ "n4" } },
				new() { Id = "n4", Type = MapNodeType.Boss, EncounterId = "encounter_boss_1", Next = new List<string>() }
			};
		}

		// Starter deck: take first N cards from data.
		var starterDeck = cards.Count > 0 ? cards : Array.Empty<CardDef>();

		_runState = new RunState();
		_runState.StartRun(starterDeck);

		// Wire systems
		_encounterManager.Init(enemies, turret, _runState.Resources);

		_resolver = new EffectResolver(_runState, turret);
		turret.Bind(_encounterManager, _runState.Statuses);
		turret.Origin = Vector2.Zero;
		if (_projectiles != null)
		{
			var size = GetViewport().GetVisibleRect().Size;
			_projectiles.Position = size / 2f;
			_projectiles.Scale = new Vector2(20f, 20f);
			turret.SetProjectileManager(_projectiles);
		}
		debugView?.Bind(_runState.Resources, _runState);

		// Hand UI
		if (_handView != null)
		{
			_handView.Bind(_runState.Deck, _runState.Resources);
			_handView.Visible = false;
			_handView.OnPlayRequested = idx =>
			{
				if (!_runState.Deck.TryPlay(idx, _runState.Resources, _resolver!, _runState.Rng, out var drew))
				{
					_handView.FlashInsufficient(idx);
				}
				else
				{
					if (!drew)
					{
						_handView.ShowDeckEmptyNotice();
					}
				}
				_handView.Refresh();
			};
		}

		// HUD
		_hudView?.Bind(_runState.Resources);
		if (_hudView != null)
		{
			_hudView.SetEncounterInfo(0, _runManager?.Context.Phase);
		}

		// Run/map setup
		_runManager?.Init(mapNodes, mapSeed);
		if (_runManager != null && string.IsNullOrEmpty(_runManager.Context.CurrentNodeId) && mapNodes.Any())
		{
			_runManager.SetCurrent(mapNodes.First().Id);
		}
		if (_mapView != null && _encounterManager != null) WireMapCallbacks(_mapView, _runManager, _encounterManager, encounters);
		if (_rewardView != null && _encounterManager != null) WireRewardCallbacks(_rewardView, _encounterManager);
		_mapView?.Refresh();

		// Auto-start first node if it's a battle/boss for visibility
		if (_runManager != null && _encounterManager != null)
		{
			var current = _runManager.GetCurrentNode();
			if (current != null && (current.Type == MapNodeType.Battle || current.Type == MapNodeType.Boss))
			{
				_runManager.MarkBattleStart();
				StartBattleForNode(current, encounters);
			}
		}
	}

	public override void _Process(double delta)
	{
		if (_hudView != null && _encounterManager != null)
		{
			var enemyCount = _encounterManager.Enemies.Count;
			var phase = _runManager?.Context.Phase;
			_hudView.SetEncounterInfo(enemyCount, phase);
		}
	}

	private void WireMapCallbacks(MapView mapView, RunManager? runManager, EncounterManager encounterManager, IReadOnlyList<EncounterDef> encounters)
	{
		if (runManager == null) return;

		mapView.OnBattleSelected = node =>
		{
			runManager.SetCurrent(node.Id);
			runManager.MarkBattleStart();
			StartBattleForNode(node, encounters);
		};

		mapView.OnBossSelected = node =>
		{
			runManager.SetCurrent(node.Id);
			runManager.MarkBattleStart();
			StartBattleForNode(node, encounters);
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
			if (_handView != null) _handView.Visible = false;
			_mapView?.Refresh();
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
