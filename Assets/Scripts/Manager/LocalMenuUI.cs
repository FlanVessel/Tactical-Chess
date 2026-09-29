using UnityEngine;
using UnityEngine.Tilemaps;

public class LocalMenuUI : MonoBehaviour
{
    [SerializeField] private LocalLobbyController lobbyController;

    public void ContinueButton()
    {
        if (lobbyController == null) return;
        if (!lobbyController.CanContinue()) return;
        if (GameManager.Instance == null) return;
        GameManager.Instance.StartPawnRecruitment();
    }
    
    public void ReturnMenu()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.ReturnToMenu();
    }
}
