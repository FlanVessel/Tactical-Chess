using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    [SerializeField] private Button returnButton;
    [SerializeField] private Button continueBattleButton;
    
    private Button[] _hostButtons;
    private int _selectedButtonIndex;
    
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

        _hostButtons = new Button[] { returnButton, continueBattleButton };
        _selectedButtonIndex = 1;

        if (EventSystem.current != null) EventSystem.current.sendNavigationEvents = false;

        if (_gameSet == null || _localInputManager == null)
        {
            UpdateBattleButton();
            return;
        }
        
        _localInputManager.EnableJoining();
        
        foreach (LocalPlayerInputController controller in _localInputManager.Players)
        {
            PreparePlayersLobby(controller);
        }
        
        RefreshLobbyUI();
        UpdateHostButtonSelection();
    }

    private void PreparePlayersLobby(LocalPlayerInputController controller)
    {
        if (controller == null) return;
        
        SubscribePlayer(controller);
        LocalPlayerData player = controller.PlayerData;
        
        if (player == null) return;
        if (player.SlotIndex >= 0) return;
        
        int availableSlot = FindAvailableSlot();
        if (availableSlot < 0) return;
            
        player.AssignSlot(availableSlot);
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
        if (_localInputManager == null) return;
        _localInputManager.PlayerJoined += HandleInputPlayerJoined;
        _localInputManager.PlayerLeft += HandleInputPlayerLeft;
    }

    private void OnDisable()
    {
        if (_localInputManager != null)
        {
            _localInputManager.PlayerJoined -= HandleInputPlayerJoined;
            _localInputManager.PlayerLeft -= HandleInputPlayerLeft;
        }
        
        UnsubscribePlayersExisting();
    }

    private void HandleInputPlayerJoined(LocalPlayerInputController controller)
    {
        PreparePlayersLobby(controller);
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
        
        controller.SubmitRequested += HandleSubmitRequested;
        controller.CancelRequested += HandleCancelRequested;
        
        LocalPlayerData player = controller.PlayerData;

        if (player != null && player.PlayerIndex == 0) controller.NavigateRequested += HandleHostNavigateRequested;
    }
    
    private void UnsubscribePlayer(LocalPlayerInputController controller)
    {
        if (controller == null) return;
        if (!_subscribedPlayers.Remove(controller)) return;
        
        controller.SubmitRequested -= HandleSubmitRequested;
        controller.CancelRequested -= HandleCancelRequested;

        controller.NavigateRequested -= HandleHostNavigateRequested;
    }

    private void UnsubscribePlayersExisting()
    {
        LocalPlayerInputController[] controllers = new LocalPlayerInputController[_subscribedPlayers.Count];
        _subscribedPlayers.CopyTo(controllers);
        
        foreach (LocalPlayerInputController controller in controllers) UnsubscribePlayer(controller);
    }

    private void HandleSubmitRequested(LocalPlayerInputController inputController) //Cuando lo jugadores aceptan el slot en el que van a jugar
    {
        LocalPlayerData player = inputController.PlayerData;
        
        if (player == null) return;
        if (player.SlotIndex < 0) return;
        if (!player.IsReady)
        {
            player.SetReady(true);
            RefreshLobbyUI();
            if (player.PlayerIndex == 0) UpdateHostButtonSelection();
            return;
        }
        
        if (player.PlayerIndex == 0) InvokeSelectedHostButton();
    }

    private void HandleCancelRequested(LocalPlayerInputController inputController) //Cuando los jugadores cancelan el slot
    {
        LocalPlayerData player = inputController.PlayerData;
        
        if (player == null) return;
        if (!player.IsReady) return;
        
        player.SetReady(false);
        RefreshLobbyUI();
    }

    private void HandleHostNavigateRequested(LocalPlayerInputController inputController, int direction)
    {
        LocalPlayerData player = inputController.PlayerData;

        if (player == null) return;
        if (player.PlayerIndex != 0) return;
        if (!player.IsReady) return;
        if (_hostButtons == null || _hostButtons.Length == 0) return;

        int previousIndex = _selectedButtonIndex;

        do
        {
            _selectedButtonIndex += direction;

            if (_selectedButtonIndex < 0)
            {
                _selectedButtonIndex = _hostButtons.Length - 1;
            }
            else if (_selectedButtonIndex >= _hostButtons.Length)
            {
                _selectedButtonIndex = 0;
            }

            Button candidate = _hostButtons[_selectedButtonIndex];

            if (candidate != null && candidate.interactable) break;
            
        }
        while (_selectedButtonIndex != previousIndex);

        UpdateHostButtonSelection();
    }

    private void UpdateHostButtonSelection()
    {
        if (EventSystem.current == null) return;
        if (_hostButtons == null || _hostButtons.Length == 0) return;
        
        Button selectedButton = _hostButtons[_selectedButtonIndex];
        
        if (selectedButton == null) return;
        
        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
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
        if (_gameSet == null) return;
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
        
        UpdateBattleButton();
    }

    private int FindAvailableSlot()
    {
        for (int i = 0; i < playerSlots.Length; i++) if (!IsSlotOccupied(i)) return i;

        return -1;
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
        UpdateHostButtonSelection();
    }

    public bool CanContinue()
    {
        if (_gameSet == null) return false;
        if (_gameSet.Players.Count < 1) return false;

        foreach (LocalPlayerData player in _gameSet.Players)
        {
            if (player == null) return false;
            if (player.SlotIndex < 0) return false;
            if (!player.IsReady) return false;
        }
        return true;
    }

    private void InvokeSelectedHostButton()
    {
        if (_hostButtons == null || _hostButtons.Length == 0) return;
        
        Button selectedButton = _hostButtons[_selectedButtonIndex];
        
        if (selectedButton == null) return;
        if (!selectedButton.interactable) return;
        
        selectedButton.onClick.Invoke();
    }
}
