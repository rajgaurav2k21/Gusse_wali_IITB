using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductManager : MonoBehaviour
{
    [Header("TMP Input Fields")]
    public TMP_InputField nameInput;
    public TMP_InputField lengthInput;
    public TMP_InputField breadthInput;
    public TMP_InputField heightInput;
    public TMP_InputField quantityInput;
    public TMP_InputField weightInput;

    [Header("TMP Buttons")]
    public Button enterButton;
    public Button updateButton;

    [Header("List View Setup")]
    public Transform productListContainer;
    public GameObject productItemPrefab;

    private List<Product> products = new List<Product>();
    private ProductItem selectedProductItem = null;

    void Start()
    {
        enterButton.onClick.AddListener(() => AddProduct());
        updateButton.onClick.AddListener(() => UpdateProduct());
        updateButton.gameObject.SetActive(false);
    }

    void AddProduct()
    {
        if (!ValidateInputs()) return;

        Product newProduct = new Product(
            nameInput.text,
            float.Parse(lengthInput.text),
            float.Parse(breadthInput.text),
            float.Parse(heightInput.text),
            int.Parse(quantityInput.text),
            float.Parse(weightInput.text)
        );

        products.Add(newProduct);
        DisplayProduct(newProduct);
        ClearInputs();
    }

    void DisplayProduct(Product product)
    {
        GameObject productItem = Instantiate(productItemPrefab, productListContainer);
        ProductItem itemScript = productItem.GetComponent<ProductItem>();
        itemScript.Setup(product, this);
        productItem.transform.SetAsLastSibling();
    }

    public void EditProduct(Product product, ProductItem item)
    {
        nameInput.text = product.productName;
        lengthInput.text = product.length.ToString();
        breadthInput.text = product.breadth.ToString();
        heightInput.text = product.height.ToString();
        quantityInput.text = product.quantity.ToString();
        weightInput.text = product.weight.ToString();

        selectedProductItem = item;
        enterButton.gameObject.SetActive(false);
        updateButton.gameObject.SetActive(true);
    }

    void UpdateProduct()
    {
        if (selectedProductItem == null) return;
        if (!ValidateInputs()) return;

        Product updatedProduct = new Product(
            nameInput.text,
            float.Parse(lengthInput.text),
            float.Parse(breadthInput.text),
            float.Parse(heightInput.text),
            int.Parse(quantityInput.text),
            float.Parse(weightInput.text)
        );

        selectedProductItem.UpdateProduct(updatedProduct);
        ClearInputs();
        selectedProductItem = null;

        enterButton.gameObject.SetActive(true);
        updateButton.gameObject.SetActive(false);
    }

    void ClearInputs()
    {
        nameInput.text = lengthInput.text = breadthInput.text = heightInput.text = quantityInput.text = weightInput.text = "";
    }

    bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text) ||
            string.IsNullOrWhiteSpace(lengthInput.text) ||
            string.IsNullOrWhiteSpace(breadthInput.text) ||
            string.IsNullOrWhiteSpace(heightInput.text) ||
            string.IsNullOrWhiteSpace(quantityInput.text) ||
            string.IsNullOrWhiteSpace(weightInput.text))
        {
            Debug.LogWarning("Please fill in all fields!");
            return false;
        }
        return true;
    }
}
