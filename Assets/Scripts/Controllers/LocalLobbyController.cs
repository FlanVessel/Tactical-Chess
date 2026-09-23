using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class LocalLobbyController : MonoBehaviour
{
    private PlayerInputManager _playerInputManager;
    private GameSet _gameSet;
    
    [Header("Interfaz Lobby")]
    [SerializeField] private PlayerSlotUI[] playerSlots;

    private void Awake()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();

        if (GameManager.Instance == null)
        {
            Debug.LogError("No existe GameManager. Ejecuta el juego desde Bootstrap.");
            return;
        }

        _gameSet = GameManager.Instance.Set;
        if (_gameSet == null) Debug.LogError("GameManager no contiene una GameSession.");
    }

    private void Start()
    {
        InitialPlayerSlots();
    }

    private void InitialPlayerSlots()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i] == null) continue;
            playerSlots[i].Initialize(i);
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

    private void HandlePlayerJoined(PlayerInput playerInput)
    {
        if (_gameSet == null || playerInput == null) return;

        string deviceName = GetDeviceDisplayName(playerInput);

        bool registered = _gameSet.TryAddLocalPlayer(playerInput.playerIndex, playerInput.user.id, deviceName, playerInput.currentControlScheme, out LocalPlayerData playerData);

        if (!registered)
        {
            Debug.LogWarning("No fue posible registrar al jugador en la sesión." );
            Destroy(playerInput.gameObject);
            return;
        }
        
        LocalPlayerInputController inputController = playerInput.GetComponent<LocalPlayerInputController>();

        if (inputController == null)
        {
            Debug.LogError($"{playerInput.name} no tiene LocalPlayerInputController.");
            
            _gameSet.RemoveLocalPlayer(playerInput.user.id);
            Destroy(playerInput.gameObject);
            return;
        }

        bool config = inputController.Setup(playerData);

        if (!config)
        {
            Debug.LogError($"No fue posible configurar la entrada {deviceName}.");
            
            _gameSet.RemoveLocalPlayer(playerInput.user.id);
            Destroy(playerInput.gameObject);
            return;
        }

        inputController.NavigateRequested += HandleNavigateRequested;
        inputController.SubmitRequested += HandleSubmitRequested;
        inputController.CancelRequested += HandleCancelRequested;

        Debug.Log($"{deviceName} está esperando seleccionar un espacio." );
    }
    
    private void HandleNavigateRequested(LocalPlayerInputController inputController, int direction)
    {
        string directionName = direction > 0 ? "derecha" : "izquierda";
        Debug.Log($"{inputController.PlayerData.DeviceName} " + $"navega hacia la {directionName}.");
    }

    private void HandleSubmitRequested(LocalPlayerInputController inputController)
    {
        Debug.Log($"{inputController.PlayerData.DeviceName} presionó confirmar.");
    }

    private void HandleCancelRequested(LocalPlayerInputController inputController)
    {
        Debug.Log($"{inputController.PlayerData.DeviceName} presionó cancelar.");
    }

    private void HandlePlayerLeft(PlayerInput playerInput)
    {
        if (_gameSet == null || playerInput == null) return;
        _gameSet.RemoveLocalPlayer(playerInput.user.id);
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
}
