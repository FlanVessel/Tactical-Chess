using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapControlller : MonoBehaviour
{
    [Header("Configuracion de Mapa")]
    [SerializeField]private Vector2Int mapSize = new Vector2Int(12, 12);
    [SerializeField]private Vector2Int mapOrigin = Vector2Int.zero;
    [SerializeField]private TileBase boardTile;

    [Header("Configuracion de unidades")]
    [SerializeField] private List<UnitSpawnConfig> playerSpawns = new List<UnitSpawnConfig>();
    [SerializeField] private List<UnitSpawnConfig> enemySpawns = new List<UnitSpawnConfig>();
    [SerializeField] private Vector3 unitOffset = Vector3.zero;

    [Header("Configuracion de Movimiento")]
    [SerializeField]private TileBase reachableTile;
    //[SerializeField, Min(0)]private int playerMovementPoints = 6;
    [SerializeField, Min(0.1f)]private float playerMovementSpeed = 3f;



    private Tilemap _highlighTilemap;
    private Tilemap _boardTilemap;

    private PlayerTacticalController _playerTacticalController;
    private BoardOccupancy _boardOccupancy;


    private void Start()
    {
        CreateIsometricGrid();
        GenerateBoard();

        SetupCamera();
        SetUpBoardOccupancy();
        SetupPlayerTacticalController();

        SpawnUnits(playerSpawns, true);
        SpawnUnits(enemySpawns, false);
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

        //Creamos el HighLightTilemap para ver las areas visibles donde se puede mover
        _highlighTilemap = CreateHighlightTilemap(gridObject);
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

    private void SpawnUnits(List<UnitSpawnConfig> spawnConfigs, bool beginActive)
    {
        foreach (UnitSpawnConfig config in spawnConfigs)
        {
            SpawnUnit(config, beginActive);
        }
    }

    private void SpawnUnit(UnitSpawnConfig config, bool beginActive)
    {
        if (config == null)
        {
            Debug.LogError("Existe una configuracion de aparicion vacia.");
            return;
        }

        if (config.Prefap == null)
        {
            Debug.LogError("Unidad no tiene prefab asignado.");
            return;
        }

        Vector2Int configCell = config.SpawnCell;

        Vector3Int cellPosition = new Vector3Int(configCell.x, configCell.y, 0);

        //Revisamos que exista una casilla
        if (!_boardTilemap.HasTile(cellPosition))
        {
            Debug.LogError($"No existe una casilla en {cellPosition}.");
            return;
        }

        //Convertimos la casilla en una posicion de la escena
        Vector3 worldPosition = _boardTilemap.GetCellCenterWorld(cellPosition);

        /*if (_boardOccupancy.IsOccupied(cellPosition))
        {
            Debug.LogError($"No se puede generar una unidad: la casilla {cellPosition} esta ocupada");
            continue;
        }*/

        GameObject unitObject = Instantiate(config.Prefap, worldPosition + unitOffset, Quaternion.identity);

        Unit unit = unitObject.GetComponent<Unit>();

        if (unit == null)
        {
            Destroy(unitObject);
            return;
        }

        unitObject.name = $"{config.Prefap.name}_Cell_" + $"{configCell.x}_{configCell.y}";

        unit.Initialize(cellPosition);

        if (!_boardOccupancy.RegisterUnit(unit, cellPosition))
        {
            Debug.LogError($"No se pudo registrar {unit.name} en {cellPosition}.");
            Destroy(unitObject);
            return;
        }

        if (beginActive) unit.BeginTurn();

        if (unit is PlayerUnit playerUnit)
        {
            ConfigureMovement(unitObject, unit, cellPosition);
            _playerTacticalController.RegisterUnit(playerUnit);
        }
    }

    private void SetupPlayerTacticalController()
    {
        _playerTacticalController = GetComponent<PlayerTacticalController>();

        if (_playerTacticalController == null) _playerTacticalController = gameObject.AddComponent<PlayerTacticalController>();

        _playerTacticalController.Setup(_boardTilemap, Camera.main);
    }

    private void ConfigureMovement(GameObject unitObject, Unit unit, Vector3Int cellPosition)
    {
        TurnMovementController movementController = unitObject.GetComponent<TurnMovementController>();

        if (movementController == null)
        {
            Debug.LogError($"{unitObject.name} no tiene " + "TurnMovementController.");
            return;
        }

        movementController.Setup(_boardTilemap, _highlighTilemap, reachableTile, cellPosition, unitOffset, playerMovementSpeed, _boardOccupancy);
    }

    private Tilemap CreateHighlightTilemap(GameObject gridObject)
    {
        //Creamos MovementHiglights y lo hacemos hijo
        GameObject highLightObject = new GameObject("MovementHighlights");
        highLightObject.transform.SetParent(gridObject.transform);

        //Le agregamos los componentes de Tilemap y TilemapRenderer
        Tilemap tilemap = highLightObject.AddComponent<Tilemap>();
        TilemapRenderer renderer = highLightObject.AddComponent<TilemapRenderer>();

        //Lo renderiza
        renderer.sortOrder = TilemapRenderer.SortOrder.TopRight;
        renderer.sortingOrder = 10;

        return tilemap;
    }

    private void SetupCamera()
    {
        //Buscamos a la CamaraMain
        Camera mainCamera = Camera.main;

        //Si no existe o es nula, resalta comentario
        if (mainCamera == null)
        {
            Debug.LogError("No existe una cámara con el tag MainCamera.");
            return;
        }

        //Buscamos el Script CameraController en la Camara
        CameraController cameraController = mainCamera.GetComponent<CameraController>();

        //Si no existe o es nula, reslta comentario
        if (cameraController == null)
        {
            Debug.LogError("Main Camera no tiene CameraController.");
            return;
        }

        //Llamamos al metodo Setup con el valor de Tilemap
        cameraController.Setup(_boardTilemap);
    }

    private void SetUpBoardOccupancy()
    {
        _boardOccupancy = GetComponent<BoardOccupancy>();

        if (_boardOccupancy == null) _boardOccupancy = gameObject.AddComponent<BoardOccupancy>();
    }

}
