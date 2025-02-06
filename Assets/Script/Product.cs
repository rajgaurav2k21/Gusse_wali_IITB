using UnityEngine;
using TMPro;

public class ProductItem : MonoBehaviour
{
    public TMP_Text productNameText, dimensionsText, weightText, quantityText;
    
    public void SetProductDetails(string name, string length, string breadth, string height, string weight, string quantity)
    {
        productNameText.text = name;
        dimensionsText.text = $"L: {length} B: {breadth} H: {height}";
        weightText.text = $"Weight: {weight} kg";
        quantityText.text = $"Qty: {quantity}";
    }
}
