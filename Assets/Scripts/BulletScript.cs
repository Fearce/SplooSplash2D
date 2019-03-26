using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BulletScript : MonoBehaviour
{
    public int Damage = 50;
    public float velX = 5f;
    private float velY = 0f;
    private Rigidbody2D rb;
    Stopwatch sw = new Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        //velocity = Vector2.zero;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(),GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
        rb = GetComponent<Rigidbody2D>();
        sw.Start();
    }

    // Update is called once per frame
    void Update()
    {
       //rb.velocity = new Vector2(velX, velY);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Points")
        {
            string otherObject = collision.gameObject.name;
            Debug.Log("Bullet collided with " + otherObject);
            Destroy(gameObject);
            sw.Stop();
            Debug.Log($"Bullet survived for {sw.Elapsed}");
            if (collision.gameObject.tag == "Enemy")
            {
                //Destroy(collision.gameObject);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Points")
        {
            Debug.Log("Colliosion on bullet detected");
            Destroy(gameObject);
        }
    }
}
