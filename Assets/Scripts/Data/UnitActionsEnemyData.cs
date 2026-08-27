using UnityEngine;

public class UnitActionsEnemyData : ScriptableObject
{
    [Header("Informacion")]
    [SerializeField] private string actionEnemyName;
    [SerializeField] private Sprite actionEnemySprite;

    [Header("Costes")]
    [SerializeField, Min(0)] private int moveEnemyCost; 
    [SerializeField, MinAttribute(0)] private int actionPointCost;

    public string ActionEnemyName => actionEnemyName;
    public Sprite ActionEnemySprite => actionEnemySprite;
    public int MoveEnemyCost => moveEnemyCost;
    public int ActionPointCost => actionPointCost;
}
