using UnityEngine;
using System;
using System.Collections.Generic;

public class CardHandUI : MonoBehaviour
{
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform cardsContainer;

    private readonly List<CardView> _createdCards = new();

    public void ShowHand(IReadOnlyList<CardData> cards, Action<CardData> onCardSelected)
    {
        ClearHand();

        if (cards == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        foreach (CardData card in cards)
        {
            if (card == null) continue;

            CardView cardView = Instantiate(cardPrefab, cardsContainer);

            cardView.Setup(card, onCardSelected);
            _createdCards.Add(cardView);
        }
    }

    public void HideHand()
    {
        ClearHand();
        gameObject.SetActive(false);
    }

    private void ClearHand()
    {
        foreach (CardView card in _createdCards)
        {
            if (card != null) Destroy(card.gameObject);
        }
        
        _createdCards.Clear();
    }
}
