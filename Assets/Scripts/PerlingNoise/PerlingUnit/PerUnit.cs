using UnityEngine;

public class PerUnit : MonoBehaviour
{
    private Vector3Int _currentCell;

    private int _health;
    private int _currentHealth;

    private int _movementPoints;
    private int _remainingMovementPoints;

    private int _attackDamage;
    
    private bool _initialized;
    private bool _isDead;
    
    
    public Vector3Int CurrentCell => _currentCell;
    
    public int Health => _health;
    public int CurrentHealth => _currentHealth;
    
    public int MovementPoints => _movementPoints;
    public int RemainingMovementPoints => _remainingMovementPoints;

    public int AttackDamage => _attackDamage;
    
    public bool IsInitialized => _initialized;
    public bool IsDead => _isDead;

    public virtual void Initialize(Vector3Int initialCell, PerUnitStatsData perUnitStatsData)
    {
        _currentCell = initialCell;
        
        _health = perUnitStatsData.Health;
        _currentHealth = _health;

        _movementPoints = perUnitStatsData.MovePoints;
        _remainingMovementPoints = _movementPoints;

        _attackDamage = perUnitStatsData.AttackDamage;
        
        _initialized = true;
        _isDead = false;
    }
    
    public void SetCurrentCell(Vector3Int newCell)
    {
        _currentCell = newCell;
    }
}
