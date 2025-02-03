using System.Collections;
using UnityEngine;

public class CargoLoader : MonoBehaviour
{
    public GameObject cargoPrefab; // Assign the cargo box prefab in the Inspector
    public Transform truckContainer; // Acts as the ground reference for cargo placement
    public int rows = 2, columns = 5, layers = 3; // Cargo arrangement dimensions
    public float spacing = 1.2f; // Space between boxes
    public float loadDelay = 0.1f; // Delay between loading cargo to simulate simultaneous loading effect

    public GameObject[] truckPartsToDisable; // Array for truck parts to disable

    public void OnLoadButtonClick()
    {
        // Debug to check if truckContainer exists and is active
        Debug.Log("Truck Container Exists? " + (truckContainer != null));
        Debug.Log("Before loading: Truck Container Active? " + truckContainer.gameObject.activeSelf);

        if (truckContainer == null || cargoPrefab == null)
        {
            Debug.LogError("CargoPrefab or TruckContainer is not assigned in the Inspector.");
            return;
        }

        // Ensure truckContainer stays active
        truckContainer.gameObject.SetActive(true);
        truckContainer.localScale = Vector3.one; // Reset scale in case it was changed
        truckContainer.position = Vector3.zero;  // Adjust if needed

        DisableTruckParts(); // Disable specific truck parts
        StartCoroutine(LoadCargoSimultaneously()); // Load cargo with a delay for simulation effect

        Debug.Log("After loading: Truck Container Active? " + truckContainer.gameObject.activeSelf);
    }

    private void DisableTruckParts()
    {
        foreach (GameObject part in truckPartsToDisable)
        {
            if (part != null && part != truckContainer.gameObject) // Ensure truckContainer is NOT disabled
            {
                part.SetActive(false); // Disable the GameObject
                Debug.Log($"{part.name} has been disabled.");
            }
        }
    }

    private IEnumerator LoadCargoSimultaneously()
    {
        ClearPreviousCargo(); // Ensure the truck is empty before loading

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < layers; y++)
            {
                for (int z = 0; z < rows; z++)
                {
                    // Calculate the cargo position relative to truckContainer
                    Vector3 localPosition = new Vector3(x * spacing, y * spacing, z * spacing);

                    // Instantiate cargo box without parenting to truckContainer
                    GameObject box = Instantiate(cargoPrefab);
                    box.transform.position = truckContainer.position + localPosition;

                    Debug.Log($"Cargo box instantiated at: {box.transform.position}");

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
