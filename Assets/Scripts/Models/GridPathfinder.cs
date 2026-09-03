using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GridPathfinder
{
    private static readonly Vector3Int[] Directions = {Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down};

    public HashSet<Vector3Int> FindReachableCells(Tilemap boardTilemap, BoardOccupancy occupancy, Vector3Int start,int moveRange, Dictionary<Vector3Int, Vector3Int> cameFrom)
    {
        HashSet<Vector3Int> reachableCells = new();

        Queue<Vector3Int> pendingCells = new();
        Dictionary<Vector3Int, int> distances = new();

        cameFrom.Clear();

        pendingCells.Enqueue(start);
        distances[start] = 0;

        while (pendingCells.Count > 0)
        {
            Vector3Int current = pendingCells.Dequeue();
            int currentDistance = distances[current];

            foreach (Vector3Int direction in Directions)
            {
                Vector3Int next = current + direction;
                int nextDistance = currentDistance + 1;

                if (nextDistance > moveRange) continue;
                if (!boardTilemap.HasTile(next)) continue;
                if (distances.ContainsKey(next)) continue;
                if (occupancy.IsOccupied(next)) continue;

                distances[next] = nextDistance;
                cameFrom[next] = current;

                reachableCells.Add(next);
                pendingCells.Enqueue(next);
            }
        }

        return reachableCells;
    }

    public List<Vector3Int> BuildPath(Vector3Int start, Vector3Int destination, Dictionary<Vector3Int, Vector3Int> cameFrom)
    {
        List<Vector3Int> path = new();
        Vector3Int current = destination;

        path.Add(current);

        while (current != start)
        {
            if (!cameFrom.TryGetValue(current, out Vector3Int previous))return new List<Vector3Int>();

            current = previous;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}
