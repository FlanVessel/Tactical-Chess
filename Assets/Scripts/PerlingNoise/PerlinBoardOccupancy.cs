using UnityEngine;
using System.Collections.Generic;

public class PerlinBoardOccupancy : MonoBehaviour
{
    Dictionary<Vector3Int, Unit> _unitsByCell;
    
    private bool IsOccupied(Vector3Int cell)
    {
        return _unitsByCell.ContainsKey(cell);
    }
    
    public Unit GetUnitAt(Vector3Int cell)
    {
        _unitsByCell.TryGetValue(cell, out Unit unit);
        return unit;
    }
    
    public bool RegisterUnit(Unit unit, Vector3Int cell)
    {
        if (unit == null) return false;
        
        if (_unitsByCell.ContainsValue(unit)) return false;
        if (IsOccupied(cell)) return false;
        
        _unitsByCell.Add(cell, unit);
        return true;
    }

    public bool CanMoveUnit(Unit unit, Vector3Int origin, Vector3Int destination)
    {
        if (unit == null) return false;
        if (origin == destination) return false;
        if (!_unitsByCell.TryGetValue(origin, out Unit registeredUnit)) return false;
        if (registeredUnit != unit) return false;
        if (IsOccupied(destination)) return false;
        
        return true;
    }
    
    public bool TryMoveUnit(Unit unit, Vector3Int origin, Vector3Int destination)
    {
        if (!CanMoveUnit(unit, origin, destination)) return false;
        
        _unitsByCell.Remove(origin);
        _unitsByCell.Add(destination, unit);

        return true;
    }
    
    public void RemoveUnit(Unit unit)
    {
        if (unit == null) return;
        if (_unitsByCell.TryGetValue(unit.CurrentCell, out Unit registeredUnit) && registeredUnit == unit) _unitsByCell.Remove(unit.CurrentCell);
    }
    
}
