using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main; // Cache the main camera
    }

    void OnMouseDown()
    {
        // Calculate offset from the mouse position to the object's position
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        // Move the object by setting the new position
        transform.position = GetMouseWorldPos() + offset;
    }

    private Vector3 GetMouseWorldPos()
    {
        // Convert mouse position to world position
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
