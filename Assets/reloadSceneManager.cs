using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class reloadSceneManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene(0);
            PortalSpawn.DifficultyModifier = PortalSpawn.DifficultyModifier * 1.1f;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
            PortalSpawn.DifficultyModifier = 1f;
        }
    }
}
