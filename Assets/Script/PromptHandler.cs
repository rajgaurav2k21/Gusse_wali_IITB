using UnityEngine;
using UnityEngine.UI;

public class PromptHandler : MonoBehaviour
{
    // Reference to the InputField for the prompt
    public InputField inputField;

    // Reference to the Button to trigger the function
    public Button sendButton;

    // Reference to the Text element to display the response
    public Text responseText;

    void Start()
    {
        // Ensure references are set in the Inspector
        if (inputField == null || sendButton == null || responseText == null)
        {
            Debug.LogError("Please assign all UI elements in the Inspector.");
            return;
        }

        // Add a listener to the button's onClick event
        sendButton.onClick.AddListener(OnSendButtonClicked);
    }

    // Function to handle the button click
    private void OnSendButtonClicked()
    {
        // Get the text from the input field
        string prompt = inputField.text;

        // Check if the input field is empty
        if (string.IsNullOrEmpty(prompt))
        {
            responseText.text = "Please enter a prompt!";
            return;
        }

        // Process the prompt (replace this with your logic)
        string response = ProcessPrompt(prompt);

        // Display the response above the input field
        responseText.text = response;
    }

    // Function to process the prompt (replace with your logic)
    private string ProcessPrompt(string prompt)
    {
        // For now, just return a mock response
        return $"You entered: {prompt}\nResponse: This is a mock response.";
    }
}