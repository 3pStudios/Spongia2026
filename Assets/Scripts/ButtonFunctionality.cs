using System.Collections.Generic;
using NUnit.Framework.Constraints;
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
        SceneManager.LoadScene("Start Menu");
    }

    public void StartHalflingsGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void StartElvesGame()
    {
        Debug.Log("Start elf game");
    }
    public void StartMaiarGame()
    {
        Debug.Log("Start maiar game");
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
}
