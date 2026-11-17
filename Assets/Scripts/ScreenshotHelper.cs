using UnityEngine;
using UnityEngine.InputSystem;
public class ScreenshotHelper : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(fileName);
            Debug.Log("Saved screenshot: " + fileName);
        }
    }
}