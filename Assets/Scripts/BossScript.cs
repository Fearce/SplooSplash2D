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

    private GameObject Enemy;

    public float StartOnPlayerXValue;
    public float MovementSpeed;
    public int Health;
    public float SpawnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Enemy = GameObject.FindGameObjectWithTag("Enemy");
        Splatter = Enemy.GetComponent<EnemyScript>().Splatter;
        BloodSplash = Enemy.GetComponent<EnemyScript>().BloodSplash;
        enemyDeath = Enemy.GetComponent<EnemyScript>().enemyDeath;
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
                if (gameObject.name == "Boss1")
                {
                    EndScreen.score = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().points;
                    EndScreen.currentLevel = 2;
                    SceneManager.LoadScene("PersistentScene");
                    SceneManager.LoadScene("LevelTwo", LoadSceneMode.Additive);

                }
                else if (gameObject.name == "Boss2")
                {
                    EndScreen.score = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().points;
                    EndScreen.currentLevel = 3;
                    SceneManager.LoadScene("PersistentScene");
                    SceneManager.LoadScene("BossLevel", LoadSceneMode.Additive);
                }
            }
            else
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().points += 1000;
                EndScreen.score = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().points;
                SceneManager.LoadScene("EndScene");
            }
        }

        if (Fighting && moveCounter < 100)
        {
            moveCounter++;
            transform.position = new Vector3(transform.position.x + MovementSpeed, transform.position.y);
        }
        else if (Fighting && moveCounter < 250)
        {
            moveCounter++;
            transform.position = new Vector3(transform.position.x - MovementSpeed, transform.position.y);
        }
        else if (Fighting)
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
                GameObject ghost = Instantiate(Enemy, transform.position, Quaternion.identity);
                Color[] colorValues = new Color[]
                {
                    Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow, Color.white
                };
                ghost.GetComponent<SpriteRenderer>().color = colorValues[Random.Range(0, colorValues.Length - 1)];
                yield return new WaitForSeconds(SpawnSpeed);
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
