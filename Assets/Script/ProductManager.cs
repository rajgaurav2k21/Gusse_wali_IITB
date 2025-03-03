using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ProductManager : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField nameInput;
    public TMP_InputField lengthInput;
    public TMP_InputField breadthInput;
    public TMP_InputField heightInput;
    public TMP_InputField weightInput;
    public TMP_InputField volumeInput;
    public TMP_InputField quantitymanagerInput;

    [Header("UI Elements")]
    public Transform listViewContent;
    public GameObject productPrefab;
    public TMP_Text inventoryCountText;

    private List<ProductData> productList = new List<ProductData>();
    private int totalQuantity = 0;

    public void AddProductToList()
    {
        // Validate inputs
        if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(lengthInput.text) ||
            string.IsNullOrEmpty(breadthInput.text) || string.IsNullOrEmpty(heightInput.text) ||
            string.IsNullOrEmpty(weightInput.text) || string.IsNullOrEmpty(volumeInput.text) ||
            string.IsNullOrEmpty(quantitymanagerInput.text))
        {
            Debug.LogWarning("Please fill in all fields.");
            return;
        }

        // Parse inputs safely
        if (!float.TryParse(lengthInput.text, out float length) ||
            !float.TryParse(breadthInput.text, out float breadth) ||
            !float.TryParse(heightInput.text, out float height) ||
            !float.TryParse(weightInput.text, out float weight) ||
            !float.TryParse(volumeInput.text, out float volume) ||
            !int.TryParse(quantitymanagerInput.text, out int quantity))
        {
            Debug.LogWarning("Invalid input format. Ensure numeric values are entered correctly.");
            return;
        }

        totalQuantity += quantity;

        // Create new product instance in the UI
        GameObject newProduct = Instantiate(productPrefab, listViewContent);
        ProductUI productUI = newProduct.GetComponent<ProductUI>();

        // Check if ProductUI component exists
        if (productUI != null)
        {
            productUI.SetProductDetails(nameInput.text, length, breadth, height, weight, volume, quantity);
        }
        else
        {
            Debug.LogError("ProductUI script is missing on the productPrefab.");
        }

        // Store the product details in the inventory list
        productList.Add(new ProductData(nameInput.text, quantity));

        // Update UI with total inventory count
        UpdateInventoryUI();
        
        // Clear input fields
        ClearInputs();
    }

    private void UpdateInventoryUI()
    {
        if (inventoryCountText != null)
        {
            inventoryCountText.text = "Total Items: " + totalQuantity.ToString();
        }
    }

    public void ClearInputs()
    {
        nameInput.text = "";
        lengthInput.text = "";
        breadthInput.text = "";
        heightInput.text = "";
        weightInput.text = "";
        volumeInput.text = "";
        quantitymanagerInput.text = "";
    }
}

// Class to store product details
[System.Serializable]
public class ProductData
{
    public string productName;
    public int quantity;

    public ProductData(string name, int qty)
    {
        productName = name;
        quantity = qty;
    }
}
