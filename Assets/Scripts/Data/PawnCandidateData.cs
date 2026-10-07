using UnityEngine;
using System;

[Serializable]
public class PawnCandidateData
{
    [SerializeField] private UnitData unitData;
    
    [SerializeField] private string pawnName;

    [SerializeField] private int maxHealth;
    [SerializeField] private int moveRange;
    [SerializeField] private int maxMana;
    [SerializeField] private int manaRecovery;

    public UnitData UnitData => unitData;
    public string PawnName => pawnName;
    public Sprite PawnSprite => unitData != null ? unitData.UnitSprite : null;
    public int MaxHealth => maxHealth;
    public int MoveRange => moveRange;
    public int MaxMana => maxMana;
    public int ManaRecovery => manaRecovery;

    public PawnCandidateData(UnitData data, string generateName, int health, int movement, int mana, int recovery)
    {
        unitData = data;
        pawnName = generateName;
        
        maxHealth = Mathf.Max(1, health);
        moveRange = Mathf.Max(0, movement);
        maxMana = Mathf.Max(0, mana);
        manaRecovery = Mathf.Clamp(recovery, 0, maxMana);
    }
}
