using UnityEngine;

public class TruckViewController : MonoBehaviour
{
    [Header("Truck and View Positions")]
    [SerializeField] private GameObject truck; // The truck GameObject
    [SerializeField] private Transform rightViewCube; // Reference position for the right view
    [SerializeField] private Transform leftViewCube;  // Reference position for the left view
    [SerializeField] private Transform topViewCube;   // Reference position for the top view

    // Function to show the right view
    public void ShowRightView()
    {
        MoveTruckToView(rightViewCube);
    }

    // Function to show the left view
    public void ShowLeftView()
    {
        MoveTruckToView(leftViewCube);
    }

    // Function to show the top view
    public void ShowTopView()
    {
        MoveTruckToView(topViewCube);
    }

    // Helper function to move the truck to the specified view position
    private void MoveTruckToView(Transform viewTransform)
    {
        truck.transform.position = viewTransform.position;
        truck.transform.rotation = viewTransform.rotation;
    }
}
