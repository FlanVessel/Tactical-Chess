using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInputManager))]
public class LocalInputManager : MonoBehaviour
{
    public static LocalInputManager Instance { get; private set; }
    
    private PlayerInputManager _playerInputManager;
    private GameSet _gameSet;

    private readonly List<LocalPlayerInputController> _players = new();

    public IReadOnlyList<LocalPlayerInputController> Players => _players;

    public event Action<LocalPlayerInputController> PlayerJoined;
    public event Action<LocalPlayerInputController> PlayerLeft;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        _playerInputManager = GetComponent<PlayerInputManager>();
        _gameSet = GetComponentInParent<GameSet>();

        if (_gameSet == null)
        {
            Debug.Log("LocalInputManager no encontro GameSet.");
        }
    }

    private void OnEnable()
    {
        if (_playerInputManager == null) return;

        _playerInputManager.onPlayerJoined += HandlePlayerJoined;
        _playerInputManager.onPlayerLeft += HandlePlayerLeft;
    }

    private void OnDisable()
    {
        if (_playerInputManager == null) return;

        _playerInputManager.onPlayerJoined -= HandlePlayerJoined;
        _playerInputManager.onPlayerLeft -= HandlePlayerLeft;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void HandlePlayerJoined(PlayerInput playerInput)
    {
        if (_gameSet == null || playerInput == null) return;

        LocalPlayerInputController controller = playerInput.GetComponent<LocalPlayerInputController>();

        if (controller == null)
        {
            Debug.LogError($"{playerInput.name} no tiene LocalPlayerInputController.");
            Destroy(playerInput.gameObject);
            return;
        }

        string deviceName = GetDeviceDisplayName(playerInput);
        bool registered = _gameSet.TryAddLocalPlayer(playerInput.playerIndex, playerInput.user.id, deviceName, playerInput.currentControlScheme, GetDeviceType(playerInput), out LocalPlayerData playerData);

        if (!registered)
        {
            Debug.LogWarning("No fue posible registrar al jugador.");
            Destroy(playerInput.gameObject);
            return;
        }

        bool config = controller.Setup(playerData);

        if (!config)
        {
            _gameSet.RemoveLocalPlayer(playerData.InputUserId);
            Destroy(playerInput.gameObject);
            return;
        }
        
        if (!_players.Contains(controller)) _players.Add(controller);

        // Lo organizamos dentro del objeto persistente.
        playerInput.transform.SetParent(transform);
        PlayerJoined?.Invoke(controller);
        Debug.Log($"{deviceName} fue registrado por LocalInputManager.");
    }

    private void HandlePlayerLeft(PlayerInput playerInput)
    {
        if (playerInput == null) return;
        LocalPlayerInputController controller = playerInput.GetComponent<LocalPlayerInputController>();

        if (controller == null) return;
        
        bool wasRegistered = _players.Remove(controller);
        if (!wasRegistered) return;

        if (_gameSet != null && controller.PlayerData != null) _gameSet.RemoveLocalPlayer(playerInput.user.id);
        
        PlayerLeft?.Invoke(controller);
    }

    public void EnableJoining()
    {
        if (_playerInputManager == null) return;
        _playerInputManager.EnableJoining();
    }

    public void DisableJoining()
    {
        if (_playerInputManager == null) return;
        _playerInputManager.DisableJoining();
    }

    private string GetDeviceDisplayName(PlayerInput playerInput)
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse") return "Teclado y ratón";

        foreach (InputDevice device in playerInput.devices)
        {
            if (device is not Gamepad) continue;
            if (!string.IsNullOrWhiteSpace(device.description.product)) return device.description.product;
            return device.displayName;
        }

        return "Dispositivo desconocido";
    }

    private LocalDeviceType GetDeviceType(PlayerInput playerInput)
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse") return LocalDeviceType.KeyboardMouse;
        
        foreach (InputDevice device in playerInput.devices)
        {
            if (device is not Gamepad) continue;

            string description = ($"{device.description.manufacturer} " + $"{device.description.product} " + $"{device.displayName}").ToLowerInvariant();

            if (description.Contains("xbox") || description.Contains("microsoft")) return LocalDeviceType.Xbox;
            if (description.Contains("dualshock") || description.Contains("dualsense") || description.Contains("playstation")) return LocalDeviceType.Playstation;

            return LocalDeviceType.GenericGamepad;
        }

        return LocalDeviceType.None;
    }

    public void ClearLocalPlayers()
    {
        DisableJoining();
        
        LocalPlayerInputController[] playersToRemove = new LocalPlayerInputController[_players.Count];
        _players.CopyTo(playersToRemove, 0);
        _players.Clear();

        foreach (LocalPlayerInputController controller in playersToRemove)
        {
            if (controller == null) continue;
            
            LocalPlayerData playerData = controller.PlayerData;
            
            if (_gameSet != null && playerData != null) _gameSet.RemoveLocalPlayer(controller.PlayerData.InputUserId);
            
            Destroy(controller.gameObject);
        }
    }
}
