using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{
    [Header("Estadisticas")] 
    [SerializeField] private PerUnitStatsData pawnStatsData;
    [SerializeField] private PerUnitStatsData enemyStatsData;
    
    [Header("Prefabs Peones")]
    [SerializeField] private List<GameObject> pawnPrefabs;
    
    [Header("Prefabs Enemigos")]
    [SerializeField] private List<GameObject> enemysPrefabs;
    
    private PerlingMapController _mapController;
    private Tilemap _boardTilemap;
    private PerBoardOccupancy _perBoardOccupancy;
    
    public void ConfigSpawner(PerlingMapController mapController, PerBoardOccupancy perBoardOccupancy)
    {
        _mapController = mapController;
        _boardTilemap = mapController.BoardTilemap;
        _perBoardOccupancy = perBoardOccupancy;
    }

    public void SpawnAll()
    {
        List<Vector3Int> availableSpawnCells = _mapController.SpawnCell();
        List<Vector3Int> spawnableCells = _mapController.SpawnCellEnemy();
        
        SpawnPawns(availableSpawnCells);
        SpawnEnemies(spawnableCells);
    }

    private void SpawnPawns(List<Vector3Int> availableSpawnCells)
    {
        foreach (GameObject pawn in pawnPrefabs)
        {
            int randomNumber = Random.Range(0, availableSpawnCells.Count);
            
            Vector3Int selectedCell = availableSpawnCells[randomNumber];
            Vector3 worldPosition = _boardTilemap.GetCellCenterWorld(selectedCell);
            
            GameObject spawnedPawn = Instantiate(pawn, worldPosition, Quaternion.identity);

            PerUnitPawn unitPawn = spawnedPawn.GetComponent<PerUnitPawn>();
            unitPawn.Initialize(selectedCell, pawnStatsData);
            bool pawnRegistered = _perBoardOccupancy.RegisterUnit(unitPawn, selectedCell);
            
            availableSpawnCells.RemoveAt(randomNumber);
        }
    }

    private void SpawnEnemies(List<Vector3Int> spawnableCells)
    {
        foreach (GameObject enemy in enemysPrefabs)
        {
            int randomNumber = Random.Range(0, spawnableCells.Count);
            
            Vector3Int selectedCell = spawnableCells[randomNumber];
            Vector3 worldPosition = _boardTilemap.GetCellCenterWorld(selectedCell);
            
            GameObject spawnedEnemies = Instantiate(enemy, worldPosition, Quaternion.identity);

            PerUnitEnemy unitEnemy = spawnedEnemies.GetComponent<PerUnitEnemy>();
            unitEnemy.Initialize(selectedCell, enemyStatsData);
            bool enemyRegistered = _perBoardOccupancy.RegisterUnit(unitEnemy, selectedCell);
            
            spawnableCells.RemoveAt(randomNumber);
        }
    }
    
}
