using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerTacticalController : MonoBehaviour
{
    private Camera _gameCamera;
    private Tilemap _boardTilemap;

    private readonly List<PlayerUnit> _playerUnits = new();

    private static readonly Vector3Int[] AttackDirections = {new Vector3Int(1, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0)};

    private PlayerUnit _selectedUnit;
    private TurnMovementController _selectedMovement;

    [SerializeField] private Button attackButton;

    private BoardOccupancy _boardOccupancy;
    private Tilemap _highlightTilemap;
    private TileBase _attackTile;
    [SerializeField] private UnitActionData _basicAttack;

    private bool _attackMode;

    private readonly HashSet<Vector3Int> _attackCells = new();

    private bool _inputEnabled;

    public void Setup(Tilemap boardTilemap, Camera gameCamera, BoardOccupancy boardOccupancy, Tilemap highlightTilemap, TileBase attackTile, UnitActionData basicAttack)
    {
        _boardTilemap = boardTilemap;
        _gameCamera = gameCamera;
        _boardOccupancy = boardOccupancy;
        _highlightTilemap = highlightTilemap;
        _attackTile = attackTile;
        _basicAttack = basicAttack;

        UpdateAttackButton();
    }

    public void RegisterUnit (PlayerUnit unit)
    {
        if (unit == null) return;
        if (_playerUnits.Contains(unit)) return;

        _playerUnits.Add(unit);
    }

    private void Update()
    {
        if (!_inputEnabled) return;
        if (_boardTilemap == null) return;
        if (_gameCamera == null) return;
        if (Mouse.current == null) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) HandleClick();
    }

    private void HandleClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(_gameCamera.transform.position.z));

        Vector3 worldPosition = _gameCamera.ScreenToWorldPoint(screenPosition);

        Vector3Int clickedCell = _boardTilemap.WorldToCell(worldPosition);

        if (_attackMode)
        {
            HandleAttackClick(clickedCell);
            return;
        }

        PlayerUnit clickedUnit = FindPlayerAtCell(clickedCell);

        if (clickedUnit != null)
        {
            SelectUnit(clickedUnit);
            return;
        }

        if (_selectedMovement != null) _selectedMovement.TryMoveTo(clickedCell);
    }

    private PlayerUnit FindPlayerAtCell(Vector3Int cell)
    {

        foreach (PlayerUnit unit in _playerUnits)
        {
            if (unit == null) continue;
            if (unit.IsDead) continue;

            if (unit.CurrentCell == cell) return unit;
        }

        return null;
    }

    private void SelectUnit(PlayerUnit unit)
    {
        if (unit == null) return;

        if (_selectedUnit == unit)
        {
            DeselectCurrentUnit();
            return;
        }

        DeselectCurrentUnit();

        if (!unit.Select()) return;

        TurnMovementController movement = unit.GetComponent<TurnMovementController>();

        if (movement == null)
        {  
            unit.Deselect();
            Debug.LogError($"{unit.name} no tiene TurnMovementController.");
            return;
        }

        _selectedUnit = unit;
        _selectedMovement = movement;

        _selectedMovement.ShowReachableCells();
        UpdateAttackButton();
    }

    private void DeselectCurrentUnit()
    {
        if (_selectedUnit != null) _selectedUnit.Deselect();

        if (_selectedMovement != null) _selectedMovement.ClearSelection();

        _selectedUnit = null;
        _selectedMovement = null;

        CancelAttackMode();
        UpdateAttackButton();
    }

    public void SetInputEnabled(bool value)
    {
        _inputEnabled = value;

        if (!_inputEnabled) DeselectCurrentUnit();

        UpdateAttackButton();
    }

    public void BeginAttackMode()
    {
        if (!_inputEnabled) return;
        if (_selectedUnit == null) return;
        if (!_selectedUnit.CanAct()) return;
        if (_basicAttack == null) return;

        _attackMode = true;

        if (_selectedMovement != null) _selectedMovement.ClearSelection();

        ShowAttackCells();
    }

    private void ShowAttackCells()
    {
        ClearAttackCells();

        if (_selectedUnit == null) return;

        foreach (Vector3Int direction in AttackDirections)
        {
            Vector3Int attackCell = _selectedUnit.CurrentCell + direction;

            if (!_boardTilemap.HasTile(attackCell)) continue;

            _attackCells.Add(attackCell);
            _highlightTilemap.SetTile(attackCell, _attackTile);
        }
    }

    private void HandleAttackClick(Vector3Int clickedCell)
    {
        if (!_attackCells.Contains(clickedCell))
        {
            Debug.Log("La casilla no es parte del rango de ataque.");
            CancelAttackMode();
            return;
        }

        Unit target = _boardOccupancy.GetUnitAt(clickedCell);

        if (target == null)
        {
            Debug.Log("No hay ninguna unidad en esta casilla.");
            CancelAttackMode();
            return;
        }

        if (target is not EnemyUnit)
        {
            Debug.Log("El Ataque basico ataca enemigos.");
            CancelAttackMode();
            return;
        }

        if (!_selectedUnit.UseAction())
        {
            Debug.Log("El peon ya utilizo su accion.");
            CancelAttackMode();
            return;
        }

        Debug.Log($"{_selectedUnit.name} usa {_basicAttack.ActionName} " + $"contra {target.name}.");

        target.TakeDamage(_basicAttack.Damage);

        CancelAttackMode();
        UpdateAttackButton();
    }

    private void UpdateAttackButton()
    {
        if (attackButton == null) return;

        attackButton.interactable = _inputEnabled && _selectedUnit != null && _selectedUnit.CanAct();
    }

    private void ClearAttackCells()
    {
        _attackCells.Clear();

        if (_highlightTilemap != null) _highlightTilemap.ClearAllTiles();
    }

    private void CancelAttackMode()
    {
        _attackMode = false;
        ClearAttackCells();
    }
}
