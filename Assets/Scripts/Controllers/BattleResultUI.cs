using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    private TurnManager _turnManager;

    public void Setup(TurnManager turnManager)
    {
        if (_turnManager != null)
        {
            _turnManager.PhaseChanged -= HandlePhaseChanged;
        }

        _turnManager = turnManager;

        if (_turnManager == null)
        {
            Debug.LogError(
                "BattleResultUI no recibió TurnManager."
            );

            return;
        }

        _turnManager.PhaseChanged += HandlePhaseChanged;

        gameObject.SetActive(false);
    }

    private void HandlePhaseChanged(BattlePhase phase)
    {
        if (phase == BattlePhase.Victory)
        {
            ShowResult("VICTORIA");
            return;
        }

        if (phase == BattlePhase.Defeat)
        {
            ShowResult("DERROTA");
        }
    }

    private void ShowResult(string message)
    {
        gameObject.SetActive(true);

        if (resultText != null)
        {
            resultText.text = message;
        }
    }

    public void RestartLevel()
    {
        Scene currentScene =
            SceneManager.GetActiveScene();

        SceneManager.LoadScene(
            currentScene.buildIndex
        );
    }

    private void OnDestroy()
    {
        if (_turnManager != null)
        {
            _turnManager.PhaseChanged -= HandlePhaseChanged;
        }
    }
}
