using UnityEngine;
using TMPro;

public class InputFieldNavigator : MonoBehaviour
{
    [Header("Input Fields in Order")]
    [SerializeField] private TMP_InputField[] inputFields; // Array of input fields in sequential order

    private int currentIndex = 0; // Track the current input field index

    private void Start()
    {
        // Set focus to the first field
        if (inputFields.Length > 0)
        {
            SetFocusOnField(currentIndex);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter key
        {
            MoveToNextField();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) // Up Arrow key
        {
            MoveToPreviousField();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) // Down Arrow key
        {
            MoveToNextField();
        }
    }

    private void MoveToNextField()
    {
        if (inputFields.Length == 0) return;

        // Increment the index and loop back if needed
        currentIndex = (currentIndex + 1) % inputFields.Length;
        SetFocusOnField(currentIndex);
    }

    private void MoveToPreviousField()
    {
        if (inputFields.Length == 0) return;

        // Decrement the index and loop back if needed
        currentIndex = (currentIndex - 1 + inputFields.Length) % inputFields.Length;
        SetFocusOnField(currentIndex);
    }

    private void SetFocusOnField(int index)
    {
        if (inputFields[index] != null)
        {
            inputFields[index].ActivateInputField(); // Activate the selected field
        }
    }
}
