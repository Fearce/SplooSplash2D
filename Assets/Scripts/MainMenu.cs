using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject aboutPanel;

    public void PlayGame()
    {
        Debug.Log("loading scenes");
        SceneManager.LoadScene("PersistentScene");
        SceneManager.LoadScene("LevelOne", LoadSceneMode.Additive);

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AboutTheGame()
    {
        SceneManager.LoadScene(2);
    }

    public void backToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void DynamicDifficultyReset()
    {
        SceneManager.LoadScene(1);
        PortalSpawn.DifficultyModifier = PortalSpawn.DifficultyModifier * 1.1f;
    }

    public void HighScoreMenu()
    {
        SceneManager.LoadScene(3);
    }
}
