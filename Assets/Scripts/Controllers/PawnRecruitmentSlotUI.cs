using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PawnRecruitmentSlotUI : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject informationPanel;

    [Header("Identidad")]
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text deviceText;
    [SerializeField] private TMP_Text pawnNameText;
    [SerializeField] private Image pawnImage;

    [Header("Información")]
    [SerializeField] private TMP_Text maxHealthText;
    [SerializeField] private TMP_Text movementText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text manaRecoveryText;

    [Header("Botones principales")]
    [SerializeField] private Button informationButton;
    [SerializeField] private Button changeButton;
    [SerializeField] private Button readyButton;

    [Header("Botón de información")]
    [SerializeField] private Button closeInformationButton;

    [Header("Estado")]
    [SerializeField] private TMP_Text statusText;

    private int _slotIndex;
    private PawnCandidateData _currentCandidate;

    public int SlotIndex => _slotIndex;
    public PawnCandidateData CurrentCandidate => _currentCandidate;

    public event Action<int> ChangeRequested;
    public event Action<int> ReadyRequested;

    private void Awake()
    {
        if (informationButton != null) informationButton.onClick.AddListener(ShowInformation);
        
        if (closeInformationButton != null) closeInformationButton.onClick.AddListener(HideInformation);

        if (changeButton != null) changeButton.onClick.AddListener(NotifyChangeRequested);
        
        if (readyButton != null) readyButton.onClick.AddListener(NotifyReadyRequested);
        
    }

    public void Initialize(int slotIndex)
    {
        _slotIndex = slotIndex;
        _currentCandidate = null;

        HideInformation();
        gameObject.SetActive(false);
    }

    public void ShowCandidate(LocalPlayerData player, PawnCandidateData candidate)
    {
        if (player == null || candidate == null) return;

        _currentCandidate = candidate;

        gameObject.SetActive(true);

        if (mainPanel != null) mainPanel.SetActive(true);

        if (informationPanel != null) informationPanel.SetActive(false);
        
        if (playerText != null) playerText.text = $"Jugador {player.PlayerIndex + 1}";
        if (deviceText != null) deviceText.text = player.DeviceName;
        if (pawnNameText != null) pawnNameText.text = candidate.PawnName;

        if (pawnImage != null)
        {
            pawnImage.sprite = candidate.PawnSprite;
            pawnImage.enabled = candidate.PawnSprite != null;
            pawnImage.preserveAspect = true;
        }

        UpdateInformation(candidate);

        if (changeButton != null) changeButton.interactable = true;
        if (readyButton != null) readyButton.interactable = true;
        
        if (statusText != null)
        {
            statusText.text = "Eligiendo peón";
            statusText.color = Color.white;
        }
    }

    public void ShowConfirmed(LocalPlayerData player, PawnCandidateData candidate)
    {
        ShowCandidate(player, candidate);

        if (changeButton != null) changeButton.interactable = false;
        if (readyButton != null) readyButton.interactable = false;
        
        if (statusText != null)
        {
            statusText.text = "LISTO";
            statusText.color = Color.green;
        }
    }

    public void ShowInformation()
    {
        if (_currentCandidate == null) return;
        if (mainPanel != null) mainPanel.SetActive(false);
        if (informationPanel != null) informationPanel.SetActive(true);
    }

    public void HideInformation()
    {
        if (informationPanel != null) informationPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    private void UpdateInformation(PawnCandidateData candidate)
    {
        if (maxHealthText != null) maxHealthText.text = $"Vida máxima: {candidate.MaxHealth}";
        if (movementText != null) movementText.text = $"Movimiento: {candidate.MoveRange}";
        if (manaText != null) manaText.text = $"Maná máximo: {candidate.MaxMana}";
        if (manaRecoveryText != null) manaRecoveryText.text = $"Recuperación de maná: " + $"{candidate.ManaRecovery}";
    }

    private void NotifyChangeRequested()
    {
        ChangeRequested?.Invoke(_slotIndex);
    }

    private void NotifyReadyRequested()
    {
        ReadyRequested?.Invoke(_slotIndex);
    }

    private void OnDestroy()
    {
        if (informationButton != null) informationButton.onClick.RemoveListener(ShowInformation);
        if (closeInformationButton != null) closeInformationButton.onClick.RemoveListener(HideInformation);
        if (changeButton != null) changeButton.onClick.RemoveListener(NotifyChangeRequested);
        if (readyButton != null) readyButton.onClick.RemoveListener(NotifyReadyRequested);
    }
}
