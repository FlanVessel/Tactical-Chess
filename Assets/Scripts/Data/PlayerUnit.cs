using UnityEngine;

public class PlayerUnit : Unit
{
    private bool isSelected;

    public bool IsSelected => isSelected;

    public override void Initialize(Vector3Int initialCell)
    {
        base.Initialize(initialCell);
        isSelected = false;
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
