using UnityEngine;
using UnityEngine.UI;

public class TogglePartOfPrefab : MonoBehaviour
{
    public Toggle visibilityToggle;
    public GameObject prefabInstance;
    public string childObjectName;
    private GameObject partOfPrefab; 

    void Start()
    {
        if (prefabInstance == null)
        {
            Debug.LogError("Prefab instance not assigned!");
            return;
        }

        partOfPrefab = prefabInstance.transform.Find(childObjectName)?.gameObject;

        if (partOfPrefab == null)
        {
            Debug.LogError("Child object not found: " + childObjectName);
            return;
        }

        visibilityToggle.onValueChanged.AddListener(SetVisibility);
        SetVisibility(visibilityToggle.isOn);
    }

void SetVisibility(bool isVisible)
{
    Debug.Log("Toggle changed: " + isVisible);

    if (partOfPrefab != null)
    {
        partOfPrefab.SetActive(isVisible);
        Debug.Log("Upway visibility set to: " + isVisible);
    }
    else
    {
        Debug.LogError("Upway (partOfPrefab) is NULL! Check the object reference.");
    }
}


}
