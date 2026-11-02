using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class TutorialManger : MonoBehaviour
{
    public List<GameObject> scenes;
    int sceneNum = 1;

    public void Next()
    {
        if (sceneNum < scenes.Count - 1)
        {
            scenes[sceneNum].SetActive(false);
            sceneNum += 1;
            scenes[sceneNum].SetActive(true);
        } 
        else
            MainMenu();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }
}
