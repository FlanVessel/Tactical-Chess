using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum BattlePhase{Preparing, PlayerTurn, EnemyTurn, Victory, Defeat}

public class TurnManager : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField, Min(0f)]
    private float temporaryEnemyTurnDuration = 1f;

    private readonly List<PlayerUnit> _playerUnits = new();
    private readonly List<EnemyUnit> _enemyUnits = new();

    private PlayerTacticalController _playerController;
    private BoardOccupancy _boardOccupancy; 

    private BattlePhase _currentPhase = BattlePhase.Preparing;
    private int _roundNumber;

    public BattlePhase CurrentPhase => _currentPhase;
    public int RoundNumber => _roundNumber;

    public void Setup(PlayerTacticalController playerController, BoardOccupancy boardOccupancy)
    {
        _playerController = playerController;
        _boardOccupancy = boardOccupancy;
    }

    public void RegisterUnit(Unit unit)
    {
        if (unit == null) return;

        unit.Died -= HandleUnitDied;
        unit.Died += HandleUnitDied;

        if (unit is PlayerUnit playerUnit)
        {
            if (!_playerUnits.Contains(playerUnit)) _playerUnits.Add(playerUnit);
            return;
        }

        if (unit is EnemyUnit enemyUnit)
        {
            if (!_enemyUnits.Contains(enemyUnit)) _enemyUnits.Add(enemyUnit);
        }
    }

    public void StartBattle()
    {
        if (_currentPhase != BattlePhase.Preparing)return;

        _roundNumber = 0;
        BeginPlayerTurn();
    }

    public void EndPlayerTurn()
    {
        if (_currentPhase != BattlePhase.PlayerTurn) return;

        foreach (PlayerUnit playerUnit in _playerUnits)
        {
            if (playerUnit == null) continue;
            playerUnit.EndTurn();
        }

        if (_playerController != null) _playerController.SetInputEnabled(false);

        BeginEnemyTurn();
    }

    private void BeginPlayerTurn()
    {
        _currentPhase = BattlePhase.PlayerTurn;
        _roundNumber++;

        foreach (PlayerUnit playerUnit in _playerUnits)
        {
            if (playerUnit == null) continue;
            playerUnit.BeginTurn();
        }

        if (_playerController != null) _playerController.SetInputEnabled(true);

        Debug.Log($"Comienza el turno del jugador. Ronda {_roundNumber}.");
    }

    private void BeginEnemyTurn()
    {
        _currentPhase = BattlePhase.EnemyTurn;

        foreach (EnemyUnit enemyUnit in _enemyUnits)
        {
            if (enemyUnit == null) continue;
            enemyUnit.BeginTurn();
        }

        Debug.Log("Comienza el turno enemigo.");
        StartCoroutine(ExecuteEnemyTurn());
    }

    private IEnumerator ExecuteEnemyTurn()
    {
        foreach (EnemyUnit enemyUnit in _enemyUnits)
        {
            if (enemyUnit == null) continue;
            if (enemyUnit.IsDead) continue;

            EnemyMovementController enemyMovementController = enemyUnit.GetComponent<EnemyMovementController>();

            if (enemyMovementController == null)
            {
                Debug.Log($"{enemyUnit.name} no tinene EnemyMovementController.");
                continue;
            }

            yield return enemyMovementController.ExecuteMovement(_playerUnits);

            yield return new WaitForSeconds(temporaryEnemyTurnDuration);
        }

        EndEnemyTurn();
    }

    private void EndEnemyTurn()
    {
        if (_currentPhase != BattlePhase.EnemyTurn) return;

        foreach (EnemyUnit enemyUnit in _enemyUnits)
        {
            if (enemyUnit == null) continue;
            enemyUnit.EndTurn();
        }

        BeginPlayerTurn();
    }

    private void HandleUnitDied(Unit deadUnit)
    {
        if (_boardOccupancy != null) _boardOccupancy.RemoveUnit(deadUnit);

        Debug.Log($"{deadUnit.name} fue retirado del tablero.");
    }
}
