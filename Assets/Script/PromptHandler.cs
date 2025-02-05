using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro; 
using System.Text;
using System.Collections;

public class TestScriptPost : MonoBehaviour
{
    public Button submitButton;
    public TMP_InputField inputField; // Use TMP_InputField
    public TMP_Text responseText;     // Use TMP_Text

    private string apiEndpoint = "https://ff03-103-21-125-30.ngrok-free.app/start"; 

    void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SendPostRequest);
        }
        else
        {
            Debug.LogError("Button is not assigned in the Inspector.");
        }
    }

    private void SendPostRequest()
    {
        string inputValue = inputField.text;
        StartCoroutine(PostRequest(apiEndpoint, inputValue));
    }

    private IEnumerator PostRequest(string url, string bodyData)
    {
        string jsonData = $"{{\"query\":\"{bodyData}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            responseText.text = "Response: " + request.downloadHandler.text;
        }
        else
        {
            responseText.text = "Error: " + request.error;
        }
    }
}