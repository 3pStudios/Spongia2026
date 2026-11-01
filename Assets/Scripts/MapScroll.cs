using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float minY;
    public float maxY;
    public int maxFloors = 12;

    private void Start()
    {
        if (RunManager.Instance == null) return;

        int floor = Mathf.Max(0, RunManager.Instance.currentFloor);
        float t = Mathf.Clamp01((float)floor / maxFloors);
        float targetY = Mathf.Lerp(minY, maxY, t);

        transform.position = new Vector3(transform.position.x, targetY +2f, transform.position.z);
    }

    private void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        float y = transform.position.y + scroll * scrollSpeed;

        y = Mathf.Clamp(y, minY, maxY);

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}