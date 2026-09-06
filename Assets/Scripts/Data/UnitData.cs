using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/Unit Data")]

public class UnitData : ScriptableObject
{
    [Header("Identidad")]
    [SerializeField] private string unitName;
    [SerializeField] private Sprite unitSprite;

    [Header("Estadisticas")]
    [SerializeField, Min(1)] private int minHealth = 3;
    [SerializeField, Min(1)] private int maxHealth = 8;

    [Header("Movimiento")]
    [SerializeField, Min(0)] private int minMoveRange = 1;
    [SerializeField, Min(0)] private int maxMoveRange = 5;
    [SerializeField, Min(0.1f)] private float movementSpeed = 3f;

    [Header("Acciones")]
    [SerializeField] private List<UnitActionData> availableActions = new List<UnitActionData>();

    [Header("Emocion")]
    [SerializeField, Range(0, 100)] private int startingDesperation = 50;

    public string UnitName => unitName;
    public Sprite UnitSprite => unitSprite;

    public int MinHealth => minHealth;
    public int MaxHealth => maxHealth;
    
    public int MinMoveRange => minMoveRange;
    public int MaxMoveRange => maxMoveRange;
    
    public float MovementSpeed => movementSpeed;
    
    public int StartingDesperation => startingDesperation;

    public IReadOnlyList<UnitActionData> AvailableActions => availableActions;
    
    public int GenerateMaxHealth()
    {
        return Random.Range(minHealth, maxHealth + 1);
    }

    public int GenerateMoveRange()
    {
        return Random.Range(minMoveRange, MaxMoveRange + 1);
    }

    private void OnValidate()
    {
        minHealth = Mathf.Max(1, minHealth);
        maxHealth = Mathf.Max(minHealth, maxHealth);
        
        minMoveRange = Mathf.Max(0, minMoveRange);
        maxMoveRange = Mathf.Max(minMoveRange, maxMoveRange);
        
        movementSpeed = Mathf.Max(0.1f, movementSpeed);
    }
}
