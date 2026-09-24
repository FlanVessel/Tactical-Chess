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
        LocalPlayerData player = inputController.PlayerData;
        
        if (player == null) return;
        if (player.IsReady) return;
        
        player.MoveSelectedSlot(direction, playerSlots.Length);
        RefreshLobbyUI();
    }

    private void HandleSubmitRequested(LocalPlayerInputController inputController)
    {
        LocalPlayerData player = inputController.PlayerData;
        
        if (player == null) return;

        int selectedSlot = player.SelectedSlotIndex;
        
        if (player.SlotIndex < 0)
        {
            if (IsSlotOccupied(selectedSlot))
            {
                Debug.Log($"El espacio {selectedSlot + 1} ya esta ocupado.");
                return;
            }
            player.AssignSlot(selectedSlot);
            Debug.Log($"{player.DeviceName} ocupo el espacio {selectedSlot + 1}.");
        }
        else
        {
            player.SetReady(true);
            Debug.Log($"{player.DeviceName} esta listo en {selectedSlot + 1}.");
        }
        RefreshLobbyUI();
    }

    private void HandleCancelRequested(LocalPlayerInputController inputController)
    {
        LocalPlayerData player = inputController.PlayerData;
        
        if (player == null) return;

        if (player.IsReady)
        {
            player.SetReady(false);
            Debug.Log($"{player.DeviceName} no esta listo.");
        }
        else if (player.SlotIndex >= 0)
        {
            player.RealeaseSlot();
            Debug.Log($"{player.DeviceName} solto su lugar.");
        }
        else
        {
            Debug.Log($"{player.DeviceName} todavia no tiene un lugar.");
        }
        RefreshLobbyUI();
    }

    private void HandlePlayerLeft(PlayerInput playerInput)
    {
        if (_gameSet == null || playerInput == null) return;
        _gameSet.RemoveLocalPlayer(playerInput.user.id);
    }

    private bool IsSlotOccupied(int slotIndex)
    {
        foreach (LocalPlayerData player in _gameSet.Players)
        {
            if (player == null) continue;
            if (player.SlotIndex == slotIndex) return true;
        }
        return false;
    }

    private void RefreshLobbyUI()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            PlayerSlotUI slotUI = playerSlots[i];
            
            if (slotUI == null) continue;
            
            LocalPlayerData occupant = FindPlayerInSlot(i);

            if (occupant == null)
            {
                slotUI.ShowAvailable();
            }
            else if (occupant.IsReady)
            {
                slotUI.ShowReady(occupant);
            }
            else
            {
                slotUI.ShowOccupied(occupant);
            }
            
            slotUI.ShowSelection(false, Color.white);
        }

        foreach (LocalPlayerData player in _gameSet.Players)
        {
            if (player == null || player.IsReady) continue;
            
            int selectedSlot = player.SelectedSlotIndex;

            if (selectedSlot < 0 || selectedSlot >= playerSlots.Length) continue;
            
            playerSlots[selectedSlot].ShowSelection(true, GetPlayerColor(player.PlayerIndex));
        }
    }

    private LocalPlayerData FindPlayerInSlot(int slotIndex)
    {
        foreach (LocalPlayerData player in _gameSet.Players)
        {
            if (player == null) continue;
            if (player.SlotIndex == slotIndex) return player;
        }
        return null;
    }

    private Color GetPlayerColor(int playerIndex)
    {
        return playerIndex switch
        {
            0 => Color.cyan,
            1 => Color.yellow,
            2 => Color.green,
            3 => Color.blue,
            _ => Color.white
        };
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
