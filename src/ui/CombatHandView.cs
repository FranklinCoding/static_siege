using Godot;
using StaticSiege.Cards;
using StaticSiege.Core;

namespace StaticSiege.UI;

/// <summary>
/// Minimal hand view: shows cards as buttons, raises play requests, flashes on insufficient energy.
/// </summary>
public partial class CombatHandView : Control
{
    public System.Action<int>? OnPlayRequested;
    public System.Action<int>? OnInsufficient;
    public System.Action? OnDeckEmpty;
    private DeckState? _deck;
    private Resources? _resources;
    private VBoxContainer _root = new();
    private HBoxContainer _row = new();
    private Label _status = new();

    public override void _Ready()
    {
        _root.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _root.SizeFlagsVertical = SizeFlags.ShrinkCenter;
        _root.Alignment = BoxContainer.AlignmentMode.Center;

        _row.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _row.SizeFlagsVertical = SizeFlags.ShrinkCenter;
        _row.Alignment = BoxContainer.AlignmentMode.Center;
        _root.AddChild(_row);

        _status.Modulate = new Color(1, 0.8f, 0.2f);
        _status.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _status.HorizontalAlignment = HorizontalAlignment.Center;
        _status.CustomMinimumSize = new Vector2(0, 16);
        _root.AddChild(_status);

        AddChild(_root);

        Visible = false;
    }

    public void Bind(DeckState deck, Resources resources)
    {
        _deck = deck;
        _resources = resources;
        Refresh();
    }

    public void Refresh()
    {
        for (int i = _row.GetChildCount() - 1; i >= 0; i--) _row.GetChild(i).QueueFree();
        if (_deck == null) return;
        for (int i = 0; i < _deck.Hand.Count; i++)
        {
            var card = _deck.Hand[i];
            var btn = new Button { Text = $"{card.Def.Name} ({card.Def.Cost})", SizeFlagsHorizontal = SizeFlags.ShrinkCenter };
            btn.TooltipText = $"{card.Def.Name} (Cost {card.Def.Cost})\n{card.Def.Description}";
            int idx = i;
            btn.Pressed += () => OnPlayRequested?.Invoke(idx);
            _row.AddChild(btn);
        }

        if (_deck.Hand.Count >= _deck.HandLimit)
        {
            _status.Text = "Hand full";
            _status.Show();
        }
        else
        {
            _status.Text = "";
            _status.Hide();
        }
    }

    public void FlashInsufficient(int handIndex)
    {
        if (handIndex < 0 || handIndex >= _row.GetChildCount()) return;
        var child = _row.GetChild(handIndex);
        if (child is Button b && IsInstanceValid(b))
        {
            b.Modulate = new Color(1, 0.3f, 0.3f);
            GetTree().CreateTimer(0.3f).Timeout += () =>
            {
                if (IsInstanceValid(b))
                    b.Modulate = Colors.White;
            };
        }
    }

    public void ShowDeckEmptyNotice()
    {
        _status.Text = "Deck empty";
        _status.Show();
        GetTree().CreateTimer(0.8f).Timeout += () =>
        {
            _status.Text = "";
            _status.Hide();
        };
    }
}

