using Godot;
using StaticSiege.Run;

namespace StaticSiege.UI;

/// <summary>
/// Minimal map chooser for PR #3: renders current node and next selectable nodes as buttons.
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

        var current = _runManager.GetCurrentNode();
        AddLabel($"Current: {current?.Id} [{current?.Type}] (phase: {_runManager.Context.Phase})");

        if (current != null && _runManager.Context.Phase == RunPhase.InMap)
        {
            AddButton($"Enter {current.Id} ({current.Type})", () => Handle(current));
        }

        foreach (var next in _runManager.GetAvailableNext())
        {
            AddButton($"Next: {next.Id} ({next.Type})", () => Handle(next));
        }
    }

    private void Handle(MapNodeDef node)
    {
        switch (node.Type)
        {
            case MapNodeType.Battle: OnBattleSelected?.Invoke(node); break;
            case MapNodeType.Boss:   OnBossSelected?.Invoke(node); break;
            case MapNodeType.Shop:   OnShopSelected?.Invoke(node); break;
            case MapNodeType.Rest:   OnRestSelected?.Invoke(node); break;
        }
    }

    private void AddButton(string text, System.Action onPress)
    {
        var btn = new Button { Text = text, SizeFlagsHorizontal = SizeFlags.ExpandFill };
        btn.Pressed += () => onPress();
        AddChild(btn);
    }

    private void AddLabel(string text)
    {
        var lbl = new Label { Text = text };
        AddChild(lbl);
    }

    private void ClearChildren()
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }
    }
}

