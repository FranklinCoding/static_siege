using Godot;
using System.Linq;
using StaticSiege.Run;

namespace StaticSiege.UI;

/// <summary>
/// Minimal map chooser with clearer layout and phase-aware interactivity.
/// </summary>
public partial class MapView : Control
{
    [Export] public NodePath? RunManagerPath;

    public System.Action<MapNodeDef>? OnBattleSelected;
    public System.Action<MapNodeDef>? OnBossSelected;
    public System.Action<MapNodeDef>? OnShopSelected;
    public System.Action<MapNodeDef>? OnRestSelected;

    private RunManager? _runManager;

    public override void _Ready()
    {
        _runManager = GetNodeOrNull<RunManager>(RunManagerPath ?? string.Empty);
        Refresh();
    }

    public void Refresh()
    {
        if (_runManager == null) return;
        ClearChildren();

        var margin = new MarginContainer
        {
            CustomMinimumSize = new Vector2(200, 0),
            SizeFlagsHorizontal = SizeFlags.Fill,
            SizeFlagsVertical = SizeFlags.Fill,
            MouseFilter = MouseFilterEnum.Ignore
        };
        margin.AddThemeConstantOverride("margin_left", 6);
        margin.AddThemeConstantOverride("margin_right", 6);
        margin.AddThemeConstantOverride("margin_top", 6);
        margin.AddThemeConstantOverride("margin_bottom", 6);

        var panel = new PanelContainer
        {
            SizeFlagsHorizontal = SizeFlags.Fill,
            SizeFlagsVertical = SizeFlags.Fill
        };
        margin.AddChild(panel);

        var vbox = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };

        var current = _runManager.GetCurrentNode();
        var phase = _runManager.Context.Phase;
        vbox.AddChild(new Label { Text = $"Phase: {phase}" });
        vbox.AddChild(new Label { Text = $"Current: {current?.Id ?? "None"} [{current?.Type}]" });

        if (current != null)
        {
            if (phase == RunPhase.InMap)
            {
                vbox.AddChild(MakeButton($"Enter {current.Id} ({current.Type})", () => Handle(current)));
            }
            else
            {
                vbox.AddChild(new Label { Text = "(Locked: in battle/reward)" });
            }
        }

        var nextNodes = _runManager.GetAvailableNext().ToList();
        vbox.AddChild(new Label { Text = "Next:" });
        if (nextNodes.Count == 0)
        {
            vbox.AddChild(new Label { Text = "None" });
        }
        else
        {
            foreach (var next in nextNodes)
            {
                var btn = MakeButton($"{next.Id} ({next.Type})", () => Handle(next));
                btn.Disabled = phase != RunPhase.InMap;
                vbox.AddChild(btn);
            }
        }

        panel.AddChild(vbox);
        AddChild(margin);
    }

    private Button MakeButton(string text, System.Action onPress)
    {
        var btn = new Button { Text = text, SizeFlagsHorizontal = SizeFlags.ExpandFill };
        btn.Pressed += () => onPress();
        return btn;
    }

    private void Handle(MapNodeDef node)
    {
        switch (node.Type)
        {
            case MapNodeType.Battle:
                OnBattleSelected?.Invoke(node);
                break;
            case MapNodeType.Boss:
                OnBossSelected?.Invoke(node);
                break;
            case MapNodeType.Shop:
                OnShopSelected?.Invoke(node);
                break;
            case MapNodeType.Rest:
                OnRestSelected?.Invoke(node);
                break;
        }
    }

    private void ClearChildren()
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }
    }
}

