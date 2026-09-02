using UnityEngine;

[System.Serializable]
public class UnitSpawnConfig
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2Int spawnCell;

    public GameObject Prefap => prefab;
    public Vector2Int SpawnCell => spawnCell;
}
