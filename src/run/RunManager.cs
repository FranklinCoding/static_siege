using System.Collections.Generic;
using System.Linq;
using Godot;

namespace StaticSiege.Run;

/// <summary>
/// Minimal run/map orchestrator for PR #3. Hardcoded map, no persistence or meta.
/// </summary>
public partial class RunManager : Node
{
    public RunContext Context { get; } = new();
    private Dictionary<string, MapNodeDef> _nodes = new();

    public void Init(IEnumerable<MapNodeDef> nodes, string seed = "")
    {
        _nodes = nodes.ToDictionary(n => n.Id, n => n);
        Context.Seed = string.IsNullOrEmpty(seed) ? System.Guid.NewGuid().ToString("N") : seed;
        Context.CurrentNodeId = nodes.FirstOrDefault()?.Id ?? string.Empty;
        Context.Phase = RunPhase.InMap;
    }

    public IEnumerable<MapNodeDef> GetAvailableNext()
    {
        if (!_nodes.TryGetValue(Context.CurrentNodeId, out var node)) return Enumerable.Empty<MapNodeDef>();
        return node.Next.Select(id => _nodes.GetValueOrDefault(id)).Where(n => n != null)!;
    }

    public MapNodeDef? GetCurrentNode() => _nodes.GetValueOrDefault(Context.CurrentNodeId);

    public void SetCurrent(string nodeId)
    {
        if (_nodes.ContainsKey(nodeId))
        {
            Context.CurrentNodeId = nodeId;
            Context.Phase = RunPhase.InMap;
        }
    }

    public void MarkBattleStart() => Context.Phase = RunPhase.InBattle;
    public void MarkReward() => Context.Phase = RunPhase.InReward;
    public void MarkComplete() => Context.Phase = RunPhase.RunComplete;
}

