using Godot;

namespace StaticSiege.UI;

/// <summary>
/// Placeholder reward screen with a single hardcoded reward for PR #3.
/// </summary>
public partial class RewardView : Control
{
    public System.Action? OnRewardChosen;

    public override void _Ready()
    {
        var lbl = new Label { Text = "Reward: Heal 10 and gain +5 scrap (placeholder)" };
        AddChild(lbl);
        var btn = new Button { Text = "Take Reward" };
        btn.Pressed += () => OnRewardChosen?.Invoke();
        AddChild(btn);
        Visible = false;
    }
}

