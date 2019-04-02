using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PortalSpawn : MonoBehaviour
{
    public GameObject enemy;
    public bool stopSpawn = false;
    private float spawnX;
    private Vector2 whereEnemySpawn;

    public float spawnRate;
    private int counter = 0;
    public float nextSpawn;
    private Animator spawnAnim;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GhostSpawn", spawnRate, nextSpawn);
        spawnAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void GhostSpawn()
    {
        
            StartCoroutine("SpawnAnim");
            SpawnAnim();
            counter++;
        

    }

    IEnumerator SpawnAnim()
    {
        if (counter < 6)
        {
            spawnAnim.SetBool("enemy", true);
            GameObject ps = GameObject.FindGameObjectWithTag("PortalSpawn");
            spawnX = ps.transform.position.x;
            whereEnemySpawn = new Vector2(spawnX, transform.position.y);
            yield return new WaitForSeconds(1f);
            Instantiate(enemy, whereEnemySpawn, Quaternion.identity);
            spawnAnim.SetBool("enemy", false);
            if (stopSpawn)
            {
                CancelInvoke("GhostSpawn");
            }

            Debug.Log(counter);
        }
        else
        {
            yield return new WaitForSeconds(10f);
            if (counter > 5)
            {
                counter = 0;
            }
        }
    }
}
