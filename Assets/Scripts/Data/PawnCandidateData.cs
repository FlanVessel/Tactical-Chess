using UnityEngine;
using System;

[Serializable]
public class PawnCandidateData
{
    [SerializeField] private UnitData unitData;
    [SerializeField] private Sprite pawnSprite;
    
    [SerializeField] private string pawnName;

    [SerializeField] private int maxHealth;
    [SerializeField] private int moveRange;
    [SerializeField] private int maxMana;
    [SerializeField] private int manaRecovery;

    public UnitData UnitData => unitData;
    public Sprite PawnSprite => pawnSprite;
    public string PawnName => pawnName;
    public int MaxHealth => maxHealth;
    public int MoveRange => moveRange;
    public int MaxMana => maxMana;
    public int ManaRecovery => manaRecovery;

    public PawnCandidateData(UnitData data, Sprite generateSprite, string generateName, int health, int movement, int mana, int recovery)
    {
        unitData = data;
        pawnSprite = generateSprite;
        pawnName = generateName;
        
        maxHealth = Mathf.Max(1, health);
        moveRange = Mathf.Max(0, movement);
        maxMana = Mathf.Max(0, mana);
        manaRecovery = Mathf.Clamp(recovery, 0, maxMana);
    }
}
