using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform topView;  // Assign the TopView empty GameObject
    public Transform leftView; // Assign the LeftView empty GameObject
    public Transform rightView; // Assign the RightView empty GameObject

    public float moveSpeed = 5f; // Speed of the camera movement

    private Transform targetPosition; // Current target position

    void Update()
    {
        if (targetPosition != null)
        {
            // Smoothly move the CameraManager to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPosition.rotation, Time.deltaTime * moveSpeed);
        }
    }

    public void MoveToTopView()
    {
        targetPosition = topView; // Set target to TopView
    }

    public void MoveToLeftView()
    {
        targetPosition = leftView; // Set target to LeftView
    }

    public void MoveToRightView()
    {
        targetPosition = rightView; // Set target to RightView
    }
}
