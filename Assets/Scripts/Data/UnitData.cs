using System.Collections.Generic;
using UnityEngine;

public class UnitData : ScriptableObject
{
    [Header("Identidad")]
    [SerializeField] private string unitName;
    [SerializeField] private Sprite unitSprite;

    [Header("Estadisticas")]
    [SerializeField, Min(1)] private int maxHealth = 10;
    [SerializeField, Min(0)] private int movePoints = 3;

    [Header("Acciones")]
    [SerializeField] private List<UnitActionData> availableActions;

    public string UnitName => unitName;
    public Sprite UnitSprite => unitSprite;
    public int MaxHealth => maxHealth;
    public int MovePoitns => movePoints;

    public IReadOnlyList<UnitActionData> AvailableActions => availableActions;
}
