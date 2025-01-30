using UnityEngine;

public class CargoLoader : MonoBehaviour
{
    public GameObject cargoPrefab; // Assign the cargo box prefab in the Inspector
    public Transform truckContainer; // Parent object inside the truck where boxes should be placed
    public int rows = 2, columns = 5, layers = 3; // Cargo arrangement dimensions
    public float spacing = 1.2f; // Space between boxes

    public void LoadCargo()
    {
        ClearPreviousCargo(); // Ensure the truck is empty before loading

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < layers; y++)
            {
                for (int z = 0; z < rows; z++)
                {
                    Vector3 position = truckContainer.position + new Vector3(x * spacing, y * spacing, z * spacing);
                    GameObject box = Instantiate(cargoPrefab, position, Quaternion.identity, truckContainer);
                    box.AddComponent<CargoInteractable>(); // Make the cargo interactable
                }
            }
        }
    }

    private void ClearPreviousCargo()
    {
        foreach (Transform child in truckContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
