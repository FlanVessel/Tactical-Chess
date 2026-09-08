using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]

public class CardData : ScriptableObject
{
    [Header("Presentación")]
    [SerializeField] private string cardName;
    [SerializeField] private Sprite cardImage;

    [TextArea(2, 4)]
    [SerializeField] private string description;

    [Header("Coste")]
    [SerializeField, Min(0)]
    private int manaCost;

    [Header("Acción")]
    [SerializeField] private UnitActionData action;

    public string CardName => cardName;
    public Sprite CardImage => cardImage;
    public string Description => description;
    public int ManaCost => manaCost;
    public UnitActionData Action => action;
}
