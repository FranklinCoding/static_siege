namespace StaticSiege.Run;

public enum RunPhase { InMap, InBattle, InReward, RunComplete }

public sealed class RunContext
{
    public string Seed { get; set; } = string.Empty;
    public RunPhase Phase { get; set; } = RunPhase.InMap;
    public string CurrentNodeId { get; set; } = string.Empty;
}

