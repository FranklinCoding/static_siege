using Godot;
using StaticSiege.Combat;
using StaticSiege.Core;

namespace StaticSiege.UI;

/// <summary>
/// Minimal 2D debug visuals: draws lanes as vertical bands and enemies as circles moving toward the core.
/// </summary>
public partial class DebugLaneView : Node2D
{
    [Export] public NodePath? LaneManagerPath;

    private LaneManager? _lanes;
    private Resources? _resources;
    private RunState? _runState;

    public override void _Ready()
    {
        _lanes = GetNode<LaneManager>(LaneManagerPath ?? string.Empty);
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_lanes == null) return;

        var size = GetViewportRect().Size;
        var laneWidth = size.X / Mathf.Max(1, _lanes.Lanes.Count);
        var top = 40f;
        var bottom = size.Y - 80f;
        var laneHeight = bottom - top;

        for (int i = 0; i < _lanes.Lanes.Count; i++)
        {
            var laneX = i * laneWidth;
            var color = new Color(0.1f, 0.15f, 0.3f, 0.4f);
            DrawRect(new Rect2(laneX + 4, top, laneWidth - 8, laneHeight), color);

            foreach (var enemy in _lanes.Lanes[i].Enemies)
            {
                var t = Mathf.Clamp(1f - (enemy.DistanceToCore / Mathf.Max(0.001f, _lanes.Lanes[i].Length)), 0, 1);
                var y = Mathf.Lerp(top, bottom, t);
                var x = laneX + laneWidth * 0.5f;
                DrawCircle(new Vector2(x, y), 10, new Color(0.9f, 0.2f, 0.2f, 0.9f));

                // Health bar
                var hpPct = Mathf.Clamp(enemy.Health / enemy.Def.MaxHealth, 0, 1);
                DrawRect(new Rect2(x - 12, y - 14, 24 * hpPct, 3), new Color(0.2f, 1f, 0.2f, 0.9f));
            }
        }

        // Core indicator
        var corePos = new Vector2(size.X * 0.5f, bottom + 30f);
        DrawCircle(corePos, 18, new Color(0.1f, 0.9f, 0.9f, 0.8f));

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

