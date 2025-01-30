using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class CargoUIManager : MonoBehaviour
{
    [Header("Cargo Input Fields")]
    [SerializeField] private TMP_InputField itemNoField;
    [SerializeField] private TMP_InputField lengthField;
    [SerializeField] private TMP_InputField widthField;
    [SerializeField] private TMP_InputField heightField;
    [SerializeField] private TMP_InputField weightField;
    [SerializeField] private TMP_InputField volumeField;
    [SerializeField] private TMP_InputField totalboxField;
    [SerializeField] private TMP_InputField groupnameField;

    [Header("Cube in Hierarchy")]
    [SerializeField] private GameObject cube; // Reference to the cube in the hierarchy
    [SerializeField] private TMP_Text[] cubeTexts; // Array of TextMeshPro components on the cube
    [SerializeField] private GameObject wayUpIcon; // UI Image for "This Way Up" icon
    [SerializeField] private Toggle wayUpToggle; // Toggle button for enabling/disabling the icon

    [Header("Buttons")]
    [SerializeField] private Button addButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    private bool isCubeActive = false; // Track whether the cube is active
    private List<CargoData> cargoList = new List<CargoData>(); // List to store all cargo data

    private const string FileName = "/cargoData.json"; // File name for saving and loading

    private void Start()
    {
        InitializeUI();
        HideCube(); // Hide the cube at the start

        // Ensure the icon is hidden initially
        if (wayUpIcon != null)
            wayUpIcon.gameObject.SetActive(false);

        // Add listeners
        if (wayUpToggle != null)
            wayUpToggle.onValueChanged.AddListener(ToggleWayUpIcon);

        if (addButton != null)
            addButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(AddCargoData);

        if (saveButton != null)
            saveButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(SaveCargoDataToFile);

        if (loadButton != null)
            loadButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(LoadCargoDataFromFile);
    }

    private void InitializeUI()
    {
        lengthField.contentType = TMP_InputField.ContentType.IntegerNumber;
        widthField.contentType = TMP_InputField.ContentType.IntegerNumber;
        heightField.contentType = TMP_InputField.ContentType.IntegerNumber;
        weightField.contentType = TMP_InputField.ContentType.DecimalNumber;

        lengthField.onValueChanged.AddListener(OnDimensionInputChanged);
        widthField.onValueChanged.AddListener(OnDimensionInputChanged);
        heightField.onValueChanged.AddListener(OnDimensionInputChanged);
        itemNoField.onEndEdit.AddListener(OnItemNoChanged);

        ClearFields();
    }

    private void ClearFields()
    {
        itemNoField.text = "";
        groupnameField.text = "";
        lengthField.text = "0";
        widthField.text = "0";
        heightField.text = "0";
        weightField.text = "0";
        volumeField.text = "0";
        totalboxField.text = "0";

        HideCube();
        UpdateCubeText("");

        // Reset toggle
        if (wayUpToggle != null)
            wayUpToggle.isOn = false;
    }

    private void OnItemNoChanged(string value)
    {
        Debug.Log($"Item No. changed to: {value}");
        UpdateCubeText(value);
    }

    private void OnDimensionInputChanged(string value)
    {
        if (!isCubeActive &&
            (float.TryParse(lengthField.text, out float length) && length > 0 ||
             float.TryParse(widthField.text, out float width) && width > 0 ||
             float.TryParse(heightField.text, out float height) && height > 0))
        {
            ShowCube();
        }

        UpdateCubeDimensions();
    }

    private void ShowCube()
    {
        if (cube != null)
        {
            cube.SetActive(true);
            isCubeActive = true;
            Debug.Log("Cube is now visible.");
        }
    }

    private void HideCube()
    {
        if (cube != null)
        {
            cube.SetActive(false);
            isCubeActive = false;
            Debug.Log("Cube is now hidden.");
        }

        // Hide the "This Way Up" icon
        if (wayUpIcon != null)
            wayUpIcon.gameObject.SetActive(false);
    }

    private void UpdateCubeDimensions()
    {
        if (cube != null &&
            float.TryParse(lengthField.text, out float length) &&
            float.TryParse(widthField.text, out float width) &&
            float.TryParse(heightField.text, out float height))
        {
            cube.transform.localScale = new Vector3(length, height, width);
            Debug.Log($"Cube dimensions updated: Length={length}, Height={height}, Width={width}");

            float volume = length * width * height;
            volumeField.text = volume.ToString("F2");
            Debug.Log($"Volume updated: {volume}");
        }
    }

    private void UpdateCubeText(string text)
    {
        if (cubeTexts != null)
        {
            foreach (var cubeText in cubeTexts)
            {
                if (cubeText != null)
                {
                    cubeText.text = text;
                }
            }
        }
    }

    private void ToggleWayUpIcon(bool isOn)
    {
        if (wayUpIcon != null)
        {
            wayUpIcon.gameObject.SetActive(isOn);
            Debug.Log(isOn ? "This Way Up icon enabled." : "This Way Up icon disabled.");
        }
    }

    // Add cargo to the list
    private void AddCargoData()
    {
        // Create a new CargoData object and populate it
        CargoData cargo = new CargoData
        {
            groupName = groupnameField.text,
            itemName = itemNoField.text,
            length= float.Parse(lengthField.text),
            width= float.Parse(widthField.text),
            height= float.Parse(heightField.text),
            volume= float.Parse(volumeField.text),
            totalbox= int.Parse(totalboxField.text),
            weight = float.Parse(weightField.text)
        };

        // Add the cargo to the list
        cargoList.Add(cargo);
        Debug.Log($"Added: {cargo}");
        ClearFields();
    }

    private void SaveCargoDataToFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "cargoData.json");
        string jsonData = JsonUtility.ToJson(new CargoWrapper { cargoDataList = cargoList }, true);

        File.WriteAllText(path, jsonData);
        Debug.Log($"Cargo data saved to {path}");
    }

    private void LoadCargoDataFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "cargoData.json");

        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            CargoWrapper wrapper = JsonUtility.FromJson<CargoWrapper>(jsonData);

            cargoList = wrapper.cargoDataList;
            Debug.Log("Cargo data loaded successfully.");
            foreach (var cargo in cargoList)
            {
                Debug.Log(cargo);
            }
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }
    // Load the cargo list from a JSON file
    private void LoadFromFile()
    {
        string filePath = Application.persistentDataPath + FileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CargoWrapper wrapper = JsonUtility.FromJson<CargoWrapper>(json);
            cargoList = wrapper.cargoDataList;

            Debug.Log("Data loaded successfully.");
        }
        else
        {
            Debug.LogWarning("No saved file found!");
        }
    }

    [System.Serializable]
   private class CargoWrapper
{
    public List<CargoData> cargoDataList;

    // Parameterless constructor
    public CargoWrapper() { }

    // Constructor with parameters
    public CargoWrapper(List<CargoData> list)
    {
        cargoDataList = list;
    }
}
    
}
