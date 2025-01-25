using UnityEngine;

public class TruckLoader : MonoBehaviour
{
    [Header("UI Input Fields")]
    public TMPro.TMP_InputField lengthInput; // Input for item length
    public TMPro.TMP_InputField widthInput;  // Input for item width
    public TMPro.TMP_InputField heightInput; // Input for item height
    public TMPro.TMP_InputField totalBoxInput; // Input for total number of items

    [Header("Truck Container")]
    public Transform container; // The truck's container Transform
    public GameObject itemPrefab; // Prefab for the cargo item

    [Header("Container Configuration")]
    public Vector3 containerStartPoint; // Starting position within the container
    public float spacing = 0.1f; // Spacing between items

    public void LoadItems()
    {
        // Parse input values
        float itemLength = float.Parse(lengthInput.text);
        float itemWidth = float.Parse(widthInput.text);
        float itemHeight = float.Parse(heightInput.text);
        int totalItems = int.Parse(totalBoxInput.text);

        // Calculate the size of the item
        Vector3 itemSize = new Vector3(itemLength, itemHeight, itemWidth);

        // Arrange items in the container
        int itemsPerRow = Mathf.FloorToInt(container.localScale.x / (itemLength + spacing));
        int itemsPerColumn = Mathf.FloorToInt(container.localScale.z / (itemWidth + spacing));

        int count = 0;
        for (int i = 0; i < Mathf.CeilToInt((float)totalItems / itemsPerRow / itemsPerColumn); i++) // Layers
        {
            for (int j = 0; j < itemsPerRow && count < totalItems; j++) // Rows
            {
                for (int k = 0; k < itemsPerColumn && count < totalItems; k++) // Columns
                {
                    // Calculate the position for the item
                    Vector3 position = containerStartPoint
                        + new Vector3(j * (itemLength + spacing), i * (itemHeight + spacing), k * (itemWidth + spacing));

                    // Instantiate the item
                    GameObject item = Instantiate(itemPrefab, position, Quaternion.identity, container);
                    item.transform.localScale = itemSize;

                    count++;
                }
            }
        }
    }
}
