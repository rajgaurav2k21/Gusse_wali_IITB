using UnityEngine;
using TMPro;

public class AddItem : MonoBehaviour
{
    public GameObject prefabToAdd;
    public Transform spawnPoint;
    public TMP_Text groupText;  // Reference to GroupText outside the prefab

    private static int groupCounter = 1;

    public void AddPrefab()
    {
        if (prefabToAdd != null && spawnPoint != null)
        {
            GameObject newPanel = Instantiate(prefabToAdd, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            newPanel.SetActive(true);

            // Update the GroupText that is outside the prefab
            if (groupText != null)
            {
                groupText.text = "Group " + groupCounter;
            }
            else
            {
                Debug.LogWarning("GroupText reference is missing in the Inspector!");
            }

            groupCounter++;  // Increment for the next group
        }
        else
        {
            Debug.LogWarning("Prefab missing or spawn point not assigned.");
        }
    }
}
