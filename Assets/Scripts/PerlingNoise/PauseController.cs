using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [Header("Menu de Pausa")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button continueButton;

    private PlayerInput _playerInput;

    private InputActionMap _systemActionMap;
    private InputAction _pauseAction;
    
    private bool _isPaused;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _systemActionMap = _playerInput.actions.FindActionMap("System");
        
        _pauseAction = _playerInput.actions.FindAction("Pause");
        
        pauseMenu.SetActive(false);
    }

    private void OnEnable()
    {
        _systemActionMap.Enable();
        _pauseAction.performed += Pause;
    }

    private void OnDisable()
    {
        _pauseAction.performed -= Pause;
        _systemActionMap.Disable();
    }

    private void Pause(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
        {
            _isPaused = false;
            pauseMenu.SetActive(false);
        }
        else
        {
            _isPaused = true;
            pauseMenu.SetActive(true);
            _playerInput.SwitchCurrentActionMap("UI");
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }
    }
    
    public void LoadScene()
    {
        SceneManager.LoadScene("PerlinScene");
    }
    
}
