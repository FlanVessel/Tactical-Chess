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

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _inputAction = _playerInput.actions.FindAction("Player/Move");
    }

    private void OnEnable()
    {
        _inputAction.performed += SelectedMove;
    }

    private void OnDisable()
    {
        _inputAction.performed -= SelectedMove;
    }

    public void Config(PerlingMapController perlingMapController, Tilemap tilemap, Vector3Int cellPosition)
    {
        _mapController = perlingMapController;
        _tilemap = tilemap;
        _cellPosition = cellPosition;
        _config = true;
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
        _tilemap.ClearAllTiles();
        _tilemap.SetTile(_cellPosition, _tileBase);
    }
    
}
