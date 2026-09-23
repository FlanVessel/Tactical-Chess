using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(PlayerInput))]
public class LocalPlayerInputController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private LocalPlayerData _playerData;

    private InputAction _navigateAction;
    private InputAction _submitAction;
    private InputAction _cancelAction;

    private bool _isConfigured;

    public PlayerInput PlayerInput => _playerInput;
    public LocalPlayerData PlayerData => _playerData;

    public event Action<LocalPlayerInputController, int> NavigateRequested;
    public event Action<LocalPlayerInputController> SubmitRequested;
    public event Action<LocalPlayerInputController> CancelRequested;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public bool Setup(LocalPlayerData playerData)
    {
        if (_isConfigured) return true;
        if (playerData == null)
        {
            Debug.LogError($"{name} recibió un LocalPlayerData vacío.");
            return false;
        }
        
        if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError($"{name} no tiene PlayerInput.");
            return false;
        }

        if (_playerInput.actions == null)
        {
            Debug.LogError($"{name} el componente PlayerInput no tiene InputSystem_Actions asignado.");
            return false;
        }

        _playerData = playerData;

        _navigateAction = _playerInput.actions.FindAction("Lobby/Navigate");
        _submitAction = _playerInput.actions.FindAction("Lobby/Submit");
        _cancelAction = _playerInput.actions.FindAction("Lobby/Cancel");

        if (_navigateAction == null || _submitAction == null || _cancelAction == null)
        {
            Debug.LogError("No se encontraron las acciones del mapa Lobby.");
            return false;
        }

        _navigateAction.performed += HandleNavigate;
        _submitAction.performed += HandleSubmit;
        _cancelAction.performed += HandleCancel;

        _isConfigured = true;

        Debug.Log($"{_playerData.DeviceName} quedó configurado para el lobby.");
        return true;
    }

    private void HandleNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigation = context.ReadValue<Vector2>();

        if (Mathf.Abs(navigation.x) < 0.5f) return;

        int direction = navigation.x > 0f ? 1 : -1;

        NavigateRequested?.Invoke(this, direction);
    }

    private void HandleSubmit(InputAction.CallbackContext context)
    {
        SubmitRequested?.Invoke(this);
    }

    private void HandleCancel(InputAction.CallbackContext context)
    {
        CancelRequested?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (!_isConfigured) return;

        _navigateAction.performed -= HandleNavigate;
        _submitAction.performed -= HandleSubmit;
        _cancelAction.performed -= HandleCancel;
    }
}
