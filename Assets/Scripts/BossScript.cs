﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    private GameObject Target;
    private bool Fighting = false;
    private GameObject enemyDeath;
    private GameObject BloodSplash;
    private Splatter Splatter;


    public float FiringSpeed;
    public float StartOnPlayerXValue;
    public float BossSizeScale;
    public float MovementSpeed;
    public int Health;

    // Start is called before the first frame update
    void Start()
    {
        var enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyScript>();
        Splatter = enemy.Splatter;
        BloodSplash = enemy.BloodSplash;
        enemyDeath = enemy.enemyDeath;
        Target = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine("WaitForPlayer");
        transform.localScale = new Vector3(BossSizeScale,BossSizeScale);
    }



    private int moveCounter = 0;

    // Update is called once per frame
    void Update()
    {
        // If health <= 0 ghost is dead
        if (Health <= 0)
        {
            Handheld.Vibrate();

            Instantiate(enemyDeath, transform.position, Quaternion.identity);
            Debug.Log("ded ghost");
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, -3.29f);
            var prevScale = gameObject.transform.localScale;
            this.BloodSplash.transform.localScale = prevScale / 4;
            Instantiate(BloodSplash, pos, Quaternion.identity);
            Splatter.randomColor = false;
            Splatter.gameObject.GetComponent<SpriteRenderer>().color =
                Color.red;
            this.Splatter.gameObject.transform.localScale = prevScale / 7;
            Splatter splatterObj = (Splatter)Instantiate(Splatter, pos, Quaternion.identity);

            this.BloodSplash.transform.localScale = Vector3.one;
            this.Splatter.gameObject.transform.localScale = Vector3.one;

            Destroy(gameObject);
        }

        if (moveCounter < 100)
        {
            moveCounter++;
            transform.position = new Vector3(transform.position.x + MovementSpeed, transform.position.y);
        }
        else if (moveCounter < 250)
        {
            moveCounter++;
            transform.position = new Vector3(transform.position.x - MovementSpeed, transform.position.y);
        }
        else
        {
            moveCounter = 0;
        }
    }

    IEnumerator WaitForPlayer()
    {
        while (!Fighting)
        {
            // Wait for player to reach x == 200 before beginning AI
            if (Target.transform.position.x > StartOnPlayerXValue && !Fighting)
            {
                // Debug.Log("Boss engaged");
                StartCoroutine("FightPlayer");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    IEnumerator FightPlayer()
    {
        if (!Fighting)
        {
            Fighting = true;
            while (Fighting)
            {
                // Debug.Log("Boss Fighting!");
                GameObject ghost = Instantiate(GameObject.FindGameObjectWithTag("Enemy"), transform.position, Quaternion.identity);
                Color[] colorValues = new Color[]
                {
                    Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow, Color.white
                };
                ghost.GetComponent<SpriteRenderer>().color = colorValues[Random.Range(0, colorValues.Length - 1)];
                yield return new WaitForSeconds(FiringSpeed);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {

        // Take damage from bullet
        if (coll.gameObject.tag.Contains("Bullet"))
        {
            Debug.Log("ghost takin dmg!");
            Health -= coll.gameObject.GetComponent<BulletScript>().Damage;
        }

        // When colliding with player (redundant?)
        if (coll.gameObject.tag == "Player")
        {
            Debug.Log("Collision with Player");
        }

    }
}
