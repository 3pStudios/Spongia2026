using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionality : MonoBehaviour
{
    public void StartGame()
        {
            SceneManager.LoadScene("SampleScene");
        }

    public void SettingsMenu()
        {
            SceneManager.LoadScene("Settings Menu");
        }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }
}
