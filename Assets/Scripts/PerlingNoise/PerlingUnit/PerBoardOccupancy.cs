using UnityEngine;
using System.Collections.Generic;
using System.Numerics;

public class PerBoardOccupancy : MonoBehaviour
{
    private Dictionary<Vector3Int, PerUnit> _unitCells = new Dictionary<Vector3Int, PerUnit>();
    
    public PerUnit UnitAt(Vector3Int cell)
    {
        PerUnit perUnit;
        bool perUnitExists = _unitCells.TryGetValue(cell, out perUnit);
        return perUnit;
    }

    public bool RegisterUnit(PerUnit perUnit, Vector3Int cell)
    {
        _unitCells.Add(cell, perUnit);
        return true;
    }
}
