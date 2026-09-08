using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitAction", menuName = "Units/Unit Action")]

public class UnitActionData : ScriptableObject
{
    [Header("Informacion")]
    [SerializeField] private string actionName;
    [SerializeField] private Sprite actionIcon;

    [Header("Ataque")] 
    [SerializeField, Min(0)] private int damage = 1;
    [SerializeField, Min(1)] private int attackRange = 1;

    [Header("Costes")]
    [SerializeField, Min(0)] private int moveCost;
    [SerializeField, Min(0)] private int actionPointCost;

    public string ActionName => actionName;
    public Sprite ActionIcon => actionIcon;
    
    public int Damage => damage;
    public int AttackRange => attackRange;

    public int MoveCost => moveCost;
    public int ActionPointCost => actionPointCost;
}
