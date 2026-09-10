using UnityEngine;
public enum UnitActionType {IncreaseDamage, Heal, Damage}

[CreateAssetMenu(fileName = "NewUnitAction", menuName = "Units/Unit Action")]


public class UnitActionData : ScriptableObject
{
    [Header("Informacion")]
    [SerializeField] private string actionName;
    [SerializeField] private Sprite actionIcon;

    [Header("Ataque")] 
    [SerializeField] private UnitActionType actionType;
    [SerializeField, Min(0)] private int effectAmount = 1;
    [SerializeField, Min(1)] private int attackRange = 1;



    public string ActionName => actionName;
    public Sprite ActionIcon => actionIcon;
    
    public UnitActionType ActionType => actionType;
    public int EffectAmount => effectAmount;
    public int AttackRange => attackRange;
    
    public int Damage => effectAmount;
}
