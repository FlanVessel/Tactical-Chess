using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PawnRecruitmentController : MonoBehaviour
{
    private GameSet _gameSet;
    private LocalInputManager _localInputManager;
    private PawnNameProvider _nameProvider;
    
    [Header("Peon Random")]
    [SerializeField] private UnitData commonPawnData;
    [SerializeField] private PawnNameData pawnNameData;
    
    [Header("Interfaz")]
    [SerializeField] private PawnRecruitmentSlotUI[] recruitmentSlots;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button returnLocalButton;

    private Button[] _hostButtons;
    
    private readonly Dictionary<uint, PawnCandidateData> _currentCandidates = new();
    private readonly HashSet<LocalPlayerInputController> _subPlayers = new();

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PawnRecruitment necesita iniciar desde Bootstrap.");
            return;
        }

        _gameSet = GameManager.Instance.Set;
        _localInputManager = LocalInputManager.Instance;
        _nameProvider = new PawnNameProvider(pawnNameData);
    }

    private void Start()
    {
        InitializeRecruitmentSlots();

        _hostButtons = new Button[] { returnLocalButton, continueButton };
        
        if (_gameSet == null || _localInputManager == null)
        {
            Debug.LogError("No se encontraron los sistemas de sesion e input.");
            return;
        }

        _localInputManager.DisableJoining();

        foreach (LocalPlayerInputController controller in _localInputManager.Players)
        {
            SubscribePlayer(controller);
            
            LocalPlayerData player = controller.PlayerData;

            if (player == null) continue;
            
            GeneratedCandidate(player);
        }

        Debug.Log($"Reclutamiento iniciado con " + $"{_localInputManager.Players.Count} jugadores.");
    }

    private void InitializeRecruitmentSlots()
    {
        for (int i = 0; i < recruitmentSlots.Length; i++)
        {
            PawnRecruitmentSlotUI slot =  recruitmentSlots[i];

            if (slot == null) continue;
            slot.Initialize(i);
            slot.ChangeRequested += HandleSlotChangeRequested;
            slot.ReadyRequested += HandleSlotReadyRequested;
        }
    }

    private void SubscribePlayer(LocalPlayerInputController controller)
    {
        if (controller == null) return;
        if (!_subPlayers.Add(controller)) return;

        controller.NavigateRequested += HandleNavigateRequested;
        controller.SubmitRequested += HandleSubmitRequested;
        controller.CancelRequested += HandleCancelRequested;
    }

    private void UnsubscribePlayer(LocalPlayerInputController controller)
    {
        if (controller == null) return;
        if (!_subPlayers.Remove(controller)) return;

        controller.NavigateRequested -= HandleNavigateRequested;
        controller.SubmitRequested -= HandleSubmitRequested;
        controller.CancelRequested -= HandleCancelRequested;
    }

    private void HandleNavigateRequested(LocalPlayerInputController controller, int direction)
    {
        LocalPlayerData player = controller.PlayerData;

        if (player == null) return;
        if (player.RecruitedPawn != null) return;

        Debug.Log($"Jugador {player.PlayerIndex + 1} solicita " + $"{direction} peon.");
    }

    private void HandleSubmitRequested(LocalPlayerInputController controller)
    {
        LocalPlayerData player = controller.PlayerData;
        ConfirmCandidate(player);
    }

    private void HandleCancelRequested(LocalPlayerInputController controller)
    {
        LocalPlayerData player = controller.PlayerData;

        if (player == null) return;
        if (player.RecruitedPawn == null) return;
        
        player.ClearRecruitedPawn();
        
        if (!_currentCandidates.TryGetValue(player.InputUserId, out PawnCandidateData candidateData)) return;
        
        PawnRecruitmentSlotUI slot = GetSlotPlayer(player);
        
        if (slot != null) slot.ShowCandidate(player, candidateData);

        Debug.Log($"Jugador {player.PlayerIndex + 1} " + $"cancela su seleccion.");
    }

    private void OnDisable()
    {
        LocalPlayerInputController[] controllers = new LocalPlayerInputController[_subPlayers.Count];

        _subPlayers.CopyTo(controllers);

        foreach (LocalPlayerInputController controller in controllers) UnsubscribePlayer(controller);

        foreach (PawnRecruitmentSlotUI slot in recruitmentSlots)
        {
            if (slot == null) continue;
            
            slot.ChangeRequested -= HandleSlotChangeRequested;
            slot.ReadyRequested -= HandleSlotReadyRequested;
        }
    }

    private void GeneratedCandidate(LocalPlayerData player)
    {
        if (player == null) return;
        if (commonPawnData == null) return;
        if (_nameProvider == null) return;

        string generateName = _nameProvider.GetNextName();
        
        PawnCandidateData candidateData = PawnCandidateGenerator.Generate(commonPawnData, generateName);
        
        if (candidateData == null) return;

        _currentCandidates[player.InputUserId] = candidateData;
        
        PawnRecruitmentSlotUI slot = GetSlotPlayer(player);
        
        if (slot != null) slot.ShowCandidate(player, candidateData);
    }

    private PawnRecruitmentSlotUI GetSlotPlayer(LocalPlayerData player)
    {
        if (player == null) return null;

        int slotIndex = player.SlotIndex;
        
        if (slotIndex < 0 || slotIndex >= recruitmentSlots.Length) slotIndex = player.PlayerIndex;
        if (slotIndex < 0 || slotIndex >= recruitmentSlots.Length) return null;
        return recruitmentSlots[slotIndex];
    }

    private LocalPlayerData FindPlayerBySlot(int slotIndex)
    {
        foreach (LocalPlayerData player in _gameSet.Players)
        {
            if (player == null) continue;
            if (player.SlotIndex == slotIndex) return player;
        }
        return null;
    }

    private void HandleSlotChangeRequested(int slotIndex)
    {
        LocalPlayerData player = FindPlayerBySlot(slotIndex);
        
        if (player == null) return;
        if (player.RecruitedPawn != null) return;
        
        GeneratedCandidate(player);
    }

    private void HandleSlotReadyRequested(int slotIndex)
    {
        LocalPlayerData player = FindPlayerBySlot(slotIndex);
        
        ConfirmCandidate(player);
    }

    private void ConfirmCandidate(LocalPlayerData player)
    {
        if (player == null) return;
        if (!_currentCandidates.TryGetValue(player.InputUserId, out PawnCandidateData candidateData)) return;
        
        player.RecruitPawn(candidateData);
        
        PawnRecruitmentSlotUI slot = GetSlotPlayer(player); 
        if (slot != null) slot.ShowConfirmed(player, candidateData);
    }

    private void UpdateButtonContinue()
    {
        if (continueButton == null) return;
        continueButton.interactable = CanContinue();
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
