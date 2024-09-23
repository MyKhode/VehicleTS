using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollReactMapView : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
{
    public Camera mapCamera;
    public float rotationAngle = 60f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomSpeed = 1f;
    public float dragSpeed = 0.1f;
    public float scrollSpeed = 0.1f;

    // Variables for scroll limits
    public float maxScrollX = 10f;
    public float maxScrollZ = 10f;

    // Variables for smooth scrolling
    public float scrollDamping = 0.1f;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;

    private Vector3 dragOrigin;
    private Vector3 initialCameraPosition;

    void Start()
    {
        if (mapCamera == null)
        {
            Debug.LogError("Map camera is not assigned!");
            return;
        }

        // Set initial camera angle
        mapCamera.transform.rotation = Quaternion.Euler(rotationAngle, 0, 0);
        initialCameraPosition = mapCamera.transform.position;
        targetPosition = initialCameraPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOrigin = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mapCamera == null) return;

        Vector3 difference = dragOrigin - Input.mousePosition;
        Vector3 movement = new Vector3(difference.x, 0, difference.y) * dragSpeed;
        targetPosition += movement;

        // Clamp the target position within the scroll limits
        targetPosition.x = Mathf.Clamp(targetPosition.x, initialCameraPosition.x - maxScrollX, initialCameraPosition.x + maxScrollX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, initialCameraPosition.z - maxScrollZ, initialCameraPosition.z + maxScrollZ);

        dragOrigin = Input.mousePosition;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (mapCamera == null) return;

        // Zoom in/out
        float scrollDelta = -eventData.scrollDelta.y * zoomSpeed;
        float newZoom = Mathf.Clamp(targetPosition.y - scrollDelta, minZoom, maxZoom);
        targetPosition.y = newZoom;
    }

    void Update()
    {
        // Apply smooth scrolling, ensuring it works even when Time.timeScale is 0
        if (Time.timeScale == 0)
        {
            mapCamera.transform.position = Vector3.SmoothDamp(mapCamera.transform.position, targetPosition, ref currentVelocity, scrollDamping, Mathf.Infinity, Time.unscaledDeltaTime);
        }
        else
        {
            mapCamera.transform.position = Vector3.SmoothDamp(mapCamera.transform.position, targetPosition, ref currentVelocity, scrollDamping);
        }
    }
}
