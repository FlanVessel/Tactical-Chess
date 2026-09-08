using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Cards/Deck Data")]

public class DeckData : ScriptableObject
{
    [SerializeField] private List<CardData> cards = new();
    
    public IReadOnlyList<CardData> Cards => cards;
}
