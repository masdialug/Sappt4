using UnityEngine;

public class ClampToScreen : MonoBehaviour
{
    private Camera mainCamera;
    private float minX, maxX, minY, maxY;

    void Start()
    {
        mainCamera = Camera.main;

        // Calculate the screen boundaries in world coordinates
        Vector3 lowerLeftCorner = mainCamera.ScreenToWorldPoint(Vector3.zero);
        Vector3 upperRightCorner = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

        minX = lowerLeftCorner.x;
        maxX = upperRightCorner.x;
        minY = lowerLeftCorner.y;
        maxY = upperRightCorner.y;
    }

    void LateUpdate()
    {
        // Clamp the player's position to the screen boundaries
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;
    }
}