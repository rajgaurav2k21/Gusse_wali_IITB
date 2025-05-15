using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CargoInteractable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    
    // Truck boundaries (set in Inspector)
    public Vector3 truckMinBounds;
    public Vector3 truckMaxBounds;
    
    // Optional visual feedback
    [Header("Visual Feedback")]
    [SerializeField] private bool enableHighlight = true;
    [SerializeField] private Color highlightColor = new Color(1f, 0.92f, 0.016f, 0.5f);
    
    // Cached components
    private Renderer objectRenderer;
    private Color originalColor;
    private Material material;
    private Vector3 originalPosition;
    
    void Start()
    {
        mainCamera = Camera.main;
        objectRenderer = GetComponent<Renderer>();
        originalPosition = transform.position;
        
        if (objectRenderer != null && enableHighlight)
        {
            material = objectRenderer.material;
            originalColor = material.color;
        }
        
        // Setup EventTrigger component in the Inspector
        SetupEventTrigger();
    }
    
    // Setup EventTrigger component with all necessary events
    private void SetupEventTrigger()
    {
        // Get or add EventTrigger component
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        
        // Clear existing entries to avoid duplicates
        eventTrigger.triggers = new List<EventTrigger.Entry>();
        
        // Add Pointer Down event
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { OnPointerDownHandler((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerDown);
        
        // Add Drag event
        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => { OnDragHandler((PointerEventData)data); });
        eventTrigger.triggers.Add(drag);
        
        // Add Pointer Up event
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { OnPointerUpHandler((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerUp);
        
        Debug.Log("EventTrigger component has been set up on " + gameObject.name);
    }
    
    // Event handler methods for EventTrigger
    public void OnPointerDownHandler(PointerEventData data)
    {
        // Calculate offset between pointer position and object position
        Vector3 pointerPos = GetPointerWorldPosition(data);
        offset = transform.position - pointerPos;
        isDragging = true;
        
        // Apply visual highlight
        if (objectRenderer != null && enableHighlight)
        {
            material.color = highlightColor;
        }
        
        Debug.Log($"Cargo selected: {gameObject.name}");
    }
    
    public void OnDragHandler(PointerEventData data)
    {
        if (isDragging)
        {
            // Get current pointer position in world space
            Vector3 pointerPos = GetPointerWorldPosition(data);
            
            // Calculate new position with the original offset
            Vector3 newPosition = pointerPos + offset;
            
            // Constrain position within truck bounds if set
            if (truckMinBounds != Vector3.zero || truckMaxBounds != Vector3.zero)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, truckMinBounds.x, truckMaxBounds.x);
                newPosition.y = Mathf.Clamp(newPosition.y, truckMinBounds.y, truckMaxBounds.y);
                newPosition.z = Mathf.Clamp(newPosition.z, truckMinBounds.z, truckMaxBounds.z);
            }
            
            transform.position = newPosition;
        }
    }
    
    public void OnPointerUpHandler(PointerEventData data)
    {
        isDragging = false;
        
        // Reset visual feedback
        if (objectRenderer != null && enableHighlight)
        {
            material.color = originalColor;
        }
        
        // Check if the cargo is in a valid position
        ValidatePosition();
        
        Debug.Log($"Cargo released: {gameObject.name}");
    }
    
    // Helper method to convert screen pointer position to world position
    private Vector3 GetPointerWorldPosition(PointerEventData data)
    {
        // Create a ray from the camera through the pointer position
        Ray ray = mainCamera.ScreenPointToRay(data.position);
        
        // Create a plane at the object's height (y-position)
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        
        // Check if the ray hits the plane
        if (dragPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        
        // Fallback to current position if raycast fails
        return transform.position;
    }
    
    // Check if the box is placed in a valid position
    private void ValidatePosition()
{
    bool inside =
        transform.position.x >= truckMinBounds.x && transform.position.x <= truckMaxBounds.x &&
        transform.position.y >= truckMinBounds.y && transform.position.y <= truckMaxBounds.y &&
        transform.position.z >= truckMinBounds.z && transform.position.z <= truckMaxBounds.z;

    if (inside)
    {
        Debug.Log($"Cargo {gameObject.name} placed inside container");
    }
    else
    {
        Debug.Log($"Cargo {gameObject.name} placed outside container - resetting position");
        transform.position = originalPosition;
    }
}

    // Legacy mouse input handlers (optional, can be removed if you only want EventTrigger)
    void OnMouseDown()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            mainCamera.WorldToScreenPoint(transform.position).z
        ));
        offset = transform.position - mousePos;
        isDragging = true;
        
        if (objectRenderer != null && enableHighlight)
        {
            material.color = highlightColor;
        }
    }
    
    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, 
                Input.mousePosition.y, 
                mainCamera.WorldToScreenPoint(transform.position).z
            ));
            
            Vector3 newPosition = mousePos + offset;
            
            if (truckMinBounds != Vector3.zero || truckMaxBounds != Vector3.zero)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, truckMinBounds.x, truckMaxBounds.x);
                newPosition.y = Mathf.Clamp(newPosition.y, truckMinBounds.y, truckMaxBounds.y);
                newPosition.z = Mathf.Clamp(newPosition.z, truckMinBounds.z, truckMaxBounds.z);
            }
            
            transform.position = newPosition;
        }
    }
    
    void OnMouseUp()
    {
        isDragging = false;
        
        if (objectRenderer != null && enableHighlight)
        {
            material.color = originalColor;
        }
        
        ValidatePosition();
    }
}