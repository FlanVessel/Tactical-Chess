using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnMovementController : MonoBehaviour
{
    private static readonly Vector3Int[] Directions = {Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down};
    private Tilemap _boardTilemap;
    private Tilemap _highlightTilemap;
    private TileBase _reachableTile;
    private Vector3Int _currentCell;
    private Vector3 _playerOffset;
    private float _movementSpeed;
    private bool _selected;
    private bool _moving;
    private readonly HashSet<Vector3Int> reachableCells = new();
    private readonly Dictionary<Vector3Int, Vector3Int> cameFrom = new();
    private Unit _unit;
    private BoardOccupancy _boardOccupancy;

    public void Setup(Tilemap board, Tilemap highlights, TileBase tile, Vector3Int initialCell, Vector3 offSet, BoardOccupancy boardOccupancy)
    {
        //Reciben informacion
        _boardTilemap = board;
        _highlightTilemap = highlights;
        _reachableTile = tile;

        _currentCell = initialCell;
        _playerOffset = offSet;

        _boardOccupancy = boardOccupancy;

        _unit = GetComponent<Unit>();

        if (_unit == null)
        {
            Debug.LogError($"{name} no contiene un componente Unit.");
            return;
        }

        //Checamos que su velocidad no sea cero o negativo
        _movementSpeed = Mathf.Max(0.1f, _unit.Data.MovementSpeed);

        //Colocacion exacta del personaje
        transform.position = _boardTilemap.GetCellCenterWorld(_currentCell) + _playerOffset;
    }

    public void ShowReachableCells()
    {
        //Limpiamos la selecciones anteriores y seleccionamos al personaje
        ClearSelection();

        if (_unit == null) return;
        if (!_unit.IsActive) return;
        if (_unit.IsMoving) return;
        if (!_unit.CanMove()) return;

        _selected = true;

        //Haremos una fila
        Queue<Vector3Int> pendingCells = new Queue<Vector3Int>();

        //Hacemos un diccionario para relacionar las casillas como los pasos que tiene que dar
        Dictionary<Vector3Int, int> distances = new Dictionary<Vector3Int, int>();

        pendingCells.Enqueue(_currentCell);
        distances[_currentCell] = 0;

        //Mientras este una casilla pendiente, continua buscando hasta terminar.
        while (pendingCells.Count > 0)
        {
            Vector3Int current = pendingCells.Dequeue();  //Entra y sale de la fila
            int currentDistance = distances[current];     //Cuantos pasos costo llegar

            foreach (Vector3Int direction in Directions)
            {
                Vector3Int next = current + direction;
                int nextDistance = currentDistance + 1;

                if (nextDistance > _unit.Data.MoveRange) continue;

                if (!_boardTilemap.HasTile(next)) continue;

                if (distances.ContainsKey(next)) continue;

                if (IsBlocked(next)) continue;

                distances[next] = nextDistance;
                cameFrom[next] = current;

                reachableCells.Add(next);
                pendingCells.Enqueue(next);
            }
        }

        foreach (Vector3Int cell in reachableCells) _highlightTilemap.SetTile(cell, _reachableTile);
    }

    private bool IsBlocked(Vector3Int cell)
    {
        if (_boardOccupancy == null) return false;
        return _boardOccupancy.IsOccupied(cell);
    }

    private IEnumerator MoveTo(List<Vector3Int> path)
    {
        if (!_unit.UseMovement())
        {
            Debug.Log($"{name} ya utilizo su movimiento.");
            yield break;
        }

        _moving = true;
        _selected = false;
        _unit.SetMoving(true);

        _highlightTilemap.ClearAllTiles();
        reachableCells.Clear();
        cameFrom.Clear();

        for (int i = 1; i < path.Count; i++)
        {
            Vector3Int nextCell = path[i];

            bool couldMove = _boardOccupancy.TryMoveUnit(_unit, _currentCell, nextCell);

            if (!couldMove)
            {
                Debug.LogWarning($"{name} no pudo avanzar hacia {nextCell}.");

                break;
            }

            Vector3 targetPosition = _boardTilemap.GetCellCenterWorld(nextCell) + _playerOffset;

            while (Vector3.Distance(transform.position,targetPosition) > 0.01f)
            {

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
                yield return null;

            }

            transform.position = targetPosition;
            _currentCell = nextCell;

            if (_unit != null)
            {
                _unit.SetCurrentCell(_currentCell);
            }
        }

        _moving = false;
        _unit.SetMoving(false);
        Debug.Log($"{name} termino su accion de movimiento.");
    }

    private List<Vector3Int>BuildPath(Vector3Int destination)
    {
        List<Vector3Int> path = new();
        Vector3Int cell = destination;

        path.Add(cell);

        while (cell != _currentCell)
        {
            cell = cameFrom[cell];
            path.Add(cell);
        }

        path.Reverse();
        return path;
    }

    public void ClearSelection()
    {
        _selected = false;
        reachableCells.Clear();
        cameFrom.Clear();

        if (_highlightTilemap != null) _highlightTilemap.ClearAllTiles();
    }

    public bool TryMoveTo(Vector3Int destination)
    {
        if (_moving) return false;
        if (!_selected) return false;
        if (_unit == null) return false;
        if (!reachableCells.Contains(destination)) return false;

        List<Vector3Int> path = BuildPath(destination);

        StartCoroutine(MoveTo(path));
        return true;
    }
}
