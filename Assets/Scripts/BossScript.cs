using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    private GameObject Target;
    private bool Fighting = false;
    private GameObject enemyDeath;
    private GameObject BloodSplash;
    private Splatter Splatter;



    public float StartOnPlayerXValue;
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
            this.BloodSplash.transform.localScale = gameObject.transform.localScale/8;
            Instantiate(BloodSplash, pos, Quaternion.identity);
            Splatter.randomColor = false;
            Splatter.gameObject.GetComponent<SpriteRenderer>().color =
                Color.red;
            this.Splatter.gameObject.transform.localScale = gameObject.transform.localScale/6;
            Splatter splatterObj = (Splatter)Instantiate(Splatter, pos, Quaternion.identity);
            Destroy(gameObject);
            if (gameObject.name != "Boss3")
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().points += 50;
                GameObject.FindGameObjectWithTag("StatusText").GetComponent<Text>().text = "Boss dead! New level!";
                GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.one;
            }
            else
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().points += 1000;
                SceneManager.LoadScene("EndScene");
            }
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
                yield return new WaitForSeconds(0.1f);
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
