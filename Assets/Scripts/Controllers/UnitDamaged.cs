using System;
using UnityEngine;
using System.Collections;

public class UnitDamaged : MonoBehaviour
{
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 1.0f;

    private Unit _unit;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
    }

    private void OnEnable()
    {
        if (_unit != null) _unit.Damaged += HandleDamage;
    }

    private void OnDisable()
    {
        if (_unit != null) _unit.Damaged -= HandleDamage;
        if (_spriteRenderer == null) _spriteRenderer.color = _originalColor;
    }

    private void HandleDamage(Unit damagedUnit, int amount)
    {
        if (_spriteRenderer == null) return;
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        _spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        _spriteRenderer.color = _originalColor;
        _flashCoroutine = null;
    }
}
