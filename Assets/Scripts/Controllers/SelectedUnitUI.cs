using UnityEngine;
using TMPro;

public class SelectedUnitUI : MonoBehaviour
{
    [SerializeField] private TMP_Text unitNameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text manaRecoveryText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text movementText;

    private Unit _selectedUnit;

    public void Show(Unit unit)
    {
        if (_selectedUnit != null) _selectedUnit.StatsChanged -= Refresh;

        _selectedUnit = unit;

        if (_selectedUnit == null)
        {
            Hide();
            return;
        }

        _selectedUnit.StatsChanged += Refresh;

        gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        if (_selectedUnit != null) _selectedUnit.StatsChanged -= Refresh;

        _selectedUnit = null;
        gameObject.SetActive(false);
    }

    private void Refresh()
    {
        if (_selectedUnit == null) return;

        unitNameText.text = _selectedUnit.Data.UnitName;

        healthText.text = $"Vida: {_selectedUnit.CurrentHealth}/" + $"{_selectedUnit.MaxHealth}";

        manaText.text = $"Maná: {_selectedUnit.CurrentMana}/" + $"{_selectedUnit.MaximumMana}";

        manaRecoveryText.text = $"Recuperación: +{_selectedUnit.ManaRecovery}";

        int currentDamage = _selectedUnit.CalculateAttackDamage(1);
        damageText.text = $"Daño actual: {currentDamage}";

        string movementStatus = _selectedUnit.HasMoved ? "Utilizado" : "Disponible";
        movementText.text = $"Mov: {_selectedUnit.MoveRange}" + $"({movementStatus})";
    }

    private void OnDestroy()
    {
        if (_selectedUnit != null) _selectedUnit.StatsChanged -= Refresh;
    }
}
