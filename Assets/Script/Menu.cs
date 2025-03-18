using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject panel;
    public GameObject disableObject;
    public GameObject enableObject;

    private bool isActive = false;
    public void TogglePanel()
    {
        isActive = !isActive;
        panel.SetActive(isActive);
        enableObject.SetActive(!isActive);
        disableObject.SetActive(isActive);
    }
}
