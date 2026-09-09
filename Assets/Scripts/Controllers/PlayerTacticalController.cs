using System;
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

    private PlayerUnit _selectedUnit;
    private TurnMovementController _selectedMovement;

    [SerializeField] private Button attackButton;
    
    [SerializeField] private CardHandUI cardHandUI;
    
    private Tilemap _highlightTilemap;
    private TileBase _attackTile;
    private UnitCombatController _selectedCombat;
    private UnitActionData _selectedAction;

    private bool _attackMode;
    
    private CardData _selectedCard; 

    private readonly HashSet<Vector3Int> _attackCells = new();

    private bool _inputEnabled;
    
    private void Awake()
    {
        if (cardHandUI != null) cardHandUI.HideHand();
    }

    public void Setup(Tilemap boardTilemap, Camera gameCamera, Tilemap highlightTilemap, TileBase attackTile)
    {
        _boardTilemap = boardTilemap;
        _gameCamera = gameCamera;
        _highlightTilemap = highlightTilemap;
        _attackTile = attackTile;

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
        UnitCombatController combat = unit.GetComponent<UnitCombatController>();

        if (movement == null)
        {  
            unit.Deselect();
            Debug.LogError($"{unit.name} no tiene TurnMovementController.");
            return;
        }
        
        if (combat == null)
        {  
            unit.Deselect();
            Debug.LogError($"{unit.name} no tiene UnitCombatController.");
            return;
        }

        _selectedUnit = unit;
        _selectedMovement = movement;
        _selectedCombat = combat;

        _selectedMovement.ShowReachableCells();
        ShowSelectedUnitHand();
        UpdateAttackButton();
    }

    private void DeselectCurrentUnit()
    {
        if (_selectedUnit != null) _selectedUnit.Deselect();

        if (_selectedMovement != null) _selectedMovement.ClearSelection();

        _selectedUnit = null;
        _selectedMovement = null;
        _selectedCombat = null;
        _selectedAction = null;
        _selectedCard  = null;

        CancelAttackMode();
        
        if (cardHandUI != null) cardHandUI.HideHand();
        
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
        if (_selectedCombat == null) return;
        if (!_selectedUnit.CanAct()) return;

        _selectedAction = GetBasicAttack();
        
        if (_selectedAction == null) return;

        _attackMode = true;

        if (_selectedMovement != null) _selectedMovement.ClearSelection();

        ShowAttackCells();
    }

    private void ShowAttackCells()
    {
        ClearAttackCells();

        if (_selectedCombat == null || _selectedAction == null) return;

        HashSet<Vector3Int> cells = _selectedCombat.GetAttackCells(_selectedAction);

        foreach (Vector3Int cell in cells)
        {
            _attackCells.Add(cell);
            _highlightTilemap.SetTile(cell, _attackTile);
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

        bool attacked = _selectedCombat.TryAttack(clickedCell, _selectedAction);

        if (!attacked)
        {
            Debug.Log("No hay ninguna unidad en esta casilla.");
            CancelAttackMode();
            UpdateAttackButton();
            return;
        }

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

    private UnitActionData GetBasicAttack()
    {
        if (_selectedUnit == null) return null;
        if (_selectedUnit.Data.AvailableActions.Count == 0) return null;
        
        return _selectedUnit.Data.AvailableActions[0];
    }

    private void ShowSelectedUnitHand()
    {
        if (cardHandUI == null)
        {
            Debug.LogWarning($"El PlayerTacticalController {name} no tiene CardHandUI.");
            return;
        }

        if (_selectedUnit == null)
        {
            cardHandUI.HideHand();
            return;
        }
        
        PawnDeckController deckController = _selectedUnit.DeckController;

        if (deckController == null)
        {
            Debug.LogWarning($"{_selectedUnit.name} no tiene un mazo.");
            cardHandUI.HideHand();
            return;
        }
        
        cardHandUI.ShowHand(deckController.Hand, HandleCardSelected);
    }

    private void HandleCardSelected(CardData card)
    {
        if (card == null) return;
        if (_selectedUnit == null) return; 
        
        _selectedCard = card; 
        
        Debug.Log($"{_selectedUnit.name} selecciono la carta " + $"{_selectedCard.CardName}.");

        if (_selectedCard.Action != null)
        {
            Debug.Log($"Accion: {_selectedCard.Action.ActionName}, " + $"Tipo: {_selectedCard.Action.ActionType}, " + $"Cantidad: {_selectedCard.Action.EffectAmount}");
        }
    }
}
