using System.Collections;
using UnityEngine;

public class CargoLoader : MonoBehaviour
{
    public GameObject cargoPrefab; // Assign the cargo box prefab in the Inspector
    public Transform truckContainer; // Parent object inside the truck where boxes should be placed
    public int rows = 2, columns = 5, layers = 3; // Cargo arrangement dimensions
    public float spacing = 1.2f; // Space between boxes
    public float loadDelay = 0.1f; // Delay between loading cargo to simulate simultaneous loading effect

    public GameObject[] truckPartsToDisable; // Array for truck parts to disable

    public void OnLoadButtonClick()
    {
        if (cargoPrefab == null || truckContainer == null)
        {
            Debug.LogError("CargoPrefab or TruckContainer is not assigned in the Inspector.");
            return;
        }

        DisableTruckParts(); // Disable specific truck parts
        StartCoroutine(LoadCargoSimultaneously()); // Load cargo with a delay for simulation effect
    }

    private void DisableTruckParts()
    {
        foreach (GameObject part in truckPartsToDisable)
        {
            if (part != null)
            {
                part.SetActive(false); // Disable the GameObject
                Debug.Log($"{part.name} has been disabled.");
            }
            else
            {
                Debug.LogWarning("One of the truck parts is not assigned in the Inspector.");
            }
        }
    }

    private IEnumerator LoadCargoSimultaneously()
    {
        ClearPreviousCargo(); // Ensure the truck is empty before loading

        // Loop through the grid dimensions (columns, layers, and rows)
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < layers; y++)
            {
                for (int z = 0; z < rows; z++)
                {
                    // Calculate the local position of the cargo box in a grid layout
                    Vector3 localPosition = new Vector3(x * spacing, y * spacing, z * spacing);

                    // Instantiate the cargo box as a child of the truckContainer
                    GameObject box = Instantiate(cargoPrefab, truckContainer);
                    box.transform.localPosition = localPosition; // Set the local position relative to the truckContainer

                    // Log the position of each box for debugging
                    Debug.Log($"Cargo box instantiated at: {box.transform.localPosition}");

                    // Add delay for simulation effect
                    yield return new WaitForSeconds(loadDelay);
                }
            }
        }

        Debug.Log("Cargo loaded successfully.");
    }

    private void ClearPreviousCargo()
    {
        foreach (Transform child in truckContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
