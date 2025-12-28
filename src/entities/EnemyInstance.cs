using Godot;
using StaticSiege.Core;

namespace StaticSiege.Entities;

public sealed class EnemyInstance
{
    public EnemyDef Def { get; }
    public float Health { get; private set; }
    public Vector2 Position { get; private set; }
    public float DistanceToCore => Position.Length();
    public int LaneIndex { get; } = -1;

    public bool IsDead => Health <= 0;

    // Deprecated lane-based constructor (kept for compatibility)
    public EnemyInstance(EnemyDef def, int laneIndex, float startDistance)
    {
        Def = def;
        Health = def.MaxHealth;
        LaneIndex = laneIndex;
        Position = Vector2.Up * startDistance;
    }

    public EnemyInstance(EnemyDef def, Vector2 startPos)
    {
        Def = def;
        Health = def.MaxHealth;
        Position = startPos;
    }

    public void Tick(float delta)
    {
        if (DistanceToCore <= 0) return;
        var dir = Position.Length() > 0 ? Position.Normalized() : Vector2.Zero;
        Position -= dir * Def.Speed * delta;
    }

    public bool ApplyDamage(float amount)
    {
        var damage = System.Math.Max(0, amount - Def.Armor);
        Health -= damage;
        return IsDead;
    }

    public void OnReachCore(Resources resources) => resources.DamageCore((int)Def.Damage);
}

