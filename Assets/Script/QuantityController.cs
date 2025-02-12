using UnityEngine;
using TMPro;

public class QuantityController : MonoBehaviour
{
    public TMP_Text quantityText;
    private int quantity = 0;

    void Start()
    {
        UpdateText();
    }

    public void IncreaseQuantity()
    {
        quantity++;
        UpdateText();
    }

    public void DecreaseQuantity()
    {
        if (quantity > 0) 
        {
            quantity--;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        quantityText.text = quantity.ToString();
    }
}
