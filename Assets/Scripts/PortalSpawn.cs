using System.Collections;
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
    private Animator spawnAnim;

    public float StartX;
    public float StopX;

    public float RestartDelay;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("GhostSpawn", spawnRate, nextSpawn);
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
            ghost.GetComponent<SpriteRenderer>().color =
                new Color(Random.Range(0, 256), Random.Range(0, 256), Random.Range(0, 256));
            spawnAnim.SetBool("enemy", false);
            if (stopSpawn)
            {
                CancelInvoke("GhostSpawn");
            }

            Debug.Log(counter);
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
