using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class UnitCombatController : MonoBehaviour
{
    private static readonly Vector3Int[] DiagonalDirections = {new Vector3Int(1, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0)};

    private Unit _unit;
    private Tilemap _boardTilemap;
    private BoardOccupancy _boardOccupancy;

    public void SetUp(Tilemap boardTilemap, BoardOccupancy boardOccupancy)
    {
        _unit = GetComponent<Unit>();
        _boardTilemap = boardTilemap;
        _boardOccupancy = boardOccupancy;
        
        if (_unit == null) Debug.LogError($"{name} no tiene un componente Unit.");
    }

    public HashSet<Vector3Int> GetAttackCells(UnitActionData actionData)
    {
        HashSet<Vector3Int> cells = new();
        
        if (_unit == null || actionData == null || _boardTilemap == null) return cells;

        foreach (Vector3Int direction in DiagonalDirections)
        {
            for (int distance = 1; distance <= actionData.AttackRange; distance++)
            {
                Vector3Int cell = _unit.CurrentCell + direction * distance;

                if (!_boardTilemap.HasTile(cell)) break;
                cells.Add(cell);
            }
        }
        return cells;
    }

    public bool TryAttack(Vector3Int targetCell, UnitActionData actionData)
    {
        if (_unit == null || actionData == null || _boardTilemap == null) return false;
        if (!_unit.CanAct()) return false;
        
        HashSet<Vector3Int> validCells = GetAttackCells(actionData);
        
        if (!validCells.Contains(targetCell)) return false;
        
        Unit target = _boardOccupancy.GetUnitAt(targetCell);
        
        if (target == null || target.IsDead) return false;
        if (!IsEnemy(target)) return false;
        if (!_unit.UseAction()) return false;
        
        target.TakeDamage(actionData.Damage);
        
        Debug.Log($"{_unit.name} usa {actionData.ActionName} contra " + $"{target.name} y causa {actionData.Damage} da dano.");
        return true;
    }

    private bool IsEnemy(Unit target)
    {
        bool attackIsPlayer = _unit is PlayerUnit;
        bool targetIsPlayer = target is PlayerUnit;
        
        return attackIsPlayer != targetIsPlayer;
    }
}
