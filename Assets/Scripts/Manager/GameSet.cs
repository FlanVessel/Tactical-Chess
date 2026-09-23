using UnityEngine;
using System.Collections.Generic;

public enum GameMode{None, SinglePlayer, LocalMultiPlayer}

public class GameSet : MonoBehaviour
{
    private const int MaxiumLocalPlayerCount = 4;
    
    private readonly List<LocalPlayerData> _players = new();
    
    public GameMode CurrentMode { get; private set; } = GameMode.None;
    
    public IReadOnlyList<LocalPlayerData> Players => _players;
    public int PlayerCount => _players.Count;
    
    public void StartSingleplayerSet()
    {
        ResetSet();
        CurrentMode = GameMode.SinglePlayer;
    }

    public void StartLocalMultiplayerSet()
    {
        ResetSet();
        CurrentMode = GameMode.LocalMultiPlayer;
        Debug.Log("Sesion Multijugadorlocal creado");
    }

    public bool TryAddLocalPlayer(int playerIndex,uint inputUserId, string deviceName, string controlScheme, out LocalPlayerData newPlayerData)
    {
        newPlayerData = null;
        
        if (CurrentMode != GameMode.LocalMultiPlayer) return false;
        if (_players.Count >= MaxiumLocalPlayerCount) return false;
        if (ContainsInputUser(inputUserId)) return false;
        
        newPlayerData = new LocalPlayerData(playerIndex, inputUserId, deviceName, controlScheme);
        
        _players.Add(newPlayerData);
        return true;
    }

    public bool RemoveLocalPlayer(uint inputUserId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].InputUserId != inputUserId) continue;
            _players.RemoveAt(i);
            return true;
        }
        return false;
    }

    public bool AreAllPlayersReady()
    {
        if (_players.Count == 0) return false;

        foreach (LocalPlayerData playerData in _players)
        {
            if (!playerData.IsReady) return false;
        }
        return true;
    }

    public void ResetSet()
    {
        _players.Clear();
        CurrentMode = GameMode.None;
    }

    private bool ContainsInputUser(uint inputUserId)
    {
        foreach (LocalPlayerData playerData in _players)
        {
            if (playerData.InputUserId == inputUserId) return true;
        }
        return false;
    }
}
