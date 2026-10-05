using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LocalMenuUI : MonoBehaviour
{
    [SerializeField] private LocalLobbyController lobbyController;
    [SerializeField] private Button selectionButton;

    private void Start()
    {
        
    }

    public void ContinueButton()
    {
        if (lobbyController == null) return;
        if (!lobbyController.CanContinue()) return;
        if (GameManager.Instance == null) return;
        GameManager.Instance.StartPawnRecruitment();
    }
    
    public void ReturnMenu()
    {
        if (LocalInputManager.Instance != null) LocalInputManager.Instance.ClearLocalPlayers();
        
        GameManager.Instance.ChangeState(GameState.MainMenu);
        GameManager.Instance.ReturnToMenu();
    }

    private void SelectButtons()
    {
        EventSystem.current.SetSelectedGameObject(selectionButton.gameObject);
    }
}
