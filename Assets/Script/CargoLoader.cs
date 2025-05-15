using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class CargoData
{
    public List<BoxPosition> final_arrangement;
}

[Serializable]
public class BoxPosition
{
    public string box_id;
    public float pos_x;
    public float pos_y;
    public float pos_z;
    public float length;
    public float width;
    public float height;
    public float x_norm;
    public float y_norm;
    public float z_norm;
    public bool placed;
    public float group;
    public bool is_fragile;
    public float weight = 1.0f;
}

[Serializable]
public class TruckData
{
    public float length;
    public float height;
    public float width;
    public bool wt_capacity;
}

public class CargoBoxProperties : MonoBehaviour
{
    [Header("Box Properties")]
    public string boxId;
    public float weight = 1.0f;
    public bool isFragile = false;
    public BoxCollider boxCollider;

    [Header("Constraint Status")]
    public bool hasOverhangIssue = false;
    public bool hasOverlapIssue = false;
    public bool hasInAirIssue = false;
    public bool isFragileCompromised = false;

    [Header("Visualization")]
    public bool showWarnings = true;
    public Material defaultMaterial;
    public Material warningMaterial;

    private Renderer boxRenderer;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider>();

        boxRenderer = GetComponent<Renderer>();
        if (boxRenderer != null)
            defaultMaterial = boxRenderer.sharedMaterial;
    }

    public void SetWarningState(bool hasWarning)
    {
        if (boxRenderer != null && showWarnings && warningMaterial != null)
        {
            boxRenderer.sharedMaterial = hasWarning ? warningMaterial : defaultMaterial;
        }
    }

    public void UpdateWarningVisual()
    {
        bool hasAnyIssue = hasOverhangIssue || hasOverlapIssue || hasInAirIssue || isFragileCompromised;
        SetWarningState(hasAnyIssue);
    }

    public Bounds GetWorldBounds()
    {
        return boxCollider.bounds;
    }
}

public class CargoLoader : MonoBehaviour
{
    [Header("Cargo References")]
    public GameObject defaultCargoPrefab;
    public GameObject[] cargoPrefabsByGroup;
    public Transform cargoAnchorPoint;

    [Header("Truck References")]
    public Transform truckContainer;

    [Header("API Settings")]
    public string apiBaseUrl = "http://127.0.0.1:8000";

    [Header("Constraint Settings")]
    public bool checkOverhang = true;
    public bool checkOverlap = true;
    public bool checkInAir = true;
    public bool checkFragility = true;
    public Material warningMaterial;
    public float maxOverhangPercentage = 0.2f;
    public float minSupportPercentage = 0.7f;

    [Header("Auto-Scaling Settings")]
    public Vector3 maxViewSize = new Vector3(150f, 76f, 71f);
    public float scalingBuffer = 1.0f;

    private Collider truckCollider;
    private float scaleFactor = 1.0f;

    private void Start()
    {
        if (cargoAnchorPoint == null || defaultCargoPrefab == null || truckContainer == null)
        {
            Debug.LogError("Missing required references in inspector");
            return;
        }

        if (truckContainer.GetComponent<Collider>() == null)
        {
            BoxCollider boxCollider = truckContainer.gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            truckCollider = boxCollider;
        }
        else
        {
            truckCollider = truckContainer.GetComponent<Collider>();
        }
    }

    public void LoadCargo()
    {
        StartCoroutine(FetchCargoData());
    }

    private IEnumerator FetchCargoData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{apiBaseUrl}/config"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to fetch truck config: {www.error}");
                yield break;
            }

            TruckData truckData = JsonUtility.FromJson<TruckData>(www.downloadHandler.text);
            ResizeTruckContainer(truckData.length, truckData.height, truckData.width);
        }

        using (UnityWebRequest www = UnityWebRequest.Get($"{apiBaseUrl}/final_arrangement"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to fetch cargo data: {www.error}");
                yield break;
            }

            CargoData cargoData = JsonUtility.FromJson<CargoData>(www.downloadHandler.text);
            if (cargoData != null && cargoData.final_arrangement != null && cargoData.final_arrangement.Count > 0)
            {
                PlaceCargo(cargoData.final_arrangement);
            }
            else
            {
                Debug.LogError("Received empty or invalid cargo data");
            }
        }
    }

    private void PlaceCargo(List<BoxPosition> boxes)
    {
        ClearExistingCargo();

        foreach (BoxPosition box in boxes)
        {
            GameObject prefab = defaultCargoPrefab;
            int groupIndex = Mathf.FloorToInt(box.group);

            if (cargoPrefabsByGroup != null && groupIndex > 0 && groupIndex <= cargoPrefabsByGroup.Length)
            {
                prefab = cargoPrefabsByGroup[groupIndex - 1];
            }

            GameObject cargoBox = Instantiate(prefab, cargoAnchorPoint);
            cargoBox.name = $"Cargo_{box.box_id}";

            cargoBox.transform.localPosition = new Vector3(
                box.pos_x * scaleFactor,
                box.pos_y * scaleFactor,
                box.pos_z * scaleFactor
            );

            cargoBox.transform.localScale = new Vector3(
                box.length * scaleFactor,
                box.width * scaleFactor,
                box.height * scaleFactor
            );

            if (box.x_norm != 0 || box.y_norm != 0 || box.z_norm != 0)
            {
                Vector3 normalY = new Vector3(0, box.y_norm, 0).normalized;
                Vector3 normalZ = new Vector3(0, 0, box.z_norm).normalized;
                Quaternion rotation = Quaternion.LookRotation(normalZ, normalY);
                cargoBox.transform.localRotation = rotation;
            }
            else
            {
                cargoBox.transform.localRotation = Quaternion.identity;
            }

            CargoBoxProperties boxProps = cargoBox.GetComponent<CargoBoxProperties>();
            if (boxProps == null)
                boxProps = cargoBox.AddComponent<CargoBoxProperties>();

            boxProps.boxId = box.box_id;
            boxProps.weight = box.weight;
            boxProps.isFragile = box.is_fragile;
            boxProps.warningMaterial = warningMaterial;
        }
    }

    private void ResizeTruckContainer(float length, float height, float width)
{
    // Define max and min view sizes
    Vector3 maxViewSize = new Vector3(150f, 76f, 71f);
    Vector3 minViewSize = new Vector3(130f, 40f, 50f); // Approximate min sizes

    // Calculate scaling factors for each dimension based on max view size
    float scaleX = maxViewSize.x / length;
    float scaleY = maxViewSize.y / height;
    float scaleZ = maxViewSize.z / width;

    // Use the minimal scaling factor to ensure the entire truck fits within max view size
    scaleFactor = Mathf.Min(scaleX, scaleY, scaleZ);

    // Ensure the scaled dimensions are not smaller than minViewSize
    float scaledLength = length * scaleFactor;
    float scaledHeight = height * scaleFactor;
    float scaledWidth = width * scaleFactor;

    // Adjust scaleFactor if any dimension is below minViewSize
    float minScaleX = minViewSize.x / length;
    float minScaleY = minViewSize.y / height;
    float minScaleZ = minViewSize.z / width;
    float minScaleFactor = Mathf.Max(minScaleX, minScaleY, minScaleZ);

    if (scaledLength < minViewSize.x || scaledHeight < minViewSize.y || scaledWidth < minViewSize.z)
    {
        scaleFactor = minScaleFactor;
        scaledLength = length * scaleFactor;
        scaledHeight = height * scaleFactor;
        scaledWidth = width * scaleFactor;
    }

    // Apply buffer to prevent clipping
    scaleFactor *= scalingBuffer;

    // Recalculate scaled dimensions with buffer
    scaledLength = length * scaleFactor;
    scaledHeight = height * scaleFactor;
    scaledWidth = width * scaleFactor;

    // Determine frame height for proper positioning
    float frameHeight = 1.0f * scaleFactor;
    Transform truckFrame = null;
    foreach (Transform child in truckContainer)
    {
        string childName = child.name.ToLower();
        if (childName.Contains("frame") || childName.Contains("chassis"))
        {
            truckFrame = child;
            break;
        }
    }

    if (truckFrame != null)
    {
        Renderer frameRenderer = truckFrame.GetComponent<Renderer>();
        if (frameRenderer != null)
        {
            frameHeight = frameRenderer.bounds.max.y - truckContainer.position.y;
        }
    }

    // Update truck components
    Transform truckBody = null;
    Transform truckFloor = null;
    Transform truckSides = null;
    Transform containerObject = null;

    foreach (Transform child in truckContainer)
    {
        string childName = child.name.ToLower();
        if (childName.Contains("body")) truckBody = child;
        if (childName.Contains("floor") || childName.Contains("base")) truckFloor = child;
        if (childName.Contains("sides") || childName.Contains("walls")) truckSides = child;
        if (childName.Contains("container") || childName.Contains("box")) containerObject = child;
    }

    // Scale truck body
    if (truckBody != null)
    {
        truckBody.localScale = new Vector3(
            truckBody.localScale.x * scaleFactor,
            truckBody.localScale.y * scaleFactor,
            truckBody.localScale.z * scaleFactor
        );
    }

    // Scale and position truck floor
    if (truckFloor != null)
    {
        truckFloor.localScale = new Vector3(
            truckFloor.localScale.x * scaleFactor,
            truckFloor.localScale.y, // Keep floor thickness unscaled in Y
            truckFloor.localScale.z * scaleFactor
        );
        truckFloor.localPosition = new Vector3(0, frameHeight, 0);
    }

    // Scale and position truck sides
    if (truckSides != null)
    {
        truckSides.localScale = new Vector3(
            truckSides.localScale.x * scaleFactor,
            truckSides.localScale.y * scaleFactor,
            truckSides.localScale.z * scaleFactor
        );
        truckSides.localPosition = new Vector3(0, frameHeight + (scaledHeight / 2), 0);
    }

    // Scale and position container
    if (containerObject != null)
    {
        containerObject.localScale = new Vector3(
            scaledLength,
            scaledHeight,
            scaledWidth
        );
        containerObject.localPosition = new Vector3(0, frameHeight + (scaledHeight / 2), 0);
    }

    // Update truck collider
    if (truckCollider != null)
    {
        DestroyImmediate(truckCollider);
    }

    BoxCollider newCollider = truckContainer.gameObject.AddComponent<BoxCollider>();
    newCollider.size = new Vector3(scaledLength, scaledHeight, scaledWidth);
    newCollider.center = new Vector3(0, frameHeight + (scaledHeight / 2), 0);
    newCollider.isTrigger = true;
    truckCollider = newCollider;

    // Adjust cargo anchor point
    cargoAnchorPoint.localPosition = new Vector3(
        cargoAnchorPoint.localPosition.x,
        frameHeight,
        cargoAnchorPoint.localPosition.z
    );
}
    private void ClearExistingCargo()
    {
        foreach (Transform child in cargoAnchorPoint)
        {
            Destroy(child.gameObject);
        }
    }
}
