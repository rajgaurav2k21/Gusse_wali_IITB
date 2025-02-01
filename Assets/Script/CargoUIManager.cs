using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [SerializeField] private GameObject cube;
    [SerializeField] private TMP_Text[] cubeTexts;
    [SerializeField] private GameObject wayUpIcon;
    [SerializeField] private Toggle wayUpToggle;

    [Header("UI Panel Management")]

    [SerializeField] private Transform panelContainer;
    [SerializeField] private GameObject panelPrefab;

    private bool isCubeActive = false;

    private void Start()
    {
        InitializeUI();
        HideCube();

        if (wayUpIcon != null)
            wayUpIcon.gameObject.SetActive(false);

        if (wayUpToggle != null)
            wayUpToggle.onValueChanged.AddListener(ToggleWayUpIcon);

    
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
        lengthField.text = "0";
        widthField.text = "0";
        heightField.text = "0";
        weightField.text = "0";
        volumeField.text = "0";
        totalboxField.text = "0";

        HideCube();
        UpdateCubeText("");

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

    private void AddNewPanel()
    {
        if (panelPrefab != null && panelContainer != null)
        {
            Instantiate(panelPrefab, panelContainer);
            Debug.Log("New cargo panel added.");
        }
    }
}