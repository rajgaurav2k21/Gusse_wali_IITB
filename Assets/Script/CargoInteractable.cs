using UnityEngine;

public class DragDropCargo : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 offset;
    private float zCoord;

    void OnMouseDown()
    {
        // Store the original position before dragging
        originalPosition = transform.position;
        
        // Get the distance from camera to object
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        
        // Calculate the offset between mouse position and object position
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        // Move the object while dragging
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        // Check if the cargo is placed on another object
        if (IsOverlapping())
        {
            // If overlapping, reset to original position
            transform.position = originalPosition;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        // Get the current mouse position in world space
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord; // Maintain original depth
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private bool IsOverlapping()
    {
        // Cast a small box (or sphere) at the cargo's position to check for collisions
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
        
        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject) // Ignore itself
            {
                return true; // Collision detected
            }
        }
        return false; // No collision
    }
}
