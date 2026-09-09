using UnityEngine;
using System.Collections.Generic;

public class PawnDeckController : MonoBehaviour
{
    private readonly List<CardData> _drawPile = new();
    private readonly List<CardData> _hand = new();
    private readonly List<CardData> _discardPile = new();

    public IReadOnlyList<CardData> Hand => _hand;
    public int DrawPileCount => _drawPile.Count;
    public int DiscardPileCount => _discardPile.Count;

    public void InitializeDeck(DeckData deckData)
    {
        _drawPile.Clear();
        _hand.Clear();
        _discardPile.Clear();

        if (deckData == null)
        {
            Debug.LogError(
                $"{name} no recibió un DeckData."
            );

            return;
        }

        foreach (CardData card in deckData.Cards)
        {
            if (card == null) continue;

            _drawPile.Add(card);
        }

        DrawInitialHand(3);
        PrintHand();
    }

    private void DrawInitialHand(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
        }
    }

    public bool DrawCard()
    {
        if (_drawPile.Count == 0)
        {
            Debug.Log($"{name} no tiene más cartas para robar.");
            return false;
        }

        CardData card = _drawPile[0];

        _drawPile.RemoveAt(0);
        _hand.Add(card);

        return true;
    }

    public bool HasCardInHand(CardData card)
    {
        if (card == null) return false;
        return _hand.Contains(card);
    }

    public bool UseCard(CardData card)
    {
        if (!HasCardInHand(card)) return false;
        
        _hand.Remove(card);
        _discardPile.Add(card);
        
        Debug.Log($"{name} utilizo {card.CardName} {card.Description} " + $"Cartas restantes: {_hand.Count}");
        return true;
    }

    public void RestoreCard()
    {
        if (_discardPile.Count == 0) return;

        foreach (CardData card in _discardPile)
        {
            if  (card == null) continue;
            _hand.Add(card);
        }
        _discardPile.Clear();
        
        Debug.Log($"{name} recupero sus cartas.");
    }

    private void PrintHand()
    {
        Debug.Log($"{name} recibió {_hand.Count} cartas:");

        foreach (CardData card in _hand)
        {
            Debug.Log(
                $"- {card.CardName}: {card.Description}"
            );
        }
    }
}
