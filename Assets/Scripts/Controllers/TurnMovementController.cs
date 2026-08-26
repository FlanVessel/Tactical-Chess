using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class TurnMovementController : MonoBehaviour
{
    private static readonly Vector3Int[] Directions = {Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down};
    private Tilemap _boardTilemap;
    private Tilemap _highlightTilemap;
    private TileBase _reachableTile;
    private Vector3Int _currentCell;
    private Vector3 _playerOffset;
    private int _movementPoints;
    private float _movementSpeed;
    private bool _selected;
    private bool _moving;
    private readonly HashSet<Vector3Int> reachableCells = new();
    private readonly Dictionary<Vector3Int, Vector3Int> cameFrom = new();

    public void Setup(Tilemap board, Tilemap highlights, TileBase tile, Vector3Int initialCell, Vector3 offSet, int steps, float speed)
    {
        //Reciben informacion
        _boardTilemap = board;
        _highlightTilemap = highlights;
        _reachableTile = tile;

        _currentCell = initialCell;
        _playerOffset = offSet;

        _movementPoints = steps;
        _movementSpeed = speed;

        //Checamos que los pasos no sean negativos y que su velocidad no sea cero o negativo
        _movementPoints = Mathf.Max(0, steps);
        _movementSpeed = Mathf.Max(0.1f, speed);

        //Colocacion exacta del personaje
        transform.position = _boardTilemap.GetCellCenterWorld(_currentCell) + _playerOffset;
    }

    private void Update()
    {
        //Checamos si se esta moviendo
        if (_moving) return;

        //Comprobamos si existe el mouse
        if (Mouse.current == null) return;

        //Detectamos el click izquierdo
        if (Mouse.current.leftButton.wasPressedThisFrame) HandleClick();
    }

    private void HandleClick()
    {
        Camera gameCamera = Camera.main;

        if (gameCamera == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(gameCamera.transform.position.z));

        Vector3 worldPosition = gameCamera.ScreenToWorldPoint(screenPosition);

        Vector3Int clickedCell = _boardTilemap.WorldToCell(worldPosition);

        if (clickedCell == _currentCell)
        {
            if (_selected)
            {
                ClearSelection();
            }
            else
            {
                ShowReachableCells();
            }

            return;
        }

        if (_selected && reachableCells.Contains(clickedCell)) StartCoroutine(MoveTo(clickedCell));
    }

    private void ShowReachableCells()
    {
        //Limpiamos la selecciones anteriores y seleccionamos al personaje
        ClearSelection();

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

                if (nextDistance > _movementPoints) continue;

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
        return false;
    }

    private IEnumerator MoveTo(Vector3Int destination)
    {
        _moving = true;
        _selected = false;

        List<Vector3Int> path = BuildPath(destination);
        _highlightTilemap.ClearAllTiles();
        reachableCells.Clear();

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 targetPosition = _boardTilemap.GetCellCenterWorld(path[i]) + _playerOffset;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
            _currentCell = path[i];
        }

        _moving = false;
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

    private void ClearSelection()
    {
        _selected = false;
        reachableCells.Clear();
        cameFrom.Clear();

        if (_highlightTilemap != null) _highlightTilemap.ClearAllTiles();
    }
}
