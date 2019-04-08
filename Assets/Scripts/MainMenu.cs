using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject aboutPanel;

    public void PlayGame()
    {
        Debug.Log("loading scenes");
        SceneManager.LoadScene("PersistentScene");
        if (EndScreen.currentLevel == 2)
        {
            SceneManager.LoadScene("LevelTwo", LoadSceneMode.Additive);
        }
        else if (EndScreen.currentLevel == 3)
        {
            SceneManager.LoadScene("BossLevel", LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadScene("LevelOne", LoadSceneMode.Additive);
        }
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
        PortalSpawn.DifficultyModifier = PortalSpawn.DifficultyModifier * 1.1f;
        SceneManager.LoadScene("PersistentScene");
        if (EndScreen.currentLevel == 2)
        {
            SceneManager.LoadScene("LevelTwo", LoadSceneMode.Additive);
        }
        else if (EndScreen.currentLevel == 3)
        {
            SceneManager.LoadScene("BossLevel", LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadScene("LevelOne", LoadSceneMode.Additive);
        }
    }

    public void HighScoreMenu()
    {
        SceneManager.LoadScene(3);
    }
}
