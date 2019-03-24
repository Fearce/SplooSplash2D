using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float velX = 5f;
    private float velY = 0f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //velocity = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
       rb.velocity = new Vector2(velX, velY);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        string otherObject = collision.gameObject.name;
        Debug.Log("Bullet collided with " + otherObject);
        Destroy(gameObject);
        if (collision.gameObject.tag == "Enemy")
        {
            //Destroy(collision.gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Colliosion on bullet detected");
        Destroy(gameObject);
    }
}
