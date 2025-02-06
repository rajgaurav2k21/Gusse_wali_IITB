using System.Collections;
using UnityEngine;

public class CargoLoader : MonoBehaviour
{
    public GameObject cargoPrefab; // Cargo box prefab
    public Transform truckContainer; // Reference to truck cargo area
    public Transform cargoAnchorPoint; // Empty placed at a truck corner for alignment
    public int rows = 2, columns = 5, layers = 3;
    public float spacing = 1.2f; // Space between boxes
    public float loadDelay = 0.1f; // Delay for simulation effect
    public float groundOffset = 0.05f; // Prevents clipping inside surface
    public GameObject[] truckPartsToDisable; // Parts to disable during loading

    private void Start()
    {
        if (cargoAnchorPoint == null || cargoPrefab == null || truckContainer == null)
        {
            Debug.LogError("CargoPrefab, CargoAnchorPoint, or TruckContainer is not assigned.");
        }
    }

    public void OnLoadButtonClick()
    {
        if (cargoAnchorPoint == null || cargoPrefab == null)
        {
            Debug.LogError("CargoPrefab or CargoAnchorPoint is not assigned.");
            return;
        }

        truckContainer.gameObject.SetActive(true); // Ensure truck is active
        truckContainer.localScale = Vector3.one; // Reset scale
        DisableTruckParts();
        StartCoroutine(LoadCargo());
    }

    private void DisableTruckParts()
    {
        foreach (GameObject part in truckPartsToDisable)
        {
            if (part != null && part != truckContainer.gameObject)
            {
                part.SetActive(false);
            }
        }
    }

    private IEnumerator LoadCargo()
    {
        ClearPreviousCargo();
        float boxHeight = cargoPrefab.GetComponent<Renderer>().bounds.size.y;
        float truckFloorHeight = GetTruckFloorHeight();

        for (int y = 0; y < layers; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                for (int z = 0; z < rows; z++)
                {
                    Vector3 localPosition = new Vector3(
                        x * spacing,
                        y * spacing + boxHeight / 2f,  // Adjust for pivot
                        z * spacing
                    );

                    GameObject box = Instantiate(cargoPrefab, cargoAnchorPoint);
                    box.transform.localPosition = localPosition; // Local to anchor
                    yield return new WaitForSeconds(loadDelay);
                }
            }
        }
    }

    private float GetTruckFloorHeight()
    {
        RaycastHit hit;
        Vector3 rayStart = truckContainer.position + Vector3.up * 1f; // Start ray slightly above truck bed

        if (Physics.Raycast(rayStart, Vector3.down, out hit, 2f))
        {
            Debug.Log("Detected truck surface at: " + hit.point.y);
            Debug.DrawRay(rayStart, Vector3.down * 2f, Color.red, 5f); // Debug visualization

            return hit.point.y + groundOffset; // Prevent clipping
        }

        Debug.LogWarning("Failed to detect truck surface, using default Y.");
        return truckContainer.position.y; // Fallback
    }

    private void ClearPreviousCargo()
    {
        foreach (Transform child in cargoAnchorPoint)
        {
            Destroy(child.gameObject);
        }
    }
}
