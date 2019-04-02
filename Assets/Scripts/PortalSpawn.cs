using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PortalSpawn : MonoBehaviour
{
    public GameObject enemy;
    private float spawnX;
    private Vector2 whereEnemySpawn;

    public float spawnRate = 1f;

    private float nextSpawn = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            GameObject ps = GameObject.FindGameObjectWithTag("PortalSpawn");
            spawnX = ps.transform.position.x;
            whereEnemySpawn = new Vector2(spawnX, transform.position.y);
            Instantiate(enemy, whereEnemySpawn, Quaternion.identity);
        }
    }
}
