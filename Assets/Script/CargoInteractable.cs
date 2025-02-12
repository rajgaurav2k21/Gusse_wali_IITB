using UnityEngine;

public class CargoInteractable : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    private Material originalMaterial;
    public Material highlightMaterial;

    private Renderer objectRenderer;
    private Collider objectCollider;

    private void Start()
    {
        // ✅ Check and assign Renderer
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError($"🚨 No Renderer found on {gameObject.name}. Make sure the cargo prefab has a MeshRenderer.");
            return;
        }
        originalMaterial = objectRenderer.material;

        // ✅ Check and assign Collider
        objectCollider = GetComponent<Collider>();
        if (objectCollider == null)
        {
            gameObject.AddComponent<BoxCollider>(); // Automatically add a collider if missing
            Debug.LogWarning($"✅ BoxCollider added to {gameObject.name}.");
        }
    }

    private void OnMouseEnter()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = highlightMaterial;
        }
        Debug.Log($"🟢 Mouse entered: {gameObject.name}");
    }

    private void OnMouseExit()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial;
        }
        Debug.Log($"🔴 Mouse exited: {gameObject.name}");
    }

    private void OnMouseDown()
    {
        if (Camera.main == null) return;

        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
        Debug.Log($"👆 Picked up: {gameObject.name}");
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        transform.position = GetMouseWorldPosition() + offset;
        Debug.Log($"🛠 Dragging: {gameObject.name}");
    }

    private void OnMouseUp()
    {
        isDragging = false;
        SnapToGrid();
        Debug.Log($"🛑 Released: {gameObject.name}");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void SnapToGrid()
    {
        float gridSize = 1.2f; // Adjust based on cargo spacing
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Round(newPosition.x / gridSize) * gridSize;
        newPosition.y = Mathf.Round(newPosition.y / gridSize) * gridSize;
        newPosition.z = Mathf.Round(newPosition.z / gridSize) * gridSize;
        transform.position = newPosition;
    }
}
