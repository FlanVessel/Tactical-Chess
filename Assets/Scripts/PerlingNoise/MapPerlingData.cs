using UnityEngine;
using System.Collections.Generic;

public enum TerrainType {Tierra, Agua, Montana, Obstaculo}

public class MapPerlingData
{
    private Vector2Int _origin;
    private Vector2Int _size;

    private int seed = Random.Range(0, 1000000);
    private float noiseScale = 0.50f;
    private float offsetX;
    private float offsetY;

    public MapPerlingData(Vector2Int origin, Vector2Int size)
    {
        _origin = origin;
        _size = size;
    }

    public List<Vector3Int> GenerateCoordinates()
    {
        List<Vector3Int> coordinates = new List<Vector3Int>();

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(_origin.x + x, _origin.y + y, 0);

                coordinates.Add(cellPosition);
            }
        }

        return coordinates;
    }

    public Dictionary<Vector3Int, TerrainType> GenerateTerrain()
    {
        System.Random rnd = new System.Random(seed);
        offsetX = rnd.Next(-100000,100000);
        offsetY = rnd.Next(-100000,100000);
        
        Dictionary<Vector3Int, TerrainType> terrain = new Dictionary<Vector3Int, TerrainType>();
        
        foreach (Vector3Int cellPosition in GenerateCoordinates())
        {
            float noiseValue = Mathf.PerlinNoise((cellPosition.x + offsetX) * noiseScale, (cellPosition.y + offsetY) * noiseScale);
            TerrainType terrainType = ClassifyTerrain(noiseValue);
            terrain[cellPosition] = terrainType;
        }
        return terrain;
    }

    private TerrainType ClassifyTerrain(float noise)
    {
        if (noise < 0.30f) return TerrainType.Tierra;
        if (noise < 0.55f) return TerrainType.Agua;
        if (noise < 0.75f) return TerrainType.Montana;
        return TerrainType.Obstaculo;
    }
}
