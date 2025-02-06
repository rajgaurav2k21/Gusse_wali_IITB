using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField lengthInput;
    public TMP_InputField breadthInput;
    public TMP_InputField heightInput;
    public TMP_InputField weightInput;
    public TMP_InputField quantityInput;
    public Transform listViewContent; // Parent to hold instantiated product prefabs
    public GameObject productPrefab; // Assign Product UI Prefab in Inspector

    public void AddProductToList()
    {
        if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(lengthInput.text) ||
            string.IsNullOrEmpty(breadthInput.text) || string.IsNullOrEmpty(heightInput.text) ||
            string.IsNullOrEmpty(weightInput.text) || string.IsNullOrEmpty(quantityInput.text))
        {
            Debug.LogWarning("Please fill in all fields.");
            return;
        }

        GameObject newProduct = Instantiate(productPrefab, listViewContent);
        ProductUI productUI = newProduct.GetComponent<ProductUI>();

        productUI.SetProductDetails(
            nameInput.text,
            float.Parse(lengthInput.text),
            float.Parse(breadthInput.text),
            float.Parse(heightInput.text),
            float.Parse(weightInput.text),
            int.Parse(quantityInput.text)
        );

        ClearInputs();
    }

    void ClearInputs()
    {
        nameInput.text = "";
        lengthInput.text = "";
        breadthInput.text = "";
        heightInput.text = "";
        weightInput.text = "";
        quantityInput.text = "";
    }
}
