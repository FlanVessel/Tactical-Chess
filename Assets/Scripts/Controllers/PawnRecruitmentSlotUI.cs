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
    
    [SerializeField] private Button continueButton;
    [SerializeField] private Button returnButton;

    [Header("Botón de información")]
    [SerializeField] private Button closeInformationButton;

    [Header("Estado")]
    [SerializeField] private TMP_Text statusText;
    
    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.red;
    
    private Button[] _mainButtons;
    private int _selectedButtonIndex;
    private bool _informationButtonActive;
    private bool _confirmed;

    private int _slotIndex;
    private PawnCandidateData _currentCandidate;

    public int SlotIndex => _slotIndex;
    public PawnCandidateData CurrentCandidate => _currentCandidate;
    public bool IsInformationActive => _informationButtonActive;
    public bool IsConfirmed => _confirmed;

    public event Action<int> ChangeRequested;
    public event Action<int> ReadyRequested;

    private void Awake()
    {
        _mainButtons = new Button[] { informationButton, changeButton, readyButton };
        
        if (informationButton != null) informationButton.onClick.AddListener(ShowInformation);
        if (closeInformationButton != null) closeInformationButton.onClick.AddListener(HideInformation);
        if (changeButton != null) changeButton.onClick.AddListener(NotifyChangeRequested);
        if (readyButton != null) readyButton.onClick.AddListener(NotifyReadyRequested);
    }

    public void Initialize(int slotIndex)
    {
        _slotIndex = slotIndex;
        _currentCandidate = null;
        _selectedButtonIndex = 0;
        _informationButtonActive = false;
        _confirmed = false;
        
        gameObject.SetActive(true);
        
        if (mainPanel != null) mainPanel.SetActive(false);
        if (informationPanel != null) informationPanel.SetActive(false);
        
        RefreshButtonSelection();
    }

    public void ShowCandidate(LocalPlayerData player, PawnCandidateData candidate)
    {
        Debug.Log($"ShowCandidate ejecutando: jugador {player?.PlayerIndex}, " +  $"peon {candidate?.PawnName}");
        if (player == null || candidate == null) return;

        _currentCandidate = candidate;
        _confirmed = false;
        _informationButtonActive = false;

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

        if (informationButton != null) informationButton.interactable = true;
        if (changeButton != null) changeButton.interactable = true;
        if (readyButton != null) readyButton.interactable = true;
        
        if (statusText != null)
        {
            statusText.text = "Eligiendo peón";
            statusText.color = Color.white;
        }
        
        RefreshButtonSelection();
    }

    public void ShowConfirmed(LocalPlayerData player, PawnCandidateData candidate)
    {
        ShowCandidate(player, candidate);
        _confirmed = true;

        if (informationButton != null) informationButton.interactable = false;
        if (changeButton != null) changeButton.interactable = false;
        if (readyButton != null) readyButton.interactable = false;
        
        if (statusText != null)
        {
            statusText.text = "LISTO";
            statusText.color = Color.green;
        }

        SelectFirstButton();
    }

    private void SelectFirstButton()
    {
        if (_mainButtons == null) return;

        for (int i = 0; i < _mainButtons.Length; i++)
        {
            Button button = _mainButtons[i];
            
            if (button == null) continue;
            if (!button.interactable) continue;

            _selectedButtonIndex = 1;
            RefreshButtonSelection();
            return;
        }
        ClearButtonSelection();
    }

    public void ShowInformation()
    {
        if (_currentCandidate == null) return;
        if (_confirmed) return;
        
        _informationButtonActive = true;
        
        if (mainPanel != null) mainPanel.SetActive(false);
        if (informationPanel != null) informationPanel.SetActive(true);
    }

    public void HideInformation()
    {
        _informationButtonActive = false;
        
        if (mainPanel != null) mainPanel.SetActive(true);
        if (informationPanel != null) informationPanel.SetActive(false);
        
        RefreshButtonSelection();
    }

    private void ClearButtonSelection()
    {
        if (_mainButtons == null) return;

        foreach (Button button in _mainButtons)
        {
            if (button == null) continue;
            if (button.targetGraphic == null) continue;
            button.targetGraphic.color = normalButtonColor;
        }
    }

    public bool TryCloseInformation()
    {
        if (!_informationButtonActive) return false;
        
        HideInformation();
        return true;
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

    public void Navigate(int direction)
    {
        if (_informationButtonActive) return;
        if (_mainButtons == null || _mainButtons.Length == 0) return;

        int previousIndex = _selectedButtonIndex;

        do
        {
            _selectedButtonIndex -= direction;
            if (_selectedButtonIndex < 0)
            {
                _selectedButtonIndex = _mainButtons.Length - 1;
            }
            else if (_selectedButtonIndex >= _mainButtons.Length)
            {
                _selectedButtonIndex = 0;
            }
            
            Button candidate = _mainButtons[_selectedButtonIndex];
            
            if (candidate != null && candidate.interactable) break;
        }
        while (_selectedButtonIndex != previousIndex);

        RefreshButtonSelection();
    }

    public void SubmitSelection()
    {
        if (_informationButtonActive)
        {
            HideInformation();
            return;
        }
        if (_mainButtons == null || _mainButtons.Length == 0) return;
        
        Button selectedButton = _mainButtons[_selectedButtonIndex];

        if (selectedButton == null) return;
        if (!selectedButton.interactable) return;
        
        selectedButton.onClick.Invoke();
    }

    private void RefreshButtonSelection()
    {
        if (_mainButtons == null) return;

        for (int i = 0; i < _mainButtons.Length; i++)
        {
            Button button = _mainButtons[i];
            
            if (button == null) continue;
            if (button.targetGraphic == null) continue;
            
            button.targetGraphic.color = i == _selectedButtonIndex ? selectedButtonColor : normalButtonColor;
        }
    }

    public void ConfigureHostButtons(Button returnButton, Button continueButton)
    {
        _mainButtons = new Button[] {informationButton, changeButton, readyButton, returnButton, continueButton};
    }
}
