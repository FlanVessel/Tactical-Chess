using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Control del Mapa")]
    [SerializeField] PerlingMapController perlingMapController;
    [SerializeField] PerlinBoardOccupancy perlinBoardOccupancy;

    private void Start()
    {
        LoadLevelMap();
    }

    private void LoadLevelMap()
    {
        perlingMapController.GenerateMap();
    }
}
