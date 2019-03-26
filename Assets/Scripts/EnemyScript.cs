﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public float speed;
    public float distance;
    public Splatter splatter;

    public Transform target;

    public Rigidbody2D PlayerBody;
    public float JumpForce = 8f;
    public int Health = 100;

    private bool IsJumping;

    private Animator pacAnim;
    // Start is called before the first frame update
    void Start()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        pacAnim = target.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            Debug.Log("ded ghost");
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, -3.29f);
            Instantiate(BloodSplash, pos, Quaternion.identity);
            Splatter splatterObj = (Splatter)Instantiate(splatter, pos, Quaternion.identity);
            Destroy(gameObject);
        }

        gameObject.transform.rotation = new Quaternion();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }

        if (target != null && (Vector2.Distance(transform.position, target.position) < distance && IsJumping == false))
        {
            PlayerBody.AddForce(transform.up * JumpForce * 100);
            IsJumping = true;
        }
    }

    public GameObject BloodSplash;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Obstacles")
        {
            // Debug.Log("touching floor");
            IsJumping = false;
        }

        if (coll.gameObject.tag.Contains("Bullet"))
        {
            Debug.Log("ghost takin dmg!");
            Health -= coll.gameObject.GetComponent<BulletScript>().Damage;
        }

        if (coll.gameObject.tag == "Player")
        {
            Debug.Log("Collision with Player");
            //Destroy(coll.gameObject, 2.1f);
            //points++;
            //GameObject.FindGameObjectWithTag("Text").GetComponent<Text>().text = "Score: " + points;
            //score.text = "Score: " + points;
        }

    }
}
