using UnityEngine;

public enum EmotionState{Neutral, Desperation, Determination}

public class EmotionController : MonoBehaviour
{
    [SerializeField,  Range(0, 100)] private int desperationPoints = 0;
    private EmotionState currentState = EmotionState.Neutral;

    public int DesperationPoints => desperationPoints;
    public int DeterminationPoints => 100 - desperationPoints;
    public EmotionState CurrentState => currentState;

    public EmotionState ResolveEmotion()
    {
        int randomResult = Random.Range(1, 101);

        if (randomResult <= desperationPoints)
        {
            currentState = EmotionState.Desperation;
        }
        else
        {
            currentState = EmotionState.Determination;
        }

        return currentState;
    } 

    public void AddDesperation(int amount)
    {
        desperationPoints = Mathf.Clamp(desperationPoints + amount, 0, 100);
    }

    public void AddDetermination(int amount)
    {
        desperationPoints = Mathf.Clamp(desperationPoints - amount, 0, 100);
    }
}
