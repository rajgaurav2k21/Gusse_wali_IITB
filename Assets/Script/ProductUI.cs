using TMPro;
using UnityEngine;

public class ProductUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text lengthText;
    public TMP_Text breadthText;
    public TMP_Text heightText;
    public TMP_Text weightText;
    public TMP_Text volumeText;
    public TMP_Text quantityTextText;

    public void SetProductDetails(string name, float length, float breadth, float height, float weight, float volume, int quantity)
    {
        nameText.text = name;
        lengthText.text = length.ToString();
        breadthText.text = breadth.ToString();
        heightText.text = height.ToString();
        weightText.text = weight.ToString();
        volumeText.text = volume.ToString();
        quantityTextText.text = quantity.ToString();
    }
}
