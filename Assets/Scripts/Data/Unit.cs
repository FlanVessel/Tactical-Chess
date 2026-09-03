using UnityEngine;
using System;
using NUnit.Framework;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected UnitData unitData;

    protected Vector3Int currentCell;
    protected int currentHealth;
    protected bool hasMoved;
    protected bool hasActed;
    protected bool isActive;
    protected bool isMoving;
    public event Action<Unit> Died;

    public UnitData Data => unitData;
    public Vector3Int CurrentCell => currentCell;
    public int CurrentHealth => currentHealth;
    public bool HasMoved => hasMoved;
    public bool HasActed => hasActed;
    public bool IsActive => isActive;
    public bool IsMoving => isMoving;
    public bool IsDead => currentHealth <= 0;

    public virtual void Initialize(Vector3Int initialCell)
    {
        if (unitData == null)
        {
            Debug.LogError($"{name} no tiene un UnitData asignado.");
            return;
        }

        currentCell = initialCell;
        currentHealth = unitData.MaxHealth;

        hasMoved = false;
        hasActed = false;
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

        if (IsDead) return;

        isActive = true;
        isMoving = false;
        hasMoved = false;
        hasActed = false;
    }

    public virtual void EndTurn()
    {
        isActive = false;
        isMoving = false;
    }

    public bool CanMove()
    {
        return !IsDead && isActive && !isMoving && !hasMoved;
    }

    public bool UseMovement()
    {
        if (!CanMove()) return false;

        hasMoved = true;
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

    public bool CanAct()
    {
        return !IsDead && isActive && !isMoving && !hasActed;
    }

    public bool UseAction()
    {
        if (!CanAct()) return false;

        hasActed = true;
        return true;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        if (amount <= 0)
        {
            Debug.Log($"Dano recibido: {amount}.");
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);

        Debug.Log($"{name} recibio {amount} de dano. " + $"Vida: {currentHealth}/{unitData.MaxHealth}");

        if (currentHealth == 0) Die();
    }

    private void Heal(int amount)
    {
        if (IsDead) return;

        if (amount <= 0)
        {
            Debug.Log($"Has recibido esta cantidad de curacion: {amount}.");
            return;
        }

        currentHealth = Mathf.Min(unitData.MaxHealth, currentHealth + amount);

        Debug.Log($"{name} recupero esta cantidad de de vida: {amount}." + $"Vida Actual: {currentHealth}/{unitData.MaxHealth}");
    }

    protected virtual void Die()
    {
        isActive = false;
        isMoving = false;

        Debug.Log($"{name} ha muerto.");

        Died?.Invoke(this);

        gameObject.SetActive(false);
    }
}
