using System.Collections.Generic;
using System.Linq;
using Godot;
using StaticSiege.Core;
using StaticSiege.Entities;

namespace StaticSiege.Combat;

/// <summary>
/// Minimal encounter runner: spawns enemies around the arena using EncounterDef.
/// TODO: Later add phases, elites, boss logic, rewards.
/// </summary>
public partial class EncounterManager : Node
{
    [Export] public NodePath? TurretPath;

    public System.Action? OnEncounterComplete;

    private readonly List<EnemyInstance> _enemies = new();
    private readonly List<ActiveSpawn> _spawns = new();
    private Dictionary<string, EnemyDef> _enemyLookup = new();
    private TurretController? _turret;
    private Resources? _resources;
    private bool _encounterActive;

    public IReadOnlyList<EnemyInstance> Enemies => _enemies;

    public override void _Ready()
    {
        GD.Randomize();
        _turret = GetNodeOrNull<TurretController>(TurretPath ?? string.Empty);
        ProcessMode = ProcessModeEnum.Always;
        SetProcess(true);
    }

    public void Init(IEnumerable<EnemyDef> enemies, TurretController turret, Resources resources)
    {
        _enemyLookup = enemies.ToDictionary(e => e.Id, e => e);
        _turret = turret;
        _resources = resources;
    }

    public void StartEncounter(EncounterDef def)
    {
        _enemies.Clear();
        _spawns.Clear();
        GD.Print($"[EncounterManager] StartEncounter id={def.Id} spawns={def.Spawns.Count}");
        foreach (var s in def.Spawns)
        {
            if (!_enemyLookup.TryGetValue(s.Enemy, out var defEnemy)) continue;
            _spawns.Add(new ActiveSpawn(s, defEnemy));
            GD.Print($"[EncounterManager] queued spawn enemy={s.Enemy} count={s.Count} cadence={s.Cadence}");
        }
        _encounterActive = true;
    }

    public override void _Process(double delta)
    {
        TickSpawns((float)delta);
        TickEnemies((float)delta);
        MaybeComplete();
    }

    public EnemyInstance? FindNearestEnemy()
    {
        EnemyInstance? nearest = null;
        foreach (var e in _enemies)
        {
            if (nearest == null || e.DistanceToCore < nearest.DistanceToCore)
            {
                nearest = e;
            }
        }
        return nearest;
    }

    private void TickSpawns(float delta)
    {
        for (int i = _spawns.Count - 1; i >= 0; i--)
        {
            var spawn = _spawns[i];
            spawn.Timer -= delta;
            if (spawn.Timer <= 0)
            {
                SpawnEnemy(spawn.Def, spawn.Config);
                spawn.Spawned++;
                spawn.Timer = spawn.Config.Cadence;
            }
            if (spawn.Spawned >= spawn.Config.Count) _spawns.RemoveAt(i);
        }
    }

    private void SpawnEnemy(EnemyDef def, EncounterSpawn cfg)
    {
        var angle = Mathf.DegToRad((float)GD.RandRange(0, cfg.ArcDegrees));
        var radius = cfg.Radius;
        var pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        _enemies.Add(new EnemyInstance(def, pos));
    }

    private void TickEnemies(float delta)
    {
        if (_turret == null || _resources == null) return;

        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            var e = _enemies[i];
            e.Tick(delta);
            if (e.DistanceToCore <= 0)
            {
                e.OnReachCore(_resources);
                _enemies.RemoveAt(i);
            }
            else if (e.IsDead)
            {
                // Gain resource on kill, clamped at cap by Resources.
                _resources.AddScrap(e.Def.ScrapReward);
                _resources.AddEnergy(1);
                _enemies.RemoveAt(i);
            }
        }
    }

    private void MaybeComplete()
    {
        if (!_encounterActive) return;
        var spawnsDone = _spawns.Count == 0;
        var enemiesDone = _enemies.Count == 0;
        if (spawnsDone && enemiesDone)
        {
            _encounterActive = false;
            OnEncounterComplete?.Invoke();
        }
    }

    private sealed class ActiveSpawn
    {
        public EncounterSpawn Config { get; }
        public EnemyDef Def { get; }
        public int Spawned { get; set; }
        public float Timer { get; set; }
        public ActiveSpawn(EncounterSpawn cfg, EnemyDef def)
        {
            Config = cfg;
            Def = def;
            Timer = cfg.Cadence;
        }
    }
}

