using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CargoManager : MonoBehaviour
{
    [SerializeField] private GameObject cargoPanelPrefab;  // Prefab for input panel
    [SerializeField] private Transform panelContainer;     // Parent container for input panels
    [SerializeField] private Transform summaryContainer;   // Parent for summary text fields
    [SerializeField] private GameObject summaryItemPrefab; // Prefab for displaying cargo summary
    [SerializeField] private Button addButton;             // Button to add new cargo panel
    [SerializeField] private Button saveButton;            // Button to save cargo details

    private List<GameObject> cargoPanels = new List<GameObject>();
    private List<string> cargoSummary = new List<string>();

    private void Start()
    {
        addButton.onClick.AddListener(AddNewCargoPanel);
        saveButton.onClick.AddListener(SaveCargoDetails);
    }

    public void AddNewCargoPanel()
    {
        GameObject newPanel = Instantiate(cargoPanelPrefab, panelContainer);
        cargoPanels.Add(newPanel);
    }

    public void SaveCargoDetails()
    {
        if (cargoPanels.Count == 0) return;

        GameObject lastPanel = cargoPanels[cargoPanels.Count - 1];
        TMP_InputField[] inputs = lastPanel.GetComponentsInChildren<TMP_InputField>();

        string cargoDetails = "";
        foreach (TMP_InputField input in inputs)
        {
            cargoDetails += input.text + " | ";
            input.text = ""; // Clear input field for new entry
        }

        cargoSummary.Add(cargoDetails);
        UpdateSummaryPanel();
    }

    private void UpdateSummaryPanel()
    {
        foreach (Transform child in summaryContainer)
        {
            Destroy(child.gameObject); // Clear previous summary
        }

        foreach (string summary in cargoSummary)
        {
            GameObject summaryItem = Instantiate(summaryItemPrefab, summaryContainer);
            TMP_Text summaryText = summaryItem.GetComponentInChildren<TMP_Text>();
            summaryText.text = summary;
        }
    }
}
