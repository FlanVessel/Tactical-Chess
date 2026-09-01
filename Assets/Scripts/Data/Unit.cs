using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected UnitData unitData;

    protected Vector3Int currentCell;
    protected int currentHealth;
    protected int remainingMovement;
    protected bool isActive;
    protected bool isMoving;

    public UnitData Data => unitData;
    public Vector3Int CurrentCell => currentCell;
    public int CurrentHealth => currentHealth;
    public int RemainingMovement => remainingMovement;
    public bool IsActive => isActive;
    public bool IsMoving => isMoving;

    public virtual void Initialize(Vector3Int initialCell)
    {
        if (unitData == null)
        {
            Debug.LogError($"{name} no tiene un UnitData asignado.");
            return;
        }

        currentCell = initialCell;
        currentHealth = unitData.MaxHealth;
        remainingMovement = 0;

        isActive = false;
        isMoving = false;
    }

    public virtual void BeginTurn()
    {
        if (unitData == null)
        {
            Debug.LogError($"{name} no puede comenzar su turno sin UnitData.");
            return;
        }

        isActive = true;
        isMoving = false;

        remainingMovement = unitData.MovePoints;
    }

    public virtual void EndTurn()
    {
        isActive = false;
        isMoving = false;
        remainingMovement = 0;
    }

    public bool CanSpendMovement(int amount)
    {
        if (amount < 0) return false;

        return isActive && !isMoving && remainingMovement >= amount;
    }

    public bool SpendMovement(int amount)
    {
        if (!CanSpendMovement(amount)) return false;

        remainingMovement -= amount;
        return true;
    }

    public void SetCurrentCell(Vector3Int cell)
    {
        currentCell = cell;
    }

    public void SetMoving(bool value)
    {
        isMoving = value;
    }
}
