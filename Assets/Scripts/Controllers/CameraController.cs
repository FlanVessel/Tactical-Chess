using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField, Min(0.1f)]
    private float movementSpeed = 5f;

    [Header("Zoom")]
    [SerializeField, Min(0.1f)]
    private float zoomSpeed = 0.5f;

    [SerializeField, Min(0.1f)]
    private float minimumZoom = 2f;

    [SerializeField, Min(0.1f)]
    private float maximumZoom = 6f;

    private Camera gameCamera;
    private Bounds mapBounds;
    private bool initialized;

    private void Awake()
    {
        gameCamera = GetComponent<Camera>();
        if (gameCamera == null) Debug.LogError("CameraController necesita un componente Camera.");
    }

    public void Setup(Tilemap boardTilemap)
    {
        if (boardTilemap == null)
        {
            Debug.LogError("CameraController no recibió el Tilemap.");
            return;
        }

        TilemapRenderer tilemapRenderer = boardTilemap.GetComponent<TilemapRenderer>();

        if (tilemapRenderer == null)
        {
            Debug.LogError("El tablero no tiene TilemapRenderer.");
            return;
        }

        mapBounds = tilemapRenderer.bounds;
        initialized = true;

        ClampZoom();
        ClampPosition();
    }

    private void Update()
    {
        if (!initialized || gameCamera == null) return;

        Vector2 direction = ReadMovement();

        MoveCamera(direction);
        ResetClickCamera();
        HandleZoom();
        ClampPosition();
    }

    private void ResetClickCamera()
    {
        //Simplemente resetea la camara de acuerdo al valor de maxiumZoom
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) gameCamera.orthographicSize = maximumZoom;
    }

    private Vector2 ReadMovement()
    {
        //Obtenemos un valor vector2
        Vector2 direction = Vector2.zero;

        //Si detecta un click de teclado regresa el valor de vector 2
        if (Keyboard.current == null) return direction;

        //Se suma el valor del vector2 para mover la camara

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) direction.y += 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) direction.y -= 1f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) direction.x -= 1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) direction.x += 1f;

        return direction.normalized;
    }

    private void MoveCamera(Vector2 direction)
    {
        Vector3 movement = new Vector3(direction.x, direction.y, 0f);

        transform.position += movement * movementSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Approximately(scroll, 0f)) return;

        gameCamera.orthographicSize -= Mathf.Sign(scroll) * zoomSpeed;

        ClampZoom();
    }

    private void ClampZoom()
    {
        gameCamera.orthographicSize = Mathf.Clamp(gameCamera.orthographicSize, minimumZoom, maximumZoom);
    }

    private void ClampPosition()
    {
        float halfHeight = gameCamera.orthographicSize;

        float halfWidth = halfHeight * gameCamera.aspect;

        float minimumX = mapBounds.min.x + halfWidth;

        float maximumX = mapBounds.max.x - halfWidth;

        float minimumY = mapBounds.min.y + halfHeight;

        float maximumY = mapBounds.max.y - halfHeight;

        Vector3 position = transform.position;

        if (minimumX > maximumX)
        {
            position.x = mapBounds.center.x;
        }
        else
        {
            position.x = Mathf.Clamp(position.x, minimumX, maximumX);
        }

        if (minimumY > maximumY)
        {
            position.y = mapBounds.center.y;
        }
        else
        {
            position.y = Mathf.Clamp(position.y, minimumY, maximumY);
        }

        transform.position = position;
    }

    public void FocusOn(Transform target)
    {
        if (target == null) return;

        Vector3 position = target.position;

        position.z = transform.position.z;

        transform.position = position;

        ClampPosition();
    }
}
