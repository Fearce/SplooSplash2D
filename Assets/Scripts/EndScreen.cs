﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public static Stopwatch Stopwatch;
    public static int score;

    // Start is called before the first frame update
    void Start()
    {
        Stopwatch.Stop();
        string text = $"Congratulations!\nYou beat the game in {Stopwatch.Elapsed.Hours}:{Stopwatch.Elapsed.Minutes}:{Stopwatch.Elapsed.Seconds}\nYour score was: {score}\nYour highest score is: {PlayerPrefs.GetInt("HighScore", 0).ToString()}";
        gameObject.GetComponent<Text>().text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
