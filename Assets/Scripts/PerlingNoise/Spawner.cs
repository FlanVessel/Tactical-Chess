using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs Peones")]
    [SerializeField] private List<GameObject> pawnPrefabs;
    
    [Header("Prefabs Enemigos")]
    [SerializeField] private List<GameObject> enemysPrefabs;

    [SerializeField] private Vector3 unitOffset;
    
    private PerlingMapController _mapController;
    private Tilemap _boardTilemap;
    
    public void ConfigSpawner(PerlingMapController mapController)
    {
        _mapController = mapController;
        _boardTilemap = mapController.BoardTilemap;
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
            
            spawnableCells.RemoveAt(randomNumber);
        }
    }
    
}
