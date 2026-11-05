using UnityEngine;

[CreateAssetMenu(fileName = "NewPerUnitStatsData", menuName = "PerlingNoise/PerUnitStatsData")]
public class PerUnitStatsData : ScriptableObject
{
    [Header("Estadisticas")] 
    [SerializeField] private int health;
    [SerializeField] private int movePoints;
    [SerializeField] private int attackDamage;
    
    public int Health => health;
    public int MovePoints => movePoints;
    public int AttackDamage => attackDamage;

}
