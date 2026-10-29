using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("TileBase de Selected")] 
    [SerializeField] private TileBase _selectedTileBase;
    
    private PerlingMapController _mapController;
    
    private Vector3Int _cellPosition;
    
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _confirmAction;
    
    private Tilemap _selectedTilemap;
    private Tilemap _hightlightTilemap;

    private bool _config;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions.FindAction("Player/Move");
        _confirmAction = _playerInput.actions.FindAction("Player/Confirm");
    }

    private void OnEnable()
    {
        _moveAction.performed += SelectedMove;
    }

    private void OnDisable()
    {
        _moveAction.performed -= SelectedMove;
    }

    public void Config(PerlingMapController perlingMapController, Tilemap selectedTilemap, Vector3Int cellPosition)
    {
        _mapController = perlingMapController;
        _selectedTilemap = selectedTilemap;
        _cellPosition = cellPosition;
        _config = true;
        
        ShowSelection();
    }

    private void SelectedMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        Vector3Int direction = Vector3Int.zero;

        if (input.x > 0f)
        {
            direction = Vector3Int.right;
        }
        else if (input.x < 0f)
        {
            direction = Vector3Int.left;
        }
        else if (input.y > 0f)
        {
            direction = Vector3Int.up;
        }
        else if (input.y < 0f)
        {
            direction = Vector3Int.down;
        }
        
        TryOtherSelection(direction);
    }

    private void TryOtherSelection(Vector3Int direction)
    {
        Vector3Int Cell = _cellPosition + direction;

        if (_mapController.CellExists(Cell))
        { 
            _cellPosition = Cell; 
            ShowSelection();
        }
    }
    
    private void ShowSelection()
    {
        _selectedTilemap.ClearAllTiles();
        _selectedTilemap.SetTile(_cellPosition, _selectedTileBase);
    }
    
}
