using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartSinglePlayer()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.StartSinglePlayerGame();
    }
    
    public void StartLocalMultiplayer()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.StartLocalMultiplayerGame();
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
