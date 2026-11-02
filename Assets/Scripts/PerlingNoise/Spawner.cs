using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private Vector3 unitOffset;

    private PerlingMapController _mapController;
    
    public void ConfigSpawner(PerlingMapController mapController)
    {
        _mapController = mapController;
    }

    public void SpawnFirstPawn()
    {
        List<Vector3Int> spawnableCells = _mapController.SpawnCell();

        int randomNumber = Random.Range(0, spawnableCells.Count);
        
        Vector3Int selectedCell = spawnableCells[randomNumber];

        Tilemap boardTilemap = _mapController.BoardTilemap;
        Vector3 worldPosition = boardTilemap.GetCellCenterWorld(selectedCell);
        
        GameObject pawn = Instantiate(pawnPrefab, worldPosition + unitOffset, Quaternion.identity);
        
    }
}
