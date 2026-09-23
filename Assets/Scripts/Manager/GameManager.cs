using UnityEngine;
using System;
using Unity.VectorGraphics;

public enum GameState{Boot, MainMenu, LocalLobby, PawnRecruitment, ClassSelection, CampaignSelection, Battle, Results}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private SceneLoader _sceneLoader;
    private GameSet _gameSet;
    
    private GameState _currentGameState = GameState.Boot;
    public GameState CurrentGameState => _currentGameState;
    public GameSet Set => _gameSet;
    
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
        
        _sceneLoader = GetComponent<SceneLoader>();
        if (_sceneLoader == null) Debug.LogError("GameManager no tiene un componente SceneLoader");
        
        _gameSet = GetComponent<GameSet>();
        if (_gameSet == null) Debug.LogError("GameManager no tiene un componente GameSet");
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
    
    public void StartSinglePlayerGame()
    {
        if (_gameSet == null) return;
        
        _gameSet.StartSingleplayerSet();
        
        ChangeState(GameState.Battle);
        LoadScene("TestMapProcedural");
    }
    
    public void StartLocalMultiplayerGame()
    {
        if (_gameSet == null) return;
        
        _gameSet.StartLocalMultiplayerSet();
        
        ChangeState(GameState.LocalLobby);
        LoadScene("LocalLobby");
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
