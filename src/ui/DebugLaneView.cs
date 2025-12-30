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
            var barWidth = 26f;
            var barHeight = 4f;
            var barX = pos.X - barWidth / 2f;
            var barY = pos.Y - 16f;
            // Background
            DrawRect(new Rect2(barX, barY, barWidth, barHeight), new Color(0, 0, 0, 0.6f));
            // Foreground
            DrawRect(new Rect2(barX, barY, barWidth * hpPct, barHeight), new Color(0.2f, 1f, 0.2f, 0.9f));
        }

        // Core indicator
        DrawCircle(center, 18, new Color(0.1f, 0.9f, 0.9f, 0.8f));

        var font = GetFont();
        if (font != null)
        {
            // HUD overlay
            if (_resources != null)
            {
                DrawString(font, new Vector2(10, 20),
                    $"HP {_resources.Health}/{_resources.HealthMax}  Shield {_resources.Shield}  Energy {_resources.Energy}/{_resources.EnergyCap}  Scrap {_resources.Scrap}",
                    modulate: new Color(0.8f, 0.95f, 1f));
            }

            if (_runState != null)
            {
                DrawString(font, new Vector2(size.X - 160, 20),
                    $"Wave {_runState.Wave}", modulate: new Color(1f, 1f, 0.6f));
            }

            DrawString(font, new Vector2(10, size.Y - 20),
                $"Enemies: {_encounter.Enemies.Count}", modulate: new Color(0.8f, 0.9f, 1f));
        }
    }

    public void Bind(Resources resources, RunState runState)
    {
        _resources = resources;
        _runState = runState;
    }

    private Font? GetFont()
    {
        // Minimal fallback: no explicit font; drawing will be skipped if null.
        return null;
    }
}

