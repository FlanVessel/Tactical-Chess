using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartSinglePlayerGame()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OpenBattle();
    }
    
    public void StartLocalMultiplayerGame()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OpenLocalLobby();
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
