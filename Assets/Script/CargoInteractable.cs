using UnityEngine;
using System.Collections;

public class CargoBox : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Vector3 originalPosition;
    private bool isDragging = false;
    private Vector3 offset;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = true; // Prevent physics movement when dragging
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tipping over

        originalPosition = transform.localPosition;
    }

    private void OnMouseOver()
{
    if (Input.GetMouseButtonDown(0)) // Left click
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }
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
    private void OnMouseDown()
{
    if (Camera.main == null) return;

    // Ignore container collider and interact only with cargo boxes
    int layerMask = LayerMask.GetMask("Cargo");
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
    {
        if (hit.collider.gameObject == gameObject) // Ensure we clicked this box
        {
            isDragging = true;
            offset = transform.position - GetMouseWorldPosition();
            Debug.Log($"👆 Picked up: {gameObject.name}");
        }
    }
}

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private IEnumerator CheckIfFloating()
    {
        yield return new WaitForSeconds(0.5f); // Allow physics to settle

        if (!IsBoxOnSurface())
        {
            Debug.Log("Box is floating! Moving back.");
            StartCoroutine(MoveBackToOriginalPosition());
        }
    }

    private bool IsBoxOnSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.2f))
        {
            if (hit.collider.CompareTag("Cargo") || hit.collider.CompareTag("Container"))
            {
                return true; // The box is resting on another surface
            }
        }
        return false; // It's floating
    }

    private IEnumerator MoveBackToOriginalPosition()
    {
        float time = 0;
        Vector3 startPosition = transform.localPosition;

        while (time < 1)
        {
            transform.localPosition = Vector3.Lerp(startPosition, originalPosition, time);
            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
