using Godot;
using StaticSiege.Combat;
using StaticSiege.Core;

namespace StaticSiege.UI;

/// <summary>
/// Minimal 2D debug visuals: draws arena enemies as circles around the core.
/// NOTE: Lanes are deprecated; this view now reads from EncounterManager.
/// </summary>
public partial class DebugLaneView : Node2D
{
    [Export] public NodePath? EncounterManagerPath;

    private EncounterManager? _encounter;
    private Resources? _resources;
    private RunState? _runState;

    public override void _Ready()
    {
        _encounter = GetNode<EncounterManager>(EncounterManagerPath ?? string.Empty);
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_encounter == null) return;

        var size = GetViewportRect().Size;
        var center = size / 2f;
        var scale = 20f; // world unit to screen scaling for debug

        foreach (var enemy in _encounter.Enemies)
        {
            var pos = center + enemy.Position * scale;
            DrawCircle(pos, 10, new Color(0.9f, 0.2f, 0.2f, 0.9f));

            var hpPct = Mathf.Clamp(enemy.Health / enemy.Def.MaxHealth, 0, 1);
            DrawRect(new Rect2(pos.X - 12, pos.Y - 14, 24 * hpPct, 3), new Color(0.2f, 1f, 0.2f, 0.9f));
        }

        // Core indicator
        DrawCircle(center, 18, new Color(0.1f, 0.9f, 0.9f, 0.8f));

        // HUD overlay
        if (_resources != null)
        {
            DrawString(GetFont(), new Vector2(10, 20),
                $"HP {_resources.Health}/{_resources.HealthMax}  Shield {_resources.Shield}  Fuel {_resources.Fuel}/{_resources.FuelMax}  Scrap {_resources.Scrap}",
                modulate: new Color(0.8f, 0.95f, 1f));
        }

        if (_runState != null)
        {
            DrawString(GetFont(), new Vector2(size.X - 160, 20),
                $"Wave {_runState.Wave}", modulate: new Color(1f, 1f, 0.6f));
        }
    }

    public void Bind(Resources resources, RunState runState)
    {
        _resources = resources;
        _runState = runState;
    }

    private Font GetFont()
    {
        // Use the default font; if absent, Godot supplies a fallback.
        return GetThemeDefaultFont();
    }
}

