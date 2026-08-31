using UnityEngine;
using System.Collections.Generic;

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
    }
}
