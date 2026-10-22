using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Control del Mapa")]
    [SerializeField] PerlingMapController perlingMapController;
    [SerializeField] PlayerController playerController;
    
    [Header("Inicio del TileSelected")]
    [SerializeField] private Vector3Int initialCell;

    private void Start()
    {
        LoadLevelMap();
    }

    private void LoadLevelMap()
    {
        perlingMapController.GenerateMap();
        playerController.Config(perlingMapController, perlingMapController.SelectionTilemap, initialCell);
    }
}
