using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Control del Mapa")]
    [SerializeField] PerlingMapController perlingMapController;
    [SerializeField] PlayerController playerController;

    private void Start()
    {
        LoadLevelMap();
    }

    private void LoadLevelMap()
    {
        perlingMapController.GenerateMap();
    }
}
