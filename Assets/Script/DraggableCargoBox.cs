using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class DraggableCargoBox : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Collider containerCollider; // Assign this in the Inspector

    private Camera cam;
    private Vector3 offset;
    private float zDist;

    void Start()
    {
        cam = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        zDist = Vector3.Distance(cam.transform.position, transform.position);
        Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, zDist));
        offset = transform.position - worldPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, zDist));
        Vector3 targetPos = worldPoint + offset;

        if (containerCollider != null)
        {
            Bounds bounds = containerCollider.bounds;
            Vector3 halfSize = GetComponent<Collider>().bounds.extents;
            targetPos.x = Mathf.Clamp(targetPos.x, bounds.min.x + halfSize.x, bounds.max.x - halfSize.x);
            targetPos.y = Mathf.Clamp(targetPos.y, bounds.min.y + halfSize.y, bounds.max.y - halfSize.y);
            targetPos.z = Mathf.Clamp(targetPos.z, bounds.min.z + halfSize.z, bounds.max.z - halfSize.z);
        }

        transform.position = targetPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // No extra logic needed
    }
}
