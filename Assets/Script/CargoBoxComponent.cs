// Base component that all cargo boxes will have
// Changed class name to avoid conflict with existing CargoBox class
using UnityEngine;

public class CargoBoxComponent : MonoBehaviour
{
    [Header("Box Properties")]
    public string boxId;
    public float weight = 1.0f;
    public BoxCollider boxCollider;

    [Header("Constraint Visualization")]
    public bool showConstraintWarnings = true;
    public Material defaultMaterial;
    public Material warningMaterial;

    private Vector3 originalSize;
    private Renderer boxRenderer;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        boxRenderer = GetComponent<Renderer>();
        if (boxRenderer != null)
        {
            defaultMaterial = boxRenderer.material;
        }

        originalSize = transform.localScale;
    }

    public void SetWarningState(bool hasWarning)
    {
        if (boxRenderer != null && showConstraintWarnings && warningMaterial != null)
        {
            boxRenderer.material = hasWarning ? warningMaterial : defaultMaterial;
        }
    }

    public Bounds GetWorldBounds()
    {
        return boxCollider.bounds;
    }

    public Vector3 GetSize()
    {
        return originalSize;
    }
}

// Fragile item component - attach to boxes that are fragile
public class FragileBox : MonoBehaviour
{
    [Header("Fragile Properties")]
    public float fragilityFactor = 1.0f; // Higher means more fragile
    public float maxWeightOnTop = 10.0f;
    public Material fragileMaterial;

    private CargoBoxComponent cargoBox;
    private Renderer boxRenderer;
    private bool isCompromised = false;

    private void Start()
    {
        cargoBox = GetComponent<CargoBoxComponent>();
        boxRenderer = GetComponent<Renderer>();
        
        if (fragileMaterial != null && boxRenderer != null)
        {
            boxRenderer.material = fragileMaterial;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision force exceeds fragility threshold
        if (collision.relativeVelocity.magnitude > fragilityFactor)
        {
            MarkAsCompromised();
        }

        // Check if box above is too heavy
        CargoBoxComponent collidingBox = collision.gameObject.GetComponent<CargoBoxComponent>();
        if (collidingBox != null)
        {
            // Determine if the colliding box is on top of this one
            Vector3 collisionDirection = collision.contacts[0].point - transform.position;
            if (Vector3.Dot(collisionDirection, Vector3.up) > 0.5f && collidingBox.weight > maxWeightOnTop)
            {
                MarkAsCompromised();
            }
        }
    }

    private void MarkAsCompromised()
    {
        if (!isCompromised)
        {
            isCompromised = true;
            Debug.LogWarning($"Fragile box {cargoBox.boxId} has been compromised!");
            cargoBox.SetWarningState(true);
        }
    }
}

// Overhang detection component
public class OverhangDetector : MonoBehaviour
{
    [Header("Overhang Settings")]
    public float maxOverhangPercentage = 0.2f; // Maximum allowed overhang (20% by default)
    public Transform truckContainer; // Reference to the truck boundaries
    
    private CargoBoxComponent cargoBox;
    private Bounds truckBounds;
    private bool hasOverhang = false;

    private void Start()
    {
        cargoBox = GetComponent<CargoBoxComponent>();
        
        // Find truck container if not assigned
        if (truckContainer == null)
        {
            GameObject truck = GameObject.FindGameObjectWithTag("TruckContainer");
            if (truck != null)
            {
                truckContainer = truck.transform;
            }
        }
        
        // Get truck bounds
        if (truckContainer != null)
        {
            Collider truckCollider = truckContainer.GetComponent<Collider>();
            if (truckCollider != null)
            {
                truckBounds = truckCollider.bounds;
                CheckOverhang();
            }
        }
    }

    public void CheckOverhang()
    {
        if (cargoBox == null || truckContainer == null) return;
        
        Bounds boxBounds = cargoBox.GetWorldBounds();
        
        // Calculate how much of the box is outside the truck bounds
        float overlapX = Mathf.Min(0, truckBounds.min.x - boxBounds.min.x) + 
                         Mathf.Max(0, boxBounds.max.x - truckBounds.max.x);
                         
        float overlapZ = Mathf.Min(0, truckBounds.min.z - boxBounds.min.z) + 
                         Mathf.Max(0, boxBounds.max.z - truckBounds.max.z);
                         
        float overlapY = Mathf.Max(0, boxBounds.max.y - truckBounds.max.y);
        
        float totalOverhang = Mathf.Abs(overlapX) + Mathf.Abs(overlapZ) + Mathf.Abs(overlapY);
        float boxVolume = boxBounds.size.x * boxBounds.size.y * boxBounds.size.z;
        float overhangPercentage = totalOverhang / boxVolume;
        
        if (overhangPercentage > maxOverhangPercentage)
        {
            hasOverhang = true;
            cargoBox.SetWarningState(true);
            Debug.LogWarning($"Box {cargoBox.boxId} has excessive overhang: {overhangPercentage:P}");
        }
    }
}

// Support/In-air check component
public class SupportDetector : MonoBehaviour
{
    [Header("Support Settings")]
    public float minSupportPercentage = 0.7f; // Minimum required support from below
    public float groundCheckDistance = 0.05f; // Distance to check for ground/support
    public LayerMask supportLayers; // Layers that can support this box
    
    private CargoBoxComponent cargoBox;
    private bool hasValidSupport = false;

    private void Start()
    {
        cargoBox = GetComponent<CargoBoxComponent>();
        supportLayers = LayerMask.GetMask("Cargo", "TruckFloor");
        
        // Check after a slight delay to allow for physics to settle
        Invoke("CheckSupport", 0.5f);
    }

    public void CheckSupport()
    {
        if (cargoBox == null) return;

        Bounds boxBounds = cargoBox.GetWorldBounds();
        Vector3 boxSize = boxBounds.size;
        Vector3 bottomCenter = new Vector3(boxBounds.center.x, boxBounds.min.y + 0.01f, boxBounds.center.z);
        
        // Create a grid of raycasts on the bottom face to check for support
        int raysPerAxis = 5; // 5x5 grid
        float xStep = boxSize.x / (raysPerAxis - 1);
        float zStep = boxSize.z / (raysPerAxis - 1);
        
        int supportedPoints = 0;
        int totalPoints = raysPerAxis * raysPerAxis;
        
        for (int x = 0; x < raysPerAxis; x++)
        {
            for (int z = 0; z < raysPerAxis; z++)
            {
                Vector3 rayStart = new Vector3(
                    bottomCenter.x - boxSize.x / 2 + x * xStep,
                    bottomCenter.y,
                    bottomCenter.z - boxSize.z / 2 + z * zStep
                );
                
                if (Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, supportLayers))
                {
                    supportedPoints++;
                }
            }
        }
        
        float supportPercentage = (float)supportedPoints / totalPoints;
        hasValidSupport = supportPercentage >= minSupportPercentage;
        
        if (!hasValidSupport)
        {
            cargoBox.SetWarningState(true);
            Debug.LogWarning($"Box {cargoBox.boxId} has insufficient support: {supportPercentage:P}");
        }
    }
}

// Overlap detector component
public class OverlapDetector : MonoBehaviour
{
    [Header("Overlap Settings")]
    public bool allowAnyOverlap = false;
    public float overlapTolerance = 0.01f; // Small tolerance to prevent false positives
    
    private CargoBoxComponent cargoBox;
    private bool hasOverlap = false;

    private void Start()
    {
        cargoBox = GetComponent<CargoBoxComponent>();
        
        // Check for overlaps after a slight delay to allow for all boxes to be placed
        Invoke("CheckOverlaps", 0.5f);
    }

    public void CheckOverlaps()
    {
        if (cargoBox == null) return;
        
        Collider[] overlappingColliders = Physics.OverlapBox(
            cargoBox.GetWorldBounds().center,
            cargoBox.GetWorldBounds().extents - new Vector3(overlapTolerance, overlapTolerance, overlapTolerance),
            transform.rotation,
            LayerMask.GetMask("Cargo")
        );
        
        foreach (Collider col in overlappingColliders)
        {
            // Skip self
            if (col.gameObject == gameObject) continue;
            
            CargoBoxComponent otherBox = col.GetComponent<CargoBoxComponent>();
            if (otherBox != null)
            {
                hasOverlap = true;
                cargoBox.SetWarningState(true);
                Debug.LogWarning($"Box {cargoBox.boxId} overlaps with {otherBox.boxId}");
                break;
            }
        }
    }
}