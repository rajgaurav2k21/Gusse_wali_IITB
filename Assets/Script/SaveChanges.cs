using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.InteropServices;

public class SavePrefabPositions : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Transform anchorPoint;
    [SerializeField] private string apiEndpoint = "http://10.119.11.41:8081/newposns"; // Change this to your actual endpoint

    [System.Serializable]
    public class PrefabData
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public class PrefabDataList
    {
        public List<PrefabData> items = new List<PrefabData>();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFileFromUnity(string filename, string text);
#endif

    void Start()
    {
        saveButton.onClick.AddListener(SavePositionsToAPI);

        if (anchorPoint == null)
        {
            GameObject anchorGO = GameObject.Find("AnchorPoint");
            if (anchorGO != null)
            {
                anchorPoint = anchorGO.transform;
                Debug.Log("Automatically found AnchorPoint: " + anchorPoint.name);
            }
            else
            {
                Debug.LogWarning("AnchorPoint not found! Please assign it manually in the inspector.");
            }
        }
    }

    void SavePositionsToAPI()
    {
        if (anchorPoint == null)
        {
            Debug.LogError("AnchorPoint is not assigned! Cannot save prefab positions.");
            return;
        }

        PrefabDataList dataList = new PrefabDataList();
        Transform[] allChildren = anchorPoint.GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child == anchorPoint || child.parent != anchorPoint)
                continue;

            PrefabData data = new PrefabData
            {
                name = child.name,
                position = child.position,
                rotation = child.eulerAngles,
                scale = child.localScale
            };
            dataList.items.Add(data);
        }

        if (dataList.items.Count == 0)
        {
            Debug.LogWarning("No prefab instances found under AnchorPoint!");
            return;
        }

        string json = JsonUtility.ToJson(dataList, true);

#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadFileFromUnity("saved_prefab_positions.json", json);
#else
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "saved_prefab_positions.json");
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("Saved prefab data locally to: " + filePath);
#endif

        StartCoroutine(PostJsonToAPI(json));
    }

    IEnumerator PostJsonToAPI(string json)
    {
        using (UnityWebRequest request = new UnityWebRequest(apiEndpoint, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending prefab data to API...");
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.Success)
#else
            if (!request.isNetworkError && !request.isHttpError)
#endif
            {
                Debug.Log("Successfully posted prefab data to API.");
                Debug.Log("Server response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to post prefab data to API.");
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
