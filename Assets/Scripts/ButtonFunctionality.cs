using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionality : MonoBehaviour
{
    public List<GameObject> menuThings;
    public List<GameObject> raceSelectionThings;

    public void StartGame()
    {
        OpenRaceSelection();
    }

    public void SettingsMenu()
    {
        SceneManager.LoadScene("Settings Menu");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Start menu");
    }

    public void StartHalflingsGame()
    {
        if (RunManager.Instance != null)
        {
            RunManager.Instance.StartNewRun(Race.Halflings);
        }
        SceneManager.LoadScene("Map");
    }

    public void StartElvesGame()
    {
        if (RunManager.Instance != null)
        {
            RunManager.Instance.StartNewRun(Race.Elves);
        }
        SceneManager.LoadScene("Map");
    }

    public void StartMaiarGame()
    {
        if (RunManager.Instance != null)
        {
            RunManager.Instance.StartNewRun(Race.Maiar);
        }
        SceneManager.LoadScene("Map");
    }

    public void HideRaceSelection()
    {
        CloseRaceSelection();
    }

    void OpenRaceSelection()
    {
        foreach (GameObject thing in menuThings)
        {
            thing.SetActive(false);
        }
        foreach (GameObject thing in raceSelectionThings)
        {
            thing.SetActive(true);
        }
        
    }

    void CloseRaceSelection()
    {
        foreach (GameObject thing in menuThings)
        {
            thing.SetActive(true);
        }
        foreach (GameObject thing in raceSelectionThings)
        {
            thing.SetActive(false);
        }
    }
    public void PlayButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButton();
    }

    public void ExitGame()
    {
        Application.Quit();
    }   
}