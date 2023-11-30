using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        // Convert screen space to world space
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // Set the object's position to follow the mouse
        transform.position = new Vector3(worldMousePosition.x, worldMousePosition.y, 0);
    }
}
