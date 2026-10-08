using UnityEngine;

public static class PawnCandidateGenerator
{
    public static PawnCandidateData Generate(UnitData unitDataBase, Sprite pawnSprite, string pawnName)
    {
        if (unitDataBase == null) return null;
        
        int health = unitDataBase.GenerateMaxHealth();
        int movement = unitDataBase.GenerateMoveRange();
        int mana = unitDataBase.GenerateMana();

        int recovery = Mathf.Min(unitDataBase.GenerateManaRecovery(), mana);

        return new PawnCandidateData(unitDataBase, pawnSprite, pawnName, health, movement, mana, recovery);
    }
}
