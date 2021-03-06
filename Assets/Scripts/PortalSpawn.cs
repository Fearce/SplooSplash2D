﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PortalSpawn : MonoBehaviour
{


    private GameObject player;

    public GameObject enemy;
    public bool stopSpawn = false;
    private Vector2 whereEnemySpawn;

    private float spawnRate = 1;
    public int amountGhosts;
    private int counter = 0;
    public float nextSpawn;

    // Dynamic dif, controlled by reloadSceneManager
    public static float DifficultyModifier = 1;
    private float actualNextSpawn => nextSpawn * DifficultyModifier;

    private Animator spawnAnim;

    public float StartX;
    public float StopX;

    public float RestartDelay;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("GhostSpawn", spawnRate, actualNextSpawn);
        spawnAnim = GetComponent<Animator>();
        gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
    }

    // Update is called once per frame
    public void GhostSpawn()
    {
        if (player.transform.position.x > StartX && player.transform.position.x < StopX) 
        {
            StartCoroutine("SpawnAnim");
            SpawnAnim();
            counter++;
        }
    }
    

    IEnumerator SpawnAnim()
    {
        if (counter < amountGhosts)
        {
            spawnAnim.SetBool("enemy", true);
            whereEnemySpawn = new Vector2(gameObject.transform.position.x, transform.position.y);
            yield return new WaitForSeconds(1f);
            GameObject ghost = Instantiate(enemy, whereEnemySpawn, Quaternion.identity);
            Color[] colorValues = new Color[]
            {
                Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow, Color.white
            };
            ghost.GetComponent<SpriteRenderer>().color = colorValues[Random.Range(0, colorValues.Length - 1)];
            spawnAnim.SetBool("enemy", false);
            if (stopSpawn)
            {
                CancelInvoke("GhostSpawn");
            }

            Debug.Log($"Difficulty {DifficultyModifier}, nextspawn {actualNextSpawn}");
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(RestartDelay);
            if (counter > amountGhosts-1)
            {
                counter = 0;
            }
            gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }
}
