using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlingMapController : MonoBehaviour
{
    [Header("Tamaño de mapa")]
    [SerializeField]private Vector2Int mapSize = new Vector2Int();
    [SerializeField]private Vector2Int mapOrigin = Vector2Int.zero;
    
    [Header("Tipo de Generacion de Mapa")]
    [SerializeField]private TileBase[] boardTile;
    [SerializeField]private bool isSingleTile = false;
    [SerializeField]private TileBase singleTile;

    private Tilemap _boardTilemap;
    public Tilemap BoardTilemap => _boardTilemap;
    
    private Tilemap _selectionTilemap;
    public Tilemap SelectionTilemap => _selectionTilemap;

    private Tilemap _highlightTilemap;
    public Tilemap HightlightTilemap => _highlightTilemap;
    
    Dictionary<Vector3Int, TerrainType> _perlin;

    public void GenerateMap()
    {
        CreateIsometricGrid();
        CheckMap();
    }
    
    private void CheckMap()
    {
        if (isSingleTile)
        {
            GenerateBoard();
        }
        else
        {
            GeneratePerlin();
        }
        
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


        //Creamos el objeto BoardTilemap
        GameObject tilemapObject = new GameObject("BoardTilemap");

        //Hijo del Grid
        tilemapObject.transform.SetParent(gridObject.transform);

        //Agregamos los componentes Tilemap
        _boardTilemap = tilemapObject.AddComponent<Tilemap>();

        //Agregamos para que renderize el mapa
        TilemapRenderer tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight;

        
        //Lo mismo de arriba pero solo randerizara el tileselect del PlayerController
        GameObject selectionObject = new GameObject("SelectionTilemap");
        selectionObject.transform.SetParent(gridObject.transform);
        
        _selectionTilemap = selectionObject.AddComponent<Tilemap>();
        
        TilemapRenderer selectionRenderer = selectionObject.AddComponent<TilemapRenderer>();
        selectionRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight;


        //Lo mismo pero estamos creando un hightlightTilemap para los tiles que se iluminaran
        GameObject highlightObject = new GameObject("HighLightTilemap");
        highlightObject.transform.SetParent(gridObject.transform);

        _highlightTilemap = highlightObject.AddComponent<Tilemap>();

        TilemapRenderer highlightRenderer = highlightObject.AddComponent<TilemapRenderer>();
        highlightRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight;

    }
    
    private void GenerateBoard()
    {
        MapPerlingData map = new MapPerlingData(mapOrigin, mapSize);

        List<Vector3Int> coordinates = map.GenerateCoordinates();

        foreach (Vector3Int coordinate in coordinates)
        {
            _boardTilemap.SetTile(coordinate, singleTile);
        }
    }

    private void GeneratePerlin()
    {
        MapPerlingData map = new MapPerlingData(mapOrigin, mapSize);

        _perlin = map.GenerateTerrain();

        foreach (KeyValuePair<Vector3Int, TerrainType> pair in _perlin)
        {
            Vector3Int coordinate = pair.Key;
            TerrainType perlinType = pair.Value;

            TileBase selectedTile = ArrayTileBase(perlinType);
            
            _boardTilemap.SetTile(coordinate, selectedTile);
        }
    }

    private TileBase ArrayTileBase(TerrainType perlinType)
    {
        int indexTerrainType = (int)perlinType;
        return boardTile[indexTerrainType];
    }

    public bool CellExists(Vector3Int cell)
    {
        return _perlin.ContainsKey(cell);
    }
    
    public bool IsSpawnable(Vector3Int cell)
    {
        return _perlin[cell] == TerrainType.Tierra;
    }
    
    public bool IsWalking(Vector3Int cell)
    {
        return _perlin[cell] != TerrainType.Obstaculo;
    } 
    
}
