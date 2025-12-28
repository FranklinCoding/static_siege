using System.Collections.Generic;
using StaticSiege.Core;
using StaticSiege.Entities;

namespace StaticSiege.Combat;

public sealed class Lane
{
    public int Index { get; }
    public float Length { get; }
    private readonly List<EnemyInstance> _enemies = new();

    public IReadOnlyList<EnemyInstance> Enemies => _enemies;

    public Lane(int index, float length)
    {
        Index = index;
        Length = length;
    }

    public void Spawn(EnemyDef def)
    {
        _enemies.Add(new EnemyInstance(def, Index, Length));
    }

    public void Tick(float delta, Resources resources)
    {
        for (var i = _enemies.Count - 1; i >= 0; i--)
        {
            var enemy = _enemies[i];
            enemy.Tick(delta);
            if (enemy.DistanceToCore <= 0)
            {
                enemy.OnReachCore(resources);
                _enemies.RemoveAt(i);
                continue;
            }

            if (enemy.IsDead) _enemies.RemoveAt(i);
        }
    }

    public EnemyInstance? GetNearest() => _enemies.Count == 0 ? null : _enemies[^1];
}

