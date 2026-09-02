using System.Collections.Generic;
using UnityEngine;

public class BoardOccupancy : MonoBehaviour
{
    private readonly Dictionary<Vector3Int, Unit> _unitsByCell = new();

    public bool IsOccupied(Vector3Int cell)
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

        if (IsOccupied(cell))
        {
            Debug.LogWarning($"La casilla {cell} ya está ocupada.");
            return false;
        }

        _unitsByCell.Add(cell, unit);
        return true;
    }

    public bool TryMoveUnit(
        Unit unit,
        Vector3Int origin,
        Vector3Int destination)
    {
        if (unit == null) return false;

        if (!_unitsByCell.TryGetValue(origin, out Unit registeredUnit))
        {
            Debug.LogWarning(
                $"{unit.name} no está registrado en la casilla {origin}."
            );

            return false;
        }

        if (registeredUnit != unit)
        {
            Debug.LogWarning(
                $"La casilla {origin} pertenece a otra unidad."
            );

            return false;
        }

        if (IsOccupied(destination))
        {
            return false;
        }

        _unitsByCell.Remove(origin);
        _unitsByCell.Add(destination, unit);

        return true;
    }

    public void RemoveUnit(Unit unit)
    {
        if (unit == null) return;

        if (_unitsByCell.TryGetValue(unit.CurrentCell, out Unit registeredUnit)
            && registeredUnit == unit)
        {
            _unitsByCell.Remove(unit.CurrentCell);
        }
    }
}
