using UnityEngine;

public class PerUnit : MonoBehaviour
{
    private Vector3Int _currentCell;
    private bool _initialized;
    
    public Vector3Int CurrentCell => _currentCell;
    public bool IsInitialized => _initialized;

    public virtual void Initialize(Vector3Int initialCell)
    {
        _currentCell = initialCell;
        _initialized = true;
    }
    
    public void SetCurrentCell(Vector3Int newCell)
    {
        _currentCell = newCell;
    }
}
