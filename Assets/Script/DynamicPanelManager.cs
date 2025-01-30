using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShipmentGroup : MonoBehaviour
{
    public GameObject inputPanelPrefab; // Prefab for the input panel
    public Transform contentArea; // The Scroll View's Content (Should have a Vertical Layout Group)
    public TMP_InputField groupNameInput; // Optional: Input field for custom group names

    private int groupCounter = 1;

    public void AddNewGroup()
    {
        // Instantiate the panel inside ScrollView Content
        GameObject newPanel = Instantiate(inputPanelPrefab, contentArea);

        // Reset Transform to ensure proper layout alignment
        RectTransform rectTransform = newPanel.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one; // Reset scale
        rectTransform.anchoredPosition = Vector2.zero; // Reset position (Layout Group will handle placement)

        // Find the GroupName text inside the panel
        TMP_Text groupNameText = newPanel.transform.Find("GroupName")?.GetComponent<TMP_Text>();

        if (groupNameText != null)
        {
            // Set the default group name
            groupNameText.text = "Group " + groupCounter;

            // Optional: Use custom name if provided
            if (groupNameInput && !string.IsNullOrWhiteSpace(groupNameInput.text))
            {
                groupNameText.text = groupNameInput.text;
                groupNameInput.text = ""; // Clear input field
            }
        }
        else
        {
            Debug.LogError("GroupName text object not found in the prefab.");
        }

        groupCounter++; // Increment group count
    }
}
