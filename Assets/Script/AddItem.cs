using UnityEngine;
using UnityEngine.UI;

public class AddItem : MonoBehaviour
{
    public GameObject panel;
    public GameObject prefabToAdd;
    public Transform spawnPoint;

    private GameObject currentInstance;

    void Start()
    {
        panel.SetActive(false);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void AddPrefab()
    {
        if (currentInstance == null && prefabToAdd != null && spawnPoint != null)
        {
            currentInstance = Instantiate(prefabToAdd, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Prefab already exists or missing references.");
        }
    }
}
