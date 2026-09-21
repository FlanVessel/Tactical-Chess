using UnityEngine;
using System;
using Unity.VectorGraphics;

public enum GameState{Boot, MainMenu, LocalLobby, PawnRecruitment, ClassSelection, CampaignSelection, Battle, Results}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private SceneLoader _sceneLoader;
    
    private GameState _currentGameState = GameState.Boot;
    public GameState CurrentGameState => _currentGameState;
    
    public event Action<GameState> StateChanged;
    
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        _sceneLoader  = GetComponent<SceneLoader>();
        
        if (_sceneLoader == null) Debug.LogError("GameManager no tiene un componente SceneLoader");
    }

    private void Start()
    {
        OpenMainMenu();
    }

    public void ChangeState(GameState newState)
    {
        if (_currentGameState == newState) return;
        
        _currentGameState = newState;
        Debug.Log($"GameState es: {_currentGameState}");
        StateChanged?.Invoke(_currentGameState);
    }
    
    public void OpenMainMenu()
    {
        ChangeState(GameState.MainMenu);
        LoadScene("MainMenu");
    }

    public void OpenLocalLobby()
    {
        ChangeState(GameState.LocalLobby);
        LoadScene("LocalLobby");
    }

    public void OpenBattle()
    {
        ChangeState(GameState.Battle);
        LoadScene("TestMapProcedural");
    }

    private void LoadScene(string sceneName)
    {
        if (_sceneLoader == null)
        {
            Debug.LogError("No se puede cargar la escena porque falta SceneLoader.");
            return;
        }

        _sceneLoader.LoadScene(sceneName);
    }
}
