using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PawnNameData", menuName = "Units/PawnNameData")]
public class PawnNameData : ScriptableObject
{
    [SerializeField] private List<string> _pawnNames  = new();
    
    public IReadOnlyList<string> PawnNames => _pawnNames;
}
