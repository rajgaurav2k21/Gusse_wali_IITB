using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public GameObject itemPanelPrefab;  // Prefab for Input Panel
    public GameObject summaryPanelPrefab; // Prefab for Summary Panel
    public Transform panelContainer; // Parent where panels will be placed

    private Queue<GameObject> summaryPanels = new Queue<GameObject>(); // Holds fixed number of panels
    private const int maxPanels = 3; // Set max visible panels

    void Start()
    {
        AddNewInputPanel();
    }

    public void AddNewInputPanel()
    {
        GameObject newPanel = Instantiate(itemPanelPrefab, panelContainer);
        Button addItemButton = newPanel.transform.Find("AddButton").GetComponent<Button>();
        addItemButton.onClick.AddListener(() => ConvertToSummary(newPanel));
    }

    public void ConvertToSummary(GameObject inputPanel)
    {
        // Get input values
        string itemNo = inputPanel.transform.Find("ItemNoInput").GetComponent<InputField>().text;
        string dimensions = inputPanel.transform.Find("DimensionsInput").GetComponent<InputField>().text;
        string quantity = inputPanel.transform.Find("QuantityInput").GetComponent<InputField>().text;

        // Create a summary panel
        GameObject summaryPanel = Instantiate(summaryPanelPrefab, panelContainer);
        summaryPanel.transform.Find("ItemNoText").GetComponent<Text>().text = itemNo;
        summaryPanel.transform.Find("DimensionsText").GetComponent<Text>().text = dimensions;
        summaryPanel.transform.Find("QuantityText").GetComponent<Text>().text = quantity;

        // Setup Modify and Delete buttons
        Button modifyButton = summaryPanel.transform.Find("ModifyButton").GetComponent<Button>();
        Button deleteButton = summaryPanel.transform.Find("DeleteButton").GetComponent<Button>();

        modifyButton.onClick.AddListener(() => ModifyItem(summaryPanel, inputPanel));
        deleteButton.onClick.AddListener(() => DeleteItem(summaryPanel));

        summaryPanels.Enqueue(summaryPanel); // Add to queue

        // Remove oldest panel if limit is exceeded
        if (summaryPanels.Count > maxPanels)
        {
            GameObject oldPanel = summaryPanels.Dequeue();
            Destroy(oldPanel);
        }

        // Remove input panel and create a new one
        Destroy(inputPanel);
        AddNewInputPanel();
    }

    public void ModifyItem(GameObject summaryPanel, GameObject newInputPanel)
    {
        // Populate the input panel with data from the summary panel
        newInputPanel.transform.Find("ItemNoInput").GetComponent<InputField>().text =
            summaryPanel.transform.Find("ItemNoText").GetComponent<Text>().text;
        newInputPanel.transform.Find("DimensionsInput").GetComponent<InputField>().text =
            summaryPanel.transform.Find("DimensionsText").GetComponent<Text>().text;
        newInputPanel.transform.Find("QuantityInput").GetComponent<InputField>().text =
            summaryPanel.transform.Find("QuantityText").GetComponent<Text>().text;

        // Remove the summary panel
        DeleteItem(summaryPanel);
    }

    public void DeleteItem(GameObject summaryPanel)
    {
        summaryPanels.Dequeue(); // Remove from queue
        Destroy(summaryPanel);
    }
}
