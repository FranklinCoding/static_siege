using Godot;
using StaticSiege.Combat;
using StaticSiege.Effects;

namespace StaticSiege.Entities;

/// <summary>
/// Auto-firing turret. Visual/projectile handling should be added in scene scripts.
/// </summary>
public partial class TurretController : Node
{
    [Export] public float Damage { get; set; } = 1f;
    [Export] public float FireRateSeconds { get; set; } = 1f;
    [Export] public Vector2 Origin { get; set; } = Vector2.Zero;
    [Export] public NodePath? ProjectileManagerPath;

    private float _fireTimer;
    private EncounterManager? _encounter;
    private StatusBucket? _statuses;
    private ProjectileManager? _projectiles;

    public void Bind(EncounterManager encounter, StatusBucket statuses)
    {
        _encounter = encounter;
        _statuses = statuses;
    }

    public void SetProjectileManager(ProjectileManager pm)
    {
        _projectiles = pm;
    }

    public override void _Ready()
    {
        if (ProjectileManagerPath != null && !ProjectileManagerPath.IsEmpty)
        {
            _projectiles = GetNodeOrNull<ProjectileManager>(ProjectileManagerPath);
        }
    }

    public override void _Process(double delta)
    {
        _fireTimer -= (float)delta;
        _statuses?.Tick((float)delta);

        if (_fireTimer > 0 || _encounter == null) return;

        var target = _encounter.FindNearestEnemy();
        if (target == null) return;

        var (damage, cadence) = ComputeModifiedFireParams();
        var killed = target.ApplyDamage(damage);
        _projectiles?.SpawnProjectile(Origin, target.Position);
        if (killed)
        {
            // reward added elsewhere
        }

        _fireTimer = cadence;
    }

    public void ApplyWeaponModifier(string stat, float magnitude, float duration)
    {
        // Example: fire_rate stat uses multiplicative change for duration.
        if (stat == "fire_rate")
        {
            FireRateSeconds = System.Math.Max(0.05f, FireRateSeconds * magnitude);
            if (duration > 0)
            {
                _statuses?.Add(new StatusEffect
                {
                    Id = "fire_rate_temp",
                    Duration = duration,
                    Magnitude = magnitude,
                    Stacking = StatusStacking.Refresh
                });
            }
        }
        else if (stat == "damage")
        {
            Damage += magnitude;
        }
    }

    public void EmitAreaDamage(float magnitude)
    {
        // Hook for scene VFX; deal flat damage to all enemies.
        // A simple approach would be to ask the lane manager to iterate enemies.
        // Implementation to be added alongside visuals.
    }

    private (float damage, float cadence) ComputeModifiedFireParams()
    {
        float damage = Damage;
        float cadence = FireRateSeconds;

        if (_statuses != null)
        {
            foreach (var status in _statuses.Effects)
            {
                if (status.Id == "pierce") continue; // handled by projectile logic later
                if (status.Id == "fire_rate_temp")
                {
                    cadence = System.Math.Max(0.05f, cadence * status.Magnitude);
                }
            }
        }

        return (damage, cadence);
    }
}

