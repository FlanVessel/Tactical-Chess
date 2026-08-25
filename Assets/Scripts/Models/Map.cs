using UnityEngine;
using System.Collections.Generic;

public class Map
{
    private Vector2Int _origin;
    private Vector2Int _size;

    public Map(Vector2Int origin, Vector2Int size)
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

}
