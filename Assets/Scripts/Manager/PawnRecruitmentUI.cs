using UnityEngine;

public class PawnRecruitmentUI : MonoBehaviour
{
    public void ReturnToLocalLobby()
    {
        if (LocalInputManager.Instance != null) LocalInputManager.Instance.ClearLocalPlayers();
        
        GameManager.Instance.ChangeState(GameState.LocalLobby);
        GameManager.Instance.ReturnToLocalLobby();
    }
}
