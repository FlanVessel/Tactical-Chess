using UnityEngine;

public class PawnRecruitmentController : MonoBehaviour
{
    private GameSet _gameSet;

    private void Awake()
    {
        if (GameManager.Instance == null) return;
        _gameSet = GameManager.Instance.Set;
        if (_gameSet == null) return;
    }

    private void Start()
    {
        InitialRecruit();
    }

    private void InitialRecruit()
    {
        if (_gameSet == null) return; 
        foreach (LocalPlayerData player in _gameSet.Players)
        {
            if (player == null) continue;
            Debug.Log($"Jugador {player.SlotIndex + 1}: {player.DeviceName}.");
        }
    }
}
