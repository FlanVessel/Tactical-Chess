using UnityEngine;

public class LocalMenuUI : MonoBehaviour
{
    public void ReturnMenu()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.ReturnToMenu();
    }
}
