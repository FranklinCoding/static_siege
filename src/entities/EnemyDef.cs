namespace StaticSiege.Entities;

public sealed class EnemyDef
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public float MaxHealth { get; init; }
    public float Speed { get; init; }
    public float Damage { get; init; }
    public float Armor { get; init; }
    public int ScrapReward { get; init; } = 1;
}

