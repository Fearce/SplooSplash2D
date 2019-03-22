﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{

    public float speed;
    public float distance;

    public Transform target;

    public Rigidbody2D PlayerBody;
    public float JumpForce = 8f;

    private bool IsJumping;
    // Start is called before the first frame update
    void Start()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = new Quaternion();
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < distance && IsJumping == false)
        {
            PlayerBody.AddForce(transform.up * JumpForce * 100);
            IsJumping = true;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Obstacles")
        {
            // Debug.Log("touching floor");
            IsJumping = false;
        }

        if (coll.gameObject.tag == "Player")
        {
            Debug.Log("Collision with Player");
            Destroy(coll.gameObject);
            //points++;
            //GameObject.FindGameObjectWithTag("Text").GetComponent<Text>().text = "Score: " + points;
            //score.text = "Score: " + points;
        }

    }
}
