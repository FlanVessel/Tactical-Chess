using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapControlller : MonoBehaviour
{
    [Header("Configuracion de Mapa")]
    [SerializeField]private Vector2Int mapSize = new Vector2Int(12, 12);
    [SerializeField]private Vector2Int mapOrigin = Vector2Int.zero;
    [SerializeField]private TileBase boardTile;

    [Header("Configuracion del Jugador")]
    [SerializeField]private GameObject playerPrefab;
    [SerializeField]private Vector2Int playerSpawnCell = new Vector2Int(3, 5);
    [SerializeField]private Vector3 playerOffset = new Vector3(0f, 0.25f, 0f);

    [Header("Configuracion de Movimiento")]
    [SerializeField]private TileBase reachableTile;
    [SerializeField, Min(0)]private int playerMovementPoints = 6;
    [SerializeField, Min(0.1f)]private float playerMovementSpeed = 3f;

    private Tilemap _hightlighTilemap;

    private Tilemap _boardTilemap;


    private void Start()
    {
        CreateIsometricGrid();
        GenerateBoard();
        SpawnPlayer(playerSpawnCell);
    }

    private void CreateIsometricGrid()
    {
        //Creamos el Grid
        GameObject gridObject = new GameObject("Grid");
        //Le agregamos el componente Grid
        Grid grid = gridObject.AddComponent<Grid>();

        //Configuramos el Grid como Isometrico
        grid.cellLayout = GridLayout.CellLayout.Isometric;

        grid.cellSize = new Vector3(1f, 0.5f, 1f);

        //Creamos el objeto Tilemap
        GameObject tilemapObject = new GameObject("BoardTilemap");

        tilemapObject.transform.SetParent(gridObject.transform);

        //Agregamos los componentes
        _boardTilemap = tilemapObject.AddComponent<Tilemap>();

        TilemapRenderer tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight;

        _hightlighTilemap = CreateHighlightTilemap(gridObject);
    }

    private void GenerateBoard()
    {
        if (boardTile == null)
        {
            Debug.LogError("No se asigno al tile del tablero.");
            return;
        }

        Map map = new Map(mapOrigin, mapSize);

        List<Vector3Int> coordinates = map.GenerateCoordinates();

        foreach (Vector3Int coordinate in coordinates)
        {
            _boardTilemap.SetTile(coordinate, boardTile);
        }
    }

    private void SpawnPlayer(Vector2Int cell)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("No se asigno el prefap del jugador.");
            return;
        }

        Vector3Int cellPosition = new Vector3Int(cell.x, cell.y, 0);

        //Revisamos que exista una casilla
        if (!_boardTilemap.HasTile(cellPosition))
        {
            Debug.LogError($"No existe una casilla en {cellPosition}.");
            return;
        }

        //Convertimos la casilla en una posicion de la escena
        Vector3 worldPosition = _boardTilemap.GetCellCenterWorld(cellPosition);

        GameObject player = Instantiate(playerPrefab, worldPosition + playerOffset,Quaternion.identity);

        player.name = $"Player_Cell_{cell.x}_{cell.y}";

        TurnMovementController movementController = player.GetComponent<TurnMovementController>();

        if (movementController == null)
        {
            Debug.LogError("El prefap no tiene TurnMovementController.");
            return;
        }

        movementController.Setup(_boardTilemap, _hightlighTilemap, reachableTile, cellPosition, playerOffset, playerMovementPoints, playerMovementSpeed);
    }

    private Tilemap CreateHighlightTilemap(GameObject gridObject)
    {
        GameObject highLightObject = new GameObject("MovementHighlights");
        highLightObject.transform.SetParent(gridObject.transform);

        Tilemap tilemap = highLightObject.AddComponent<Tilemap>();
        TilemapRenderer renderer = highLightObject.AddComponent<TilemapRenderer>();

        renderer.sortOrder = TilemapRenderer.SortOrder.TopRight;
        renderer.sortingOrder = 10;

        return tilemap;
    }

}
