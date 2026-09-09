using UnityEngine;
using System;
using System.Collections;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected UnitData unitData;

    protected Vector3Int currentCell;
    protected bool hasMoved;
    protected bool hasActed;
    protected bool isActive;
    protected bool isMoving;

    protected int runtimeMaxHealth;
    protected int currentHealth;
    
    protected int runtimeMoveRange;
    
    protected int maximumMana;
    protected int currentMana;
    protected int manaRecovery;
    
    protected int temporaryDamage;
    
    public event Action<Unit> Died;
    public event Action<Unit, int> Damaged;
    public  event Action StatsChanged;

    public UnitData Data => unitData;
    public Vector3Int CurrentCell => currentCell;
    public bool HasMoved => hasMoved;
    public bool HasActed => hasActed;
    public bool IsActive => isActive;
    public bool IsMoving => isMoving;
    
    public int MaxHealth => runtimeMaxHealth;
    public int CurrentHealth => currentHealth;
    
    public int MoveRange => runtimeMoveRange;
    
    public int MaximumMana => maximumMana;
    public int CurrentMana => currentMana;
    public int ManaRecovery => manaRecovery;
    
    public int TemporaryDamage => temporaryDamage;
    
    public bool IsDead => currentHealth <= 0;

    protected void NotifyStatsChanged()
    {
        StatsChanged?.Invoke();
    }

    public virtual void Initialize(Vector3Int initialCell)
    {
        if (unitData == null)
        {
            Debug.LogError($"{name} no tiene un UnitData asignado.");
            return;
        }

        currentCell = initialCell;

        temporaryDamage = 0;

        runtimeMaxHealth = unitData.GenerateMaxHealth();
        runtimeMoveRange = unitData.GenerateMoveRange();
        currentHealth = runtimeMaxHealth;
        
        maximumMana = unitData.GenerateMana();
        manaRecovery = unitData.GenerateManaRecovery();
        currentMana = maximumMana;

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
        
        StatusMana();

        isActive = true;
        isMoving = false;
        hasMoved = false;
        hasActed = false;
        
        NotifyStatsChanged();
    }

    public virtual void EndTurn()
    {
        isActive = false;
        isMoving = false;
        
        temporaryDamage = 0;
        RecoveryMana();
        NotifyStatsChanged();
    }

    public bool CanMove()
    {
        return !IsDead && isActive && !isMoving && !hasMoved;
    }

    public bool UseMovement()
    {
        if (!CanMove()) return false;

        hasMoved = true;
        NotifyStatsChanged();
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

    public bool CanSpendMana(int amount)
    {
        if (amount < 0) return false;
        return currentMana >= amount;
    }

    public bool SpendMana(int amount)
    {
        if (!CanSpendMana(amount)) return false;
        
        currentMana -= amount;
        NotifyStatsChanged();
        
        Debug.Log($"{name} gasto {amount} de mana. " + $"Mana restante: {currentMana}/{maximumMana}.");
        return true;
    }

    public void RecoveryMana()
    {
        int previousMana = currentMana;
        currentMana = Mathf.Min(currentMana + manaRecovery, maximumMana);
        
        NotifyStatsChanged();
        
        int recoveredAmount = currentMana - previousMana;
        
        Debug.Log($"{name} recupero {recoveredAmount} de mana. " + $"Mana actual: {currentMana}/{maximumMana}.");
    }

    public void StatusMana()
    {
        int statusMana = currentMana;
        Debug.Log($"{name} su mana actual es: {statusMana}.");

        int recoveryMana = manaRecovery;
        Debug.Log($"{name} recupera esta cantidad de mana: {recoveryMana}.");
    }
    
    public void AddTemporaryDamage(int amount)
    {
        if (amount <= 0) return;
        temporaryDamage += amount;
        Debug.Log($"{name} recibio + {amount} de dano. " + $"Resultado de aumento de dano es: {temporaryDamage}.");
        NotifyStatsChanged();
    }

    public int CalculateAttackDamage(int baseDamage)
    {
        return Mathf.Max(0, baseDamage + temporaryDamage);
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
        NotifyStatsChanged();
        Damaged?.Invoke(this, amount);

        Debug.Log($"{name} recibio {amount} de dano. " + $"Vida: {currentHealth}/{runtimeMaxHealth}");
        
        if (currentHealth == 0) Die();
        if (IsDead) Die();
    }

    public bool Heal(int amount)
    {
        if (amount <= 0) return false;
        if (IsDead) return false;
        
        if (currentHealth >= runtimeMaxHealth)
        {
            Debug.Log($"{name} tiene vida completa.");
            return false;
        }
        
        int previousHealth = currentHealth;

        currentHealth = Mathf.Min(currentHealth + amount, runtimeMaxHealth);
        
        int recoveredAmount = currentHealth - previousHealth;

        Debug.Log($"{name} recupero esta cantidad de de vida: {recoveredAmount}." + $"Vida Actual: {currentHealth}/{runtimeMaxHealth}");
        NotifyStatsChanged();
        return true;
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
