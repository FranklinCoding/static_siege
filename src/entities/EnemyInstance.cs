using StaticSiege.Core;

namespace StaticSiege.Entities;

public sealed class EnemyInstance
{
    public EnemyDef Def { get; }
    public float Health { get; private set; }
    public float DistanceToCore { get; private set; } = 1.0f;
    public int LaneIndex { get; }

    public bool IsDead => Health <= 0;

    public EnemyInstance(EnemyDef def, int laneIndex, float startDistance)
    {
        Def = def;
        Health = def.MaxHealth;
        LaneIndex = laneIndex;
        DistanceToCore = startDistance;
    }

    public void Tick(float delta)
    {
        DistanceToCore = System.Math.Max(0, DistanceToCore - Def.Speed * delta);
    }

    public bool ApplyDamage(float amount)
    {
        var damage = System.Math.Max(0, amount - Def.Armor);
        Health -= damage;
        return IsDead;
    }

    public void OnReachCore(Resources resources) => resources.DamageCore((int)Def.Damage);
}

