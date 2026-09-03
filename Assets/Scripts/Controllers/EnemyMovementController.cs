using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class EnemyMovementController : MonoBehaviour
{
    private Tilemap _boardTilemap;
    private BoardOccupancy _boardOccupancy;
    private EnemyUnit _enemyUnit;

    private Vector3 _unitOffset;
    private float _movementSpeed;

    private readonly GridPathfinder _pathfinder = new();

    public void Setup(Tilemap boardTilemap, BoardOccupancy boardOccupancy, Vector3 unitOffset)
    {
        _boardTilemap = boardTilemap;
        _boardOccupancy = boardOccupancy;
        _unitOffset = unitOffset;

        _enemyUnit = GetComponent<EnemyUnit>();

        if (_enemyUnit == null) Debug.LogError($"{name} no tiene EnemyUnit.");

        _movementSpeed = Mathf.Max(0.1f, _enemyUnit.Data.MovementSpeed);
    }

    public IEnumerator ExecuteMovement(IReadOnlyList<PlayerUnit> playerUnits)
    {
        if (_enemyUnit == null) yield break;
        if (!_enemyUnit.CanMove()) yield break;

        PlayerUnit target = FindClosestTarget(playerUnits);

        if (target == null) yield break;

        Dictionary<Vector3Int, Vector3Int> cameFrom = new();

        HashSet<Vector3Int> reachableCells = _pathfinder.FindReachableCells(_boardTilemap, _boardOccupancy, _enemyUnit.CurrentCell, _enemyUnit.Data.MoveRange, cameFrom);

        Vector3Int destination = FindBestDestination(reachableCells, target.CurrentCell);

        if (destination == _enemyUnit.CurrentCell)
        {
            _enemyUnit.UseMovement();
            yield break;
        }

        List<Vector3Int> path = _pathfinder.BuildPath(_enemyUnit.CurrentCell, destination, cameFrom);

        if (path.Count == 0)
        {
            _enemyUnit.UseMovement();
            yield break;
        }

        yield return MoveAlongPath(path);
    }

    private PlayerUnit FindClosestTarget(IReadOnlyList<PlayerUnit> playerUnits)
    {
        PlayerUnit closestTarget = null;
        int closestDistance = int.MaxValue;

        foreach (PlayerUnit playerUnit in playerUnits)
        {
            if (playerUnit == null) continue;
            if (playerUnit.IsDead) continue;

            int distance = CalculateDistance(_enemyUnit.CurrentCell, playerUnit.CurrentCell);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = playerUnit;
            }
        }

        return closestTarget;
    }

    private Vector3Int FindBestDestination(HashSet<Vector3Int> reachableCells, Vector3Int targetCell)
    {
        Vector3Int bestCell = _enemyUnit.CurrentCell;

        int bestDistance = CalculateDistance(bestCell, targetCell);

        foreach (Vector3Int cell in reachableCells)
        {
            int distance = CalculateDistance(cell, targetCell);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCell = cell;
            }
        }

        return bestCell;
    }

    private int CalculateDistance(Vector3Int first, Vector3Int second)
    {
        return Mathf.Abs(first.x - second.x) + Mathf.Abs(first.y - second.y);
    }

    private IEnumerator MoveAlongPath(List<Vector3Int> path)
    {
        if (!_enemyUnit.UseMovement()) yield break;

        _enemyUnit.SetMoving(true);

        for (int i = 1; i < path.Count; i++)
        {
            Vector3Int currentCell = _enemyUnit.CurrentCell;

            Vector3Int nextCell = path[i];

            bool couldMove = _boardOccupancy.TryMoveUnit(_enemyUnit, currentCell, nextCell);

            if (!couldMove)
            {
                Debug.LogWarning($"{name} no pudo avanzar hacia {nextCell}.");
                break;
            }

            Vector3 targetPosition = _boardTilemap.GetCellCenterWorld(nextCell) + _unitOffset;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
            _enemyUnit.SetCurrentCell(nextCell);
        }

        _enemyUnit.SetMoving(false);
    }
}
