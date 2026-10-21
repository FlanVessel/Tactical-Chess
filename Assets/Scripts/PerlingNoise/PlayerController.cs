using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Referencia del Tile")] 
    [SerializeField] private TileBase _tileBase;
    
    private PerlingMapController _mapController;
    
    private Vector3Int _cellPosition;
    
    private PlayerInput _playerInput;
    private InputAction _inputAction;
    
    private Tilemap _tilemap;
    private bool _config;

    public void Config(PerlingMapController perlingMapController, Tilemap tilemap, Vector3Int cellPosition)
    {
        _mapController = perlingMapController;
        _tilemap = tilemap;
        _cellPosition = cellPosition;
        _config = true;
        
        ShowSelection();
    }

    private void SelectedMove()
    {
        
    }

    private void TryOtherSelection()
    {
        
    }
    
    private void ShowSelection()
    {
        if (!_config) return;
        
        _tilemap.ClearAllTiles();
        _tilemap.SetTile(_cellPosition, _tileBase);
    }

    private void ClearSelection()
    {
        
    }
}
