using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerTacticalController : MonoBehaviour
{
    private Camera _gameCamera;
    private Tilemap _boardTilemap;

    private readonly List<PlayerUnit> _playerUnits = new();

    private PlayerUnit _selectedUnit;
    private TurnMovementController _selectedMovement;

    public void Setup(Tilemap boardTilemap, Camera gameCamera)
    {
        _boardTilemap = boardTilemap;
        _gameCamera = gameCamera;
    }

    public void RegisterUnit (PlayerUnit unit)
    {
        if (unit == null) return;
        if (_playerUnits.Contains(unit)) return;

        _playerUnits.Add(unit);
    }

    private void Update()
    {
        if (_boardTilemap == null) return;
        if (_gameCamera == null) return;
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) HandleClick();
    }

    private void HandleClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(_gameCamera.transform.position.z));

        Vector3 worldPosition = _gameCamera.ScreenToWorldPoint(screenPosition);

        Vector3Int clickedCell = _boardTilemap.WorldToCell(worldPosition);

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
            if (unit == null)  continue;

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
    }

    private void DeselectCurrentUnit()
    {
        if (_selectedUnit != null) _selectedUnit.Deselect();

        if (_selectedMovement != null) _selectedMovement.ClearSelection();

        _selectedUnit = null;
        _selectedMovement = null;
    }
}
