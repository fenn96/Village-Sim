// CameraController2D.cs
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float panBorderThickness = 10f;
    
    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private float boundaryLeft = -50f;
    [SerializeField] private float boundaryRight = 50f;
    [SerializeField] private float boundaryBottom = -50f;
    [SerializeField] private float boundaryTop = 50f;
    
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }
    
    private void Update()
    {
        // Handle camera movement
        HandleMovement();
        
        // Handle camera zoom
        HandleZoom();
    }
    
    private void HandleMovement()
    {
        Vector3 position = transform.position;
        
        // WASD movement
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            position.y += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
        {
            position.y -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            position.x += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
        {
            position.x -= moveSpeed * Time.deltaTime;
        }
        
        // Apply boundaries
        if (useBoundaries)
        {
            position.x = Mathf.Clamp(position.x, boundaryLeft, boundaryRight);
            position.y = Mathf.Clamp(position.y, boundaryBottom, boundaryTop);
        }
        
        transform.position = position;
    }
    
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float orthographicSize = mainCamera.orthographicSize;
        
        orthographicSize -= scroll * zoomSpeed;
        orthographicSize = Mathf.Clamp(orthographicSize, minZoom, maxZoom);
        
        mainCamera.orthographicSize = orthographicSize;
    }
    
    // Set camera boundaries based on map size
    public void SetBoundaries(float width, float height)
    {
        boundaryLeft = -width / 2f;
        boundaryRight = width / 2f;
        boundaryBottom = -height / 2f;
        boundaryTop = height / 2f;
    }
    
    // Focus camera on a position
    public void FocusOn(Vector3 targetPosition)
    {
        Vector3 newPosition = targetPosition;
        newPosition.z = transform.position.z; // Keep the same Z position
        transform.position = newPosition;
    }
}