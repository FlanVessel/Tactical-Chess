using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyIAController : MonoBehaviour
{
    [SerializeField] private float attackDelay = 0.25f;

    private EnemyUnit _enemyUnit;
    private EnemyMovementController _movementController;

    private void Awake()
    {
        _enemyUnit = GetComponent<EnemyUnit>();
        _movementController = GetComponent<EnemyMovementController>();
    }

    public IEnumerator ExecuteTurn(IReadOnlyList<PlayerUnit> playerUnits)
    {
        if (_enemyUnit == null) yield break;
        if (_enemyUnit.IsDead) yield break;
        if (!_enemyUnit.IsActive) yield break;

        if (TryAttack(playerUnits))
        {
            yield return new WaitForSeconds(attackDelay);
            yield break;
        }

        if (_movementController != null) yield return _movementController.ExecuteMovement(playerUnits);
        if (TryAttack(playerUnits)) yield return new WaitForSeconds(attackDelay);
    }

    private bool TryAttack(IReadOnlyList<PlayerUnit> playerUnits)
    {
        if (!_enemyUnit.CanAct()) return false;

        UnitActionData attack = GetBasicAttack();

        if (attack == null) return false;

        PlayerUnit target = FindDiagonalTarget(playerUnits);

        if (target == null) return false;
        if (!_enemyUnit.UseAction()) return false;

        Debug.Log($"{_enemyUnit.name} usa {attack.ActionName} " + $"contra {target.name}");
        
        target.TakeDamage((attack.Damage));
        return true;
    }

    private UnitActionData GetBasicAttack()
    {
        if (_enemyUnit.Data == null) return null;

        if (_enemyUnit.Data.AvailableActions.Count == 0)
        {
            Debug.LogWarning($"{_enemyUnit.name} no tiene acciones disponibles.");
            return null;
        }
        
        return _enemyUnit.Data.AvailableActions[0];
    }

    private PlayerUnit FindDiagonalTarget(IReadOnlyList<PlayerUnit> playerUnits)
    {
        foreach (PlayerUnit playerUnit in playerUnits)
        {
            if (playerUnit == null) continue;
            if (playerUnit.IsDead) continue;

            int differenceX = Mathf.Abs(_enemyUnit.CurrentCell.x - playerUnit.CurrentCell.x);
            int differenceY = Mathf.Abs(_enemyUnit.CurrentCell.y - playerUnit.CurrentCell.y);

            if (differenceX == 1 && differenceY == 1) return playerUnit;
        }

        return null;
    }
}
