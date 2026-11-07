using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 5f;       // Mouse wheel sensitivity
    public float touchDragSpeed = 0.02f;  // Touch drag sensitivity on mobile
    public bool invertTouchDirection = true; // Natural swipe (drag up = camera down)

    [Header("Limits")]
    public float minY;
    public float maxY;
    public int maxFloors = 12;

    private Vector2 lastMousePosition;

    private void Start()
    {
        if (RunManager.Instance == null) return;

        int floor = Mathf.Max(0, RunManager.Instance.currentFloor);
        float t = Mathf.Clamp01((float)floor / maxFloors);
        float targetY = Mathf.Lerp(minY, maxY, t);

        transform.position = new Vector3(transform.position.x, targetY + 2f, transform.position.z);
    }

    private void Update()
    {
        float deltaY = 0f;

        if (Input.mouseScrollDelta.y != 0)
        {
            deltaY += Input.mouseScrollDelta.y * scrollSpeed;
        }
        
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float touchDelta = touch.deltaPosition.y * touchDragSpeed;
                deltaY += invertTouchDirection ? -touchDelta : touchDelta;
            }
        }

        if (Mathf.Abs(deltaY) > 0.0001f)
        {
            float y = Mathf.Clamp(transform.position.y + deltaY, minY, maxY);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
}