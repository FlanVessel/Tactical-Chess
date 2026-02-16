using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Drawing;

public enum MapType {Line, Rectangle, Circle, Triangle}

public class Map
{
    private MapType _type;
    private Vector2Int _origin;
    private Vector2Int _size;
    private Tilemap _tilemap;

    public Map(Vector2Int origin, Vector2Int size, Tilemap tilemap, MapType type)
    {
        _origin = origin;
        _size = size;
        _tilemap = tilemap;
        _type = type;
    }

    public List<Vector3Int> generateCoordinates()
    {
        List<Vector3Int> coordinates = new List<Vector3Int>();

        switch(_type)
        {
            //0
            case MapType.Line: 
                for (int i = 0; i < _size.x; i++)
                {
                    Vector3Int vector = new Vector3Int(i, i);
                    coordinates.Add(vector);
                }
                return coordinates;

            //1
            case MapType.Rectangle:
                for (int x = _origin.x; x < _size.x + _origin.x; x++)
                {
                    for (int y = _origin.y; y < _size.y + _origin.y; y++)
                    {
                        Vector3Int vector = new Vector3Int(x, y);
                        coordinates.Add(vector);
                    }
                }return coordinates;

            //2
            case MapType.Circle: 
                for (int x = _origin.x; x < _size.x + _origin.x; x++)
                {
                    for (int y = _origin.y; y < _size.y + _origin.y; y++)
                    {
                        Vector3Int vector = new Vector3Int(x, y, 0);
                        coordinates.Add(vector);
                    }
                }
                coordinates.Remove(new Vector3Int(_origin.x, _origin.y, 0));
                coordinates.Remove(new Vector3Int(_origin.x, (_size.y + _origin.y - 1), 0));
                coordinates.Remove(new Vector3Int((_size.x + _origin.x - 1), _origin.y, 0));
                coordinates.Remove(new Vector3Int((_size.x + _origin.x - 1), (_size.y + _origin.y - 1), 0));
                return coordinates;

            //3
            case MapType.Triangle:
                for (int x = 0; x < _size.x; x++)
                {
                    if (x % 2 == 0 && x != 0)
                    {
                        _size.y -= 2;
                        _origin.y++;
                    }

                    if (_size.y <= 0) break;

                    for (int y = 0; y < _size.y; y++)
                    {
                        coordinates.Add(new Vector3Int(_origin.x + x, _origin.y + y, 0));
                    }
                }
                return coordinates;

                /*for (int y = 0; y < _size.y; y++)
                {
                    for (int x = 0; x <= _size.x - y; x++)
                    {
                        Vector3Int vector = new Vector3Int(_origin.x + x, _origin.y + y, 0);
                        coordinates.Add(vector);
                    }
                }return coordinates;*/
        }
        return coordinates;
    }

    public void Render(List<Vector3Int> coordinates, Tile tile, Tilemap tilemap)
    {
        for (int i = 0; i < coordinates.Count; i++)
        {
            tilemap.SetTile(coordinates[i], tile);
        }
    }
}
