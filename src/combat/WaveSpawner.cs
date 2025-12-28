using System.Linq;
using System.Collections.Generic;
using Godot;
using StaticSiege.Entities;

namespace StaticSiege.Combat;

/// <summary>
/// Drives timed spawning for a wave using data-driven WaveDef entries.
/// </summary>
public partial class WaveSpawner : Node
{
    [Export] public NodePath? LaneManagerPath;

    private LaneManager? _laneManager;
    private readonly List<ActiveGroup> _activeGroups = new();
    private Dictionary<string, EnemyDef> _enemyLookup = new();

    public override void _Ready()
    {
        _laneManager = GetNode<LaneManager>(LaneManagerPath ?? string.Empty);
    }

    public void LoadEnemies(IEnumerable<EnemyDef> enemies)
    {
        _enemyLookup = enemies.ToDictionary(e => e.Id, e => e);
    }

    public void StartWave(WaveDef wave)
    {
        _activeGroups.Clear();
        foreach (var group in wave.Groups)
        {
            if (!_enemyLookup.TryGetValue(group.Enemy, out var def)) continue;
            _activeGroups.Add(new ActiveGroup(group, def));
        }
    }

    public override void _Process(double delta)
    {
        if (_laneManager == null) return;

        for (var i = _activeGroups.Count - 1; i >= 0; i--)
        {
            var group = _activeGroups[i];
            group.Timer -= (float)delta;
            if (group.Timer <= 0)
            {
                var lane = ResolveLane(group.Group.Lane, _laneManager.Lanes.Count);
                _laneManager.SpawnEnemy(group.Def, lane);
                group.Spawned++;
                group.Timer = group.Group.Cadence;
            }

            if (group.Spawned >= group.Group.Count) _activeGroups.RemoveAt(i);
        }
    }

    private static int ResolveLane(string laneRule, int laneCount)
    {
        if (laneRule == "center") return laneCount / 2;
        if (laneRule == "wide") return 0;
        // default: any/random
        return (int)GD.RandRange(0, System.Math.Max(0, laneCount - 1));
    }

    private sealed class ActiveGroup
    {
        public SpawnGroup Group { get; }
        public EnemyDef Def { get; }
        public int Spawned { get; set; }
        public float Timer { get; set; }

        public ActiveGroup(SpawnGroup group, EnemyDef def)
        {
            Group = group;
            Def = def;
            Timer = group.Cadence;
        }
    }
}

