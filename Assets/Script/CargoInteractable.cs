using UnityEngine;
using System.Collections;

public class CargoInteractable : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 originalPosition;

    private Rigidbody rb;
    private Renderer objectRenderer;
    public Material highlightMaterial;
    private Material originalMaterial;

    private bool isSupported = true; // Checks if the box has support underneath

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Make it kinematic while dragging
        }

        // Assign Renderer
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError($"No Renderer found on {gameObject.name}. Add a MeshRenderer.");
            return;
        }
        originalMaterial = objectRenderer.material;

        originalPosition = transform.position;
    }

    private void OnMouseEnter()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = highlightMaterial;
        }
    }

    private void OnMouseExit()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }

    private void OnMouseDown()
    {
        if (Camera.main == null) return;

        isDragging = true;
        rb.isKinematic = true; // Disable physics while dragging
        offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        transform.position = GetMouseWorldPosition() + offset;
    }

    private void OnMouseUp()
    {
        isDragging = false;
        rb.isKinematic = false; // Enable physics again

        StartCoroutine(CheckIfFloating());
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // ✅ Checks if the box is floating & moves it back if needed
    private IEnumerator CheckIfFloating()
    {
        yield return new WaitForSeconds(0.5f); // Wait for physics to settle

        if (!IsBoxSupported())
        {
            Debug.Log("Box is floating! Returning to original position.");
            StartCoroutine(MoveBackSmoothly());
        }
    }

    private bool IsBoxSupported()
    {
        RaycastHit hit;
        // Cast a ray downwards to check for ground or other boxes
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.2f))
        {
            if (hit.collider.CompareTag("Cargo") || hit.collider.CompareTag("Ground"))
            {
                return true; // Supported by another box or ground
            }
        }
        return false; // Floating in the air
    }

    private IEnumerator MoveBackSmoothly()
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, originalPosition, time);
            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.position = originalPosition;
    }
}
