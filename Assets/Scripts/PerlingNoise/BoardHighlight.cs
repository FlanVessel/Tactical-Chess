using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Numerics;

public class BoardHighlight : MonoBehaviour
{
    [Header("Tile para movimiento")] 
    [SerializeField] private TileBase movementTile;
    
    private PerlingMapController _mapController;
    private PerBoardOccupancy _boardOccupancy;
    private Tilemap _highlightedTilemap;
    
    private List<Vector3Int> _searchCells = new List<Vector3Int>();

    public void ConfigBoardHighlight(PerlingMapController mapController, PerBoardOccupancy boardOccupancy, Tilemap highlightedTilemap)
    {
        _mapController = mapController;
        _boardOccupancy = boardOccupancy;
        _highlightedTilemap = highlightedTilemap;
    }

    public void ClearMovementCells()
    {
        _searchCells.Clear();
        _highlightedTilemap.ClearAllTiles();
    }
    
}
