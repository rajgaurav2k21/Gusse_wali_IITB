using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public TextMeshProUGUI quantityText;

    [Header("UI Elements")]
    public Transform listViewContent;
    public GameObject productPrefab;

    public void AddProductToList()
    {
        if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(lengthInput.text) ||
            string.IsNullOrEmpty(breadthInput.text) || string.IsNullOrEmpty(heightInput.text) ||
            string.IsNullOrEmpty(weightInput.text) || string.IsNullOrEmpty(volumeInput.text) ||
            string.IsNullOrEmpty(quantityText.text))
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
            float.Parse(volumeInput.text),
            float.Parse(quantityText.text)
        );

        ClearInputs();
    }

    public void ClearInputs()
    {
        nameInput.text = "";
        lengthInput.text = "";
        breadthInput.text = "";
        heightInput.text = "";
        weightInput.text = "";
        volumeInput.text = "";
        quantityText.text = "0";
    }
}
