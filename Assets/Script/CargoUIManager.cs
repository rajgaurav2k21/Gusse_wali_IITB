using UnityEngine;
using TMPro;

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

    [Header("Cube in Hierarchy")]
    [SerializeField] private GameObject cube; // Reference to the cube in the hierarchy
    [SerializeField] private TMP_Text[] cubeTexts; // Array of TextMeshPro components on the cube

    private bool isCubeActive = false; // Track whether the cube is active

    private void Start()
    {
        InitializeUI();
        HideCube(); // Hide the cube at the start
    }

    private void InitializeUI()
    {
        // Set Input Field Content Types for Numeric fields
        lengthField.contentType = TMP_InputField.ContentType.IntegerNumber; // Only integer values allowed
        widthField.contentType = TMP_InputField.ContentType.IntegerNumber;  // Only integer values allowed
        heightField.contentType = TMP_InputField.ContentType.IntegerNumber; // Only integer values allowed
        weightField.contentType = TMP_InputField.ContentType.DecimalNumber; // Allow decimal values for weight

        // Add listeners for real-time updates
        lengthField.onValueChanged.AddListener(OnDimensionInputChanged);
        widthField.onValueChanged.AddListener(OnDimensionInputChanged);
        heightField.onValueChanged.AddListener(OnDimensionInputChanged);

        // Add listener for item number
        itemNoField.onEndEdit.AddListener(OnItemNoChanged);

        // Initialize fields
        ClearFields();
    }

    private void ClearFields()
    {
        itemNoField.text = "";
        lengthField.text = "0";
        widthField.text = "0";
        heightField.text = "0";
        weightField.text = "0";
        volumeField.text = "0";
        totalboxField.text = "0";

        HideCube(); // Reset cube visibility when fields are cleared
        UpdateCubeText(""); // Clear text on the cube
    }

    private void OnItemNoChanged(string value)
    {
        Debug.Log($"Item No. changed to: {value}");
        UpdateCubeText(value); // Update the text on the cube
    }

    private void OnDimensionInputChanged(string value)
    {
        // Check if at least one dimension is non-zero to show the cube
        if (!isCubeActive &&
            (float.TryParse(lengthField.text, out float length) && length > 0 ||
             float.TryParse(widthField.text, out float width) && width > 0 ||
             float.TryParse(heightField.text, out float height) && height > 0))
        {
            ShowCube(); // Show the cube when a dimension is entered
        }

        UpdateCubeDimensions(); // Dynamically update the cube dimensions as the user types
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

            // Update volume field
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
                    cubeText.text = text; // Update text for each TextMeshPro component
                }
            }
        }
    }
}
