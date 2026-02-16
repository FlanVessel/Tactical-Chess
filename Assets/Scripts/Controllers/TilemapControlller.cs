using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapControlller : MonoBehaviour
{
    [Header("Configuracion de Mapa")]
    [SerializeField]public List<Vector2Int> mapSize;
    [SerializeField]public List<Vector2Int> mapOrigin;
    [SerializeField]public List<Tile> tiles;

    private void Start()
    {
        // Creamos objeto vacio
        GameObject grid = new GameObject();
        grid.name = "Grid"; //lo llamamos Grid

        // Le agregamos un objeto de la clase Grid
        grid.AddComponent<Grid>();

        //Obtenemod el componente de grid
        Grid isometricGrid = grid.GetComponent<Grid>();

        //llamamos el parametro de cellLayout y cellSize
        isometricGrid.cellLayout = GridLayout.CellLayout.Isometric;
        isometricGrid.cellSize = new Vector3(1, 0.5f, 1);

        // Creamos el hijo
        GameObject tilemap = new GameObject();
        tilemap.name = "Tilemap"; //lo llamamos al hijo Tilemap

        // Le agregamos sus objetos al gameobject tilemap y tilemapRenderer
        tilemap.AddComponent<Tilemap>();
        tilemap.AddComponent<TilemapRenderer>();

        // Le agregamos las propiedades al tilemapRenderer para modificar el orden de dibujo en SortOrder a TopRight
        TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();                      // Accedemos al componente TilemapRenderer
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight;                                 // Modificamos la propiedad sortOrder

        // Asignarle a tilemap el padre que es grid
        tilemap.transform.parent = grid.transform;

        Tilemap map = tilemap.GetComponent<Tilemap>();

        GenerateLine(map);
        GenerateRectangle(map);
        GenerateCircle(map);
        GenerateTriangle(map);

    }

    private void GenerateRectangle(Tilemap tilemap)
    {
        Map map = new Map(mapOrigin[1], mapSize[1], tilemap, MapType.Rectangle);
        List<Vector3Int> coordinates = map.generateCoordinates();

        map.Render(coordinates, tiles[1], tilemap);
    }

    private void GenerateTriangle(Tilemap tilemap)
    {
        Map map = new Map(mapOrigin[3], mapSize[3], tilemap, MapType.Triangle);
        List<Vector3Int> coordinates = map.generateCoordinates();

        map.Render(coordinates, tiles[3], tilemap);
    }

    private void GenerateLine(Tilemap tilemap)
    {
        Map map = new Map(mapOrigin[0], mapSize[0], tilemap, MapType.Line);
        List<Vector3Int> coordinates = map.generateCoordinates();

        map.Render(coordinates, tiles[0], tilemap);
    }

    private void GenerateCircle(Tilemap tilemap)
    {
        Map map = new Map(mapOrigin[2], mapSize[2], tilemap, MapType.Circle);
        List<Vector3Int> coordinates = map.generateCoordinates();

        map.Render(coordinates, tiles[2], tilemap);
    }
}
