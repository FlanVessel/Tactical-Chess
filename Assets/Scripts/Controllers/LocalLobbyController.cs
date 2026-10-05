using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class DeviceIconEntry
{
    public LocalDeviceType deviceType;
    public Sprite icon;
}

public class LocalLobbyController : MonoBehaviour
{
    private GameSet _gameSet;
    private LocalInputManager _localInputManager;
    
    [Header("Interfaz Lobby")]
    [SerializeField] private PlayerSlotUI[] playerSlots;
    [SerializeField] private Button continueBattleButton;
    
    [Header("Imagen de Dispositivos")]
    [SerializeField] private List<DeviceIconEntry> deviceIcons = new();
    
    private readonly HashSet<LocalPlayerInputController> _subscribedPlayers = new();

    private void Awake()
    {
        if (GameManager.Instance == null) return;
        
        _gameSet = GameManager.Instance.Set;
        _localInputManager = LocalInputManager.Instance;
    }

    private void Start()
    {
        InitialPlayerSlots();

        if (_gameSet == null || _localInputManager == null)
        {
            UpdateBattleButton();
            return;
        }
        
        _localInputManager.EnableJoining();
        
        foreach (LocalPlayerInputController controller in _localInputManager.Players) SubscribePlayer(controller);
        
        RefreshLobbyUI();
    }

    private void InitialPlayerSlots() //Los slots de los jugadores
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i] == null) continue;
            playerSlots[i].Initialize(i);
        }
    }

    private void OnEnable()
    {
        _localInputManager.PlayerJoined += HandleInputPlayerJoined;
        _localInputManager.PlayerLeft += HandleInputPlayerLeft;
    }

    private void OnDisable()
    {
        _localInputManager.PlayerJoined -= HandleInputPlayerJoined;
        _localInputManager.PlayerLeft -= HandleInputPlayerLeft;
        UnsubscribePlayersExisting();
    }

    private void HandleInputPlayerJoined(LocalPlayerInputController controller)
    {
        SubscribePlayer(controller);
        RefreshLobbyUI();
    }
    
    private void HandleInputPlayerLeft(LocalPlayerInputController controller)
    {
        UnsubscribePlayer(controller);
        RefreshLobbyUI();
    }

    private void SubscribePlayer(LocalPlayerInputController controller)
    {
        if (controller == null) return;
        if (!_subscribedPlayers.Add(controller)) return;
        
        controller.NavigateRequested += HandleNavigateRequested;
        controller.SubmitRequested += HandleSubmitRequested;
        controller.CancelRequested += HandleCancelRequested;
    }
    
    private void UnsubscribePlayer(LocalPlayerInputController controller)
    {
        if (controller == null) return;
        if (!_subscribedPlayers.Remove(controller)) return;
        
        controller.NavigateRequested -= HandleNavigateRequested;
        controller.SubmitRequested -= HandleSubmitRequested;
        controller.CancelRequested -= HandleCancelRequested;
    }

    private void UnsubscribePlayersExisting()
    {
        LocalPlayerInputController[] controllers = new LocalPlayerInputController[_subscribedPlayers.Count];
        _subscribedPlayers.CopyTo(controllers);
        
        foreach (LocalPlayerInputController controller in controllers) UnsubscribePlayer(controller);
    }
    
    private void HandleNavigateRequested(LocalPlayerInputController inputController, int direction) //Como podra navegar los jugadores
    {
        LocalPlayerData player = inputController.PlayerData;
        
        if (player == null) return;
        if (player.IsReady) return;
        if (player.SlotIndex >= 0) return;
        
        player.MoveSelectedSlot(direction, playerSlots.Length);
        RefreshLobbyUI();
    }

    private void HandleSubmitRequested(LocalPlayerInputController inputController) //Cuando lo jugadores aceptan el slot en el que van a jugar
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

    private void HandleCancelRequested(LocalPlayerInputController inputController) //Cuando los jugadores cancelan el slot
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

    private bool IsSlotOccupied(int slotIndex) //Cuando esta ocupado
    {
        foreach (LocalPlayerData player in _gameSet.Players)
        {
            if (player == null) continue;
            if (player.SlotIndex == slotIndex) return true;
        }
        return false;
    }

    private void RefreshLobbyUI() //refrescamos la UI
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
            else
            {
                Sprite deviceIcon = GetDeviceIcon(occupant.DeviceType);

                if (occupant.IsReady)
                {
                    slotUI.ShowReady(occupant, deviceIcon);
                }
                else
                {
                    slotUI.ShowOccupied(occupant, deviceIcon);
                }
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
        
        UpdateBattleButton();
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

    private Sprite GetDeviceIcon(LocalDeviceType deviceType)
    {
        foreach (DeviceIconEntry entry in deviceIcons)
        {
            if (entry.deviceType == deviceType) return entry.icon;
        }
        return null;
    }

    private void UpdateBattleButton()
    {
        if (continueBattleButton == null) return;
        continueBattleButton.interactable = CanContinue();
    }

    public bool CanContinue()
    {
        if (_gameSet == null) return false;
        if (_gameSet.Players.Count < 2) return false;

        foreach (LocalPlayerData player in _gameSet.Players)
        {
            if (player == null) return false;
            if (player.SlotIndex < 0) return false;
            if (!player.IsReady) return false;
        }
        return true;
    }
}
