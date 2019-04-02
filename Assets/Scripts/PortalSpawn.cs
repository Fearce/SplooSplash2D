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

    public float nextSpawn;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GhostSpawn", spawnRate, nextSpawn);
    }

    // Update is called once per frame
    public void GhostSpawn()
    {
            GameObject ps = GameObject.FindGameObjectWithTag("PortalSpawn");
            spawnX = ps.transform.position.x;
            whereEnemySpawn = new Vector2(spawnX, transform.position.y);
            Instantiate(enemy, whereEnemySpawn, Quaternion.identity);
        if (stopSpawn)
        {
            CancelInvoke("GhostSpawn");
        }
     
    }
}
