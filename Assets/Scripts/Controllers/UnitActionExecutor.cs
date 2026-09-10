using UnityEngine;

public class UnitActionExecutor : MonoBehaviour
{
    private Unit _unit;
    private UnitActionData _unitAction;
    private PawnDeckController _deckController;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        _deckController = GetComponent<PawnDeckController>();
    }

    public bool TryUseCard(CardData card)
    {
        if (_unit == null) return false;
        if (_deckController == null) return false;
        if (card == null || card.Action == null) return false;

        if (!_unit.IsActive)
        {
            Debug.Log("El peón no está en su turno.");
            return false;
        }

        if (!_deckController.HasCardInHand(card))
        {
            Debug.Log("La carta no está en la mano.");
            return false;
        }

        if (!_unit.CanSpendMana(card.ManaCost))
        {
            Debug.Log($"{_unit.name} no tiene suficiente maná. " + $"Necesita {card.ManaCost} y tiene " + $"{_unit.CurrentMana}.");
            return false;
        }

        UnitActionData action = card.Action;
        
        if (!CanApplyAction(action)) return false;
        
        if (!_unit.SpendMana(card.ManaCost)) return false;

        ApplyAction(action);
        _deckController.UseCard(card);

        Debug.Log($"{_unit.name} utilizó {card.CardName}. " + $"Maná restante: {_unit.CurrentMana}/" + $"{_unit.MaximumMana}." );

        return true;
    }

    private bool CanApplyAction(UnitActionData action)
    {
        switch (action.ActionType)
        {
            case UnitActionType.IncreaseDamage:
                return true;
            
            case UnitActionType.Heal:
                if (_unit.CurrentHealth >= _unit.MaxHealth)
                {
                    Debug.Log($"{_unit.name} ya tiene la vida completa " + $"no se utilizo la carta.");
                    return false;
                }
                return true;
            
            default:
                Debug.Log($"La accion {action.ActionType} todavia  " + $"no puede ejecutarse.");
                return false;
        }
    }

    private void ApplyAction(UnitActionData action)
    {
        switch (action.ActionType)
        {
            case UnitActionType.IncreaseDamage:
                _unit.AddTemporaryDamage(action.EffectAmount);
                break;
            case UnitActionType.Heal:
                _unit.Heal(action.EffectAmount);
                break;
        }
    }
}
