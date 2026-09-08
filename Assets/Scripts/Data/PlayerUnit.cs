using UnityEngine;

public class PlayerUnit : Unit
{
    private bool isSelected;
    private PawnDeckController _deckController;

    public bool IsSelected => isSelected;
    public PawnDeckController DeckController => _deckController;

    public override void Initialize(Vector3Int initialCell)
    {
        base.Initialize(initialCell);
        isSelected = false;
        
        _deckController = GetComponent<PawnDeckController>();

        if (_deckController == null)
        {
            Debug.LogError($"{name} no tiene PawnDeckController.");
            return;
        }

        if (unitData.StartingDeck == null)
        {
            Debug.LogError($"{name} no tiene un mazo asignado.");
            return;
        }
        
        _deckController.InitializeDeck(unitData.StartingDeck);
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        isSelected = false;
    }

    public override void EndTurn()
    {
        Deselect();
        base.EndTurn();
    }

    public bool Select()
    {
        if (!isActive) return false;

        if (isMoving) return false;

        isSelected = true;

        return true;
    }

    public void Deselect()
    {
        isSelected = false;
    }
}
