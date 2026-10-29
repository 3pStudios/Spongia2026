using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float scrollSpeed;
    public float minY;
    public float maxY;

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;   // mouse wheel
        float y = transform.position.y + scroll * scrollSpeed;

        y = Mathf.Clamp(y, minY, maxY);

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}