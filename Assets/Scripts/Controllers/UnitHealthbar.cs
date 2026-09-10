using UnityEngine;
using UnityEngine.UI;

public class UnitHealthbar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    private Unit _unit;

    private void Start()
    {
        _unit = GetComponentInParent<Unit>();

        if (_unit == null)
        {
            Debug.LogError($"{name} no encontró una unidad padre." );
            return;
        }

        _unit.StatsChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        if (_unit == null) return;
        if (healthSlider == null) return;

        if (_unit.MaxHealth <= 0)
        {
            healthSlider.value = 0f;
            return;
        }

        healthSlider.value = (float)_unit.CurrentHealth / _unit.MaxHealth;
    }

    private void OnDestroy()
    {
        if (_unit != null) _unit.StatsChanged -= Refresh;
    }

}
