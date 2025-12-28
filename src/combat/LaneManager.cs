using System.Collections.Generic;
using Godot;
using StaticSiege.Core;
using StaticSiege.Entities;

namespace StaticSiege.Combat;

/// <summary>
/// Godot-facing lane manager. UI/visuals can observe enemies list per lane.
/// </summary>
public partial class LaneManager : Node
{
    [Export] public int LaneCount { get; set; } = 3;
    [Export] public float LaneLength { get; set; } = 20.0f;

    private readonly List<Lane> _lanes = new();
    private Resources? _resources;

    public IReadOnlyList<Lane> Lanes => _lanes;

    public override void _Ready()
    {
        for (var i = 0; i < LaneCount; i++) _lanes.Add(new Lane(i, LaneLength));
    }

    public void Init(Resources resources) => _resources = resources;

    public void SpawnEnemy(EnemyDef def, int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= _lanes.Count) return;
        _lanes[laneIndex].Spawn(def);
    }

    public override void _Process(double delta)
    {
        if (_resources == null) return;
        foreach (var lane in _lanes) lane.Tick((float)delta, _resources);
    }

    public EnemyInstance? FindNearestEnemy()
    {
        EnemyInstance? nearest = null;
        foreach (var lane in _lanes)
        {
            var candidate = lane.GetNearest();
            if (candidate == null) continue;
            if (nearest == null || candidate.DistanceToCore < nearest.DistanceToCore)
            {
                nearest = candidate;
            }
        }
        return nearest;
    }
}

