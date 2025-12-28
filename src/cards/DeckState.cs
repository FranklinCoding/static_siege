using System;
using System.Collections.Generic;
using Godot;
using StaticSiege.Effects;

namespace StaticSiege.Cards;

/// <summary>
/// Draw/hand/discard/exhaust logic. Rendering and input live elsewhere.
/// </summary>
public sealed class DeckState
{
    private readonly List<CardInstance> _drawPile = new();
    private readonly List<CardInstance> _discard = new();
    private readonly List<CardInstance> _hand = new();
    private readonly List<CardInstance> _exhaust = new();

    private int _handLimit = 5;

    public IReadOnlyList<CardInstance> Hand => _hand;

    public void Init(IEnumerable<CardDef> deck, RandomNumberGenerator rng)
    {
        _drawPile.Clear();
        _discard.Clear();
        _hand.Clear();
        _exhaust.Clear();

        foreach (var def in deck)
        {
            _drawPile.Add(new CardInstance(def));
        }

        Shuffle(_drawPile, rng);
        TryDraw(4, rng);
    }

    public void OnWaveStart() => DiscardHand();

    public bool TryDraw(int count, RandomNumberGenerator rng)
    {
        var drewAny = false;
        for (var i = 0; i < count; i++)
        {
            if (_hand.Count >= _handLimit) break;
            if (_drawPile.Count == 0)
            {
                if (_discard.Count == 0) break;
                RecycleDiscardIntoDraw(rng);
            }

            var idx = _drawPile.Count - 1;
            _hand.Add(_drawPile[idx]);
            _drawPile.RemoveAt(idx);
            drewAny = true;
        }
        return drewAny;
    }

    public bool TryPlay(int handIndex, Core.Resources resources, Effects.EffectResolver resolver)
    {
        if (handIndex < 0 || handIndex >= _hand.Count) return false;
        var card = _hand[handIndex];
        if (!resources.TrySpendFuel(card.Def.Cost)) return false;

        resolver.Resolve(card, card.Def.Effects);

        if (card.Def.Exiles) _exhaust.Add(card);
        else _discard.Add(card);

        _hand.RemoveAt(handIndex);
        return true;
    }

    public void DiscardHand()
    {
        _discard.AddRange(_hand);
        _hand.Clear();
    }

    public void ModifyHandSize(int delta, bool permanent)
    {
        _handLimit = Math.Max(1, _handLimit + delta);
        if (!permanent) return;
        // Placeholder: persist to meta or run-level flags as needed.
    }

    private void RecycleDiscardIntoDraw(RandomNumberGenerator rng)
    {
        _drawPile.AddRange(_discard);
        _discard.Clear();
        Shuffle(_drawPile, rng);
    }

    private static void Shuffle(List<CardInstance> list, RandomNumberGenerator rng)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.RandiRange(0, i);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

