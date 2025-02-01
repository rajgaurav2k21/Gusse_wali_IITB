using UnityEngine;

public class CargoInteractable : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isPickedUp = false;
    
    private Rigidbody rb;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        if (!isPickedUp) 
        {
            isPickedUp = true;
            rb.isKinematic = true; // Stop physics while holding
        }
    }

    void OnMouseDrag()
    {
        if (isPickedUp)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 5f; // Adjust depth
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    void OnMouseUp()
    {
        if (isPickedUp)
        {
            isPickedUp = false;
            rb.isKinematic = false;

            // Check if placed correctly (you can add a condition for valid placement)
            if (!IsValidPlacement()) 
            {
                ReturnToOriginalPosition();
            }
        }
    }

    void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    bool IsValidPlacement()
    {
        // Define conditions for a valid drop (e.g., inside a truck area)
        return transform.position.y > 1f; // Example condition
    }
}
