using Godot;
using System.Collections.Generic;

namespace StaticSiege.Combat;

public partial class ProjectileManager : Node2D
{
    private readonly List<Projectile> _projectiles = new();
    [Export] public float Speed = 30f;
    [Export] public float Lifetime = 1.0f;
    [Export] public float Radius = 0.3f;
    [Export] public float LineWidth = 0.15f;
    [Export] public Color TrailColor = new(0.3f, 0.9f, 1f, 0.8f);

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        for (int i = _projectiles.Count - 1; i >= 0; i--)
        {
            var p = _projectiles[i];
            p.Life -= (float)delta;
            p.PrevPosition = p.Position;
            p.Position += p.Velocity * (float)delta;
            if (p.Life <= 0 || p.Position.DistanceTo(p.Target) <= Radius)
            {
                _projectiles.RemoveAt(i);
                continue;
            }
            _projectiles[i] = p;
        }
        QueueRedraw();
    }

    public override void _Draw()
    {
        // Note: positions are in encounter space (core at 0,0); Debug view applies scaling/translation.
        foreach (var p in _projectiles)
        {
            DrawLine(p.PrevPosition, p.Position, TrailColor, LineWidth, true);
            DrawCircle(p.Position, Radius, TrailColor);
        }
    }

    public void SpawnProjectile(Vector2 origin, Vector2 target)
    {
        var dir = (target - origin).Normalized();
        var proj = new Projectile
        {
            Position = origin,
            Target = target,
            Velocity = dir * Speed,
            PrevPosition = origin,
            Life = Lifetime
        };
        _projectiles.Add(proj);
    }

    private struct Projectile
    {
        public Vector2 Position;
        public Vector2 PrevPosition;
        public Vector2 Target;
        public Vector2 Velocity;
        public float Life;
    }
}

