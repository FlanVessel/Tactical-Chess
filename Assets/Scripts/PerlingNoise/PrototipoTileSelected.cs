using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PrototipoTileSelected : MonoBehaviour
{
    [Header("Configuracion Movimiento Tile")]
    [SerializeField] private TileBase tileBase;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputAction moveAction;
    [SerializeField] private Vector2Int mapSize = new Vector2Int();
    [SerializeField] private Vector2Int mapOrigin = Vector2Int.zero;

    private Tilemap _boardTilemap;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        CreateIsometricGrid();
        MoveSelectedTile();
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

        //Hijo del Grid
        tilemapObject.transform.SetParent(gridObject.transform);

        //Agregamos los componentes Tilemap
        _boardTilemap = tilemapObject.AddComponent<Tilemap>();

        //Agregamos para que renderize el mapa
        TilemapRenderer tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight;
    }

    private void MoveSelectedTile()
    {
        

        moveAction = playerInput.actions["Move"];
    }

    private void CellPosition()
    {

    }
}
