using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthbar : MonoBehaviour
{
    [SerializeField] private Image heartFillImage;
    [SerializeField] private float velocityImage;
    [SerializeField] private float distanceImage;
    
    private RectTransform rectTransform;
    private Vector2 position;

    private Unit _unit;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        position = rectTransform.anchoredPosition;
        
        _unit = GetComponentInParent<Unit>();

        if (_unit == null)
        {
            Debug.LogError($"{name} no encontró una unidad padre." );
            return;
        }
        
        if (heartFillImage == null)
        {
            Debug.LogError($"{name} no hay un HeartFill." );
            return;
        }
        
        heartFillImage.type = Image.Type.Filled;
        heartFillImage.fillMethod = Image.FillMethod.Vertical;
        heartFillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        heartFillImage.raycastTarget = false;

        _unit.StatsChanged += Refresh;
        Refresh();
    }

    private void Update()
    {
        float newYPosition = position.y + Mathf.Sin(Time.time * velocityImage) * distanceImage;
        rectTransform.anchoredPosition = new Vector2(position.x, newYPosition);
    }

    private void Refresh()
    {
        if (_unit == null  || heartFillImage == null) return;
        if (_unit.MaxHealth <= 0)
        {
            heartFillImage.fillAmount = 0f;
            return;
        }

        float healthPercentage = (float)_unit.CurrentHealth / _unit.MaxHealth;
        heartFillImage.fillAmount = Mathf.Clamp01(healthPercentage);
    }

    private void OnDestroy()
    {
        if (_unit != null) _unit.StatsChanged -= Refresh;
    }

}
