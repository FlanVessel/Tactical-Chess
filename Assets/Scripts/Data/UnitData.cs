using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/Unit Data")]

public class UnitData : ScriptableObject
{
    [Header("Identidad")]
    [SerializeField] private string unitName;
    [SerializeField] private Sprite unitSprite;

    [Header("Estadisticas")]
    [SerializeField, Min(1)] private int maxHealth = 10;
    [SerializeField, Min(0)] private int movePoints = 3;

    [Header("Acciones")]
    [SerializeField] private List<UnitActionData> availableActions = new List<UnitActionData>();

    [Header("Emocion")]
    [SerializeField, Range(0, 100)] private int startingDesperation = 50;

    public string UnitName => unitName;
    public Sprite UnitSprite => unitSprite;
    public int MaxHealth => maxHealth;
    public int MovePoints => movePoints;
    public int StartingDesperation => startingDesperation;

    public IReadOnlyList<UnitActionData> AvailableActions => availableActions;
}
