using System.Collections;
using UnityEngine;

public class CargoLoader : MonoBehaviour
{
    public GameObject cargoPrefab; // Cargo box prefab
    public Transform truckContainer; // Reference to truck cargo area
    public int rows = 2, columns = 5, layers = 3;
    public float spacing = 1.2f; // Space between boxes
    public float loadDelay = 0.1f; // Delay for simulation effect
    public float groundOffset = 0.05f; // Prevents clipping inside surface

    public GameObject[] truckPartsToDisable; // Parts to disable during loading

    private void Start()
    {
        if (truckContainer == null || cargoPrefab == null)
        {
            Debug.LogError("CargoPrefab or TruckContainer is not assigned.");
        }
    }

    public void OnLoadButtonClick()
    {
        if (truckContainer == null || cargoPrefab == null)
        {
            Debug.LogError("CargoPrefab or TruckContainer is not assigned.");
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

        // Get the detected surface height of the truck
        float baseHeight = GetTruckFloorHeight();

        if (baseHeight < 0)
        {
            Debug.LogError("No valid truck surface detected for loading cargo.");
            yield break;
        }

        // Loop through layers, rows, and columns to instantiate cargo in the correct position
        for (int y = 0; y < layers; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                for (int z = 0; z < rows; z++)
                {
                    // Calculate position based on the truck's detected cargo area surface
                    Vector3 worldPosition = new Vector3(
                        truckContainer.position.x + (x * spacing),
                        baseHeight + (y * spacing), // Start at detected surface & stack upwards
                        truckContainer.position.z + (z * spacing)
                    );

                    // Instantiate cargo inside the truck container
                    GameObject box = Instantiate(cargoPrefab, worldPosition, Quaternion.identity, truckContainer);

                    yield return new WaitForSeconds(loadDelay); // Add delay for effect
                }
            }
        }

        Debug.Log("Cargo loaded correctly.");
    }

    private float GetTruckFloorHeight()
    {
        RaycastHit hit;
        Vector3 rayStart = truckContainer.position + Vector3.up * 1f; // Start ray slightly above truck bed

        if (Physics.Raycast(rayStart, Vector3.down, out hit, 2f))
        {
            Debug.Log("Detected truck surface at: " + hit.point.y);
            return hit.point.y + groundOffset; // Prevents clipping
        }

        Debug.LogWarning("Failed to detect truck surface, using default Y.");
        return truckContainer.position.y; // Fallback
    }

    private void ClearPreviousCargo()
    {
        foreach (Transform child in truckContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
