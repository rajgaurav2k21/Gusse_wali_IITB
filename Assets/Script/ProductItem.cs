using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductItem : MonoBehaviour
{
    public TMP_Text productText;
    public Button editButton; // Still using Unity’s Button with TMP text

    private Product product;
    private ProductManager productManager;

    public void Setup(Product p, ProductManager manager)
    {
        product = p;
        productManager = manager;
        UpdateUI();
        editButton.onClick.AddListener(EditProduct);
    }

    void UpdateUI()
    {
        productText.text = $"{product.productName} - {product.length}x{product.breadth}x{product.height}, " +
                           $"Qty: {product.quantity}, Weight: {product.weight}kg";
    }

    void EditProduct()
    {
        productManager.EditProduct(product, this);
    }

    public void UpdateProduct(Product updatedProduct)
    {
        product = updatedProduct;
        UpdateUI();
    }
}
