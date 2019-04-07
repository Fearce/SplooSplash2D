﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject aboutPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
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
}
