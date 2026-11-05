using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Referencias para cargar el nivel")]
    [SerializeField] private PerlingMapController perlingMapController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Spawner spawnerController;
    [SerializeField] private PerBoardOccupancy perBoardOccupancy;
    
    [Header("Inicio del TileSelected")]
    [SerializeField] private Vector3Int initialCell;

    private void Start()
    {
        LoadLevelMap();
    }

    private void LoadLevelMap()
    {
        perlingMapController.GenerateMap();
        
        playerController.ConfigPlayer(perlingMapController, perlingMapController.SelectionTilemap, initialCell, perBoardOccupancy);
        
        spawnerController.ConfigSpawner(perlingMapController, perBoardOccupancy);
        spawnerController.SpawnAll();
    }
}
