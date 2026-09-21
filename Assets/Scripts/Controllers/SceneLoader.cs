using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("No se proporciono un nombre de escena.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"La escena '{sceneName}' no fue agregado.");
            return;
        }
        
        Debug.Log($"Cargando escena: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
