using Godot;
using StaticSiege.Core;

namespace StaticSiege.UI;

/// <summary>
/// Minimal always-on HUD for combat readability.
/// </summary>
public partial class HudView : Control
{
    [Export] public NodePath? HpLabelPath;
    [Export] public NodePath? EnergyLabelPath;
    [Export] public NodePath? ScrapLabelPath;
    [Export] public NodePath? EnemyLabelPath;

    private Label? _hp;
    private Label? _energy;
    private Label? _scrap;
    private Label? _enemy;
    private Resources? _resources;
    private int _enemyCount;
    private string _phaseText = string.Empty;

    public override void _Ready()
    {
        _hp = GetNodeOrNull<Label>(HpLabelPath ?? string.Empty);
        _energy = GetNodeOrNull<Label>(EnergyLabelPath ?? string.Empty);
        _scrap = GetNodeOrNull<Label>(ScrapLabelPath ?? string.Empty);
        _enemy = GetNodeOrNull<Label>(EnemyLabelPath ?? string.Empty);
    }

    public void Bind(Resources resources)
    {
        _resources = resources;
    }

    public override void _Process(double delta)
    {
        if (_resources == null) return;
        if (_hp != null) _hp.Text = $"HP: {_resources.Health}/{_resources.HealthMax}";
        if (_energy != null) _energy.Text = $"Energy: {_resources.Energy}/{_resources.EnergyCap}";
        if (_scrap != null) _scrap.Text = $"Scrap: {_resources.Scrap}";
        if (_enemy != null)
        {
            _enemy.Text = $"Enemies: {_enemyCount} ({_phaseText})";
        }
    }

    public void SetEncounterInfo(int enemyCount, Run.RunPhase? phase)
    {
        _enemyCount = enemyCount;
        _phaseText = phase?.ToString() ?? string.Empty;
    }
}

