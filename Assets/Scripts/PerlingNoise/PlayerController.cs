using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("TileBase de Selected")] 
    [SerializeField] private TileBase selectedTileBase;
    
    private PerlingMapController _mapController;
    private PerBoardOccupancy _perBoardOccupancy;
    
    private Vector3Int _cellPosition;
    
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _confirmAction;
    
    private Tilemap _selectedTilemap;
    private Tilemap _hightlightTilemap;
    
    private PerUnitPawn _selectedPawn;

    private bool _movementLocked;
    
    public PerUnitPawn SelectedPawn => _selectedPawn;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions.FindAction("Player/Move");
        _confirmAction = _playerInput.actions.FindAction("Player/Confirm");
    }

    private void OnEnable()
    {
        _moveAction.performed += SelectedMove;
        _moveAction.canceled += MoveCancel;

        _confirmAction.performed += ConfirmSelection;
    }

    private void OnDisable()
    {
        _moveAction.performed -= SelectedMove;
        _moveAction.canceled -= MoveCancel;
        
        _confirmAction.performed -= ConfirmSelection;
    }

    public void ConfigPlayer(PerlingMapController perlingMapController, Tilemap selectedTilemap, Vector3Int cellPosition, PerBoardOccupancy perBoardOccupancy)
    {
        _mapController = perlingMapController;
        _selectedTilemap = selectedTilemap;
        _cellPosition = cellPosition;
        _perBoardOccupancy = perBoardOccupancy;
        
        ShowSelection();
    }

    private void SelectedMove(InputAction.CallbackContext ctx)
    {
        if (_movementLocked) return;
        
        Vector2 input = ctx.ReadValue<Vector2>();
        float miniumInput = 0.5f;
        Vector3Int direction = Vector3Int.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            if (input.x > miniumInput)
            {
                direction = Vector3Int.right;
            }
            else if (input.x < -miniumInput)
            {
                direction = Vector3Int.left;
            }
        }
        else
        {
            if (input.y > miniumInput)
            {
                direction = Vector3Int.up;
            }
            else if (input.y < -miniumInput)
            {
                direction = Vector3Int.down;
            }
        }
        
        if (direction == Vector3Int.zero) return;
        
        _movementLocked  = true;
        
        Vector3Int cell = _cellPosition + direction;

        if (_mapController.CellExists(cell))
        {
            _cellPosition = cell;
            ShowSelection();
        }
    }
    
    private void ShowSelection()
    {
        _selectedTilemap.ClearAllTiles();
        _selectedTilemap.SetTile(_cellPosition, selectedTileBase);
    }

    private void MoveCancel(InputAction.CallbackContext ctx)
    {
        _movementLocked = false;
    }

    private void ConfirmSelection(InputAction.CallbackContext ctx)
    {
        PerUnit perUnitAtCell = _perBoardOccupancy.UnitAt(_cellPosition);
        if (perUnitAtCell == null)
        {
            Debug.Log($"No existe ninguna unidad en {_cellPosition}");
            return;
        }
        
        PerUnitPawn perUnitPawnAtCell = perUnitAtCell as PerUnitPawn;
        if (perUnitPawnAtCell == null)
        {
            Debug.Log($"La unidad en {_cellPosition} no es un peon");
            return;
        }

        _selectedPawn = perUnitPawnAtCell;
        Debug.Log($"Peon : {_selectedPawn}. " + $"Cell: {_selectedPawn.CurrentCell}.");
    }

}
