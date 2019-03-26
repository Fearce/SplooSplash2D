using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MovementScript : MonoBehaviour
{
    public int lifes;

    public Text hpText;
    public GameObject deadPanel;

    public static Vector2 StickDirection { get; set; }
    private float StickX => StickDirection.x;
    private float StickY => StickDirection.y;
    public float MoveSpeed = 0.05f;
    public float JumpForce = 8f;

    private bool IsJumping;

    private bool IsDead;

    private Animator myAnim;

    public Transform Player { get; set; }

    public Rigidbody2D PlayerBody;
    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponent<Transform>();
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        hpText.text = "HP: " + lifes;

        if (lifes == 0)
        {
            IsDead = true;
            deadPanel.SetActive(true);
        }

        if (!IsDead)
        {
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            cam.transform.position = new Vector3(Player.position.x, cam.transform.position.y, -10f);

            gameObject.transform.rotation = new Quaternion();
            // Right
            if (StickX > 0.2 || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                gameObject.transform.localScale = new Vector3(3, 3, 1);
                Player.position = new Vector3((Player.position.x + MoveSpeed), Player.position.y);
            }
            // Left
            if (StickX < -0.2 || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                gameObject.transform.localScale = new Vector3(-3, 3, 1);
                Player.position = new Vector3(Player.position.x - MoveSpeed, Player.position.y);
            }

            // Up
            if ((StickY > 0.5 || (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))) && IsJumping == false )
            {
                PlayerBody.AddForce(transform.up * JumpForce * 100);
                IsJumping = true;
                //Player.position = new Vector3(Player.position.x - MoveSpeed, Player.position.y);
            }
        }
        else
        {
            myAnim.SetBool("death", true);
        }
    }


    // Dummy code for future use (from cloud jumper)
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Obstacles")
        {
            // Debug.Log("touching floor");
            IsJumping = false;
        }

        if(coll.gameObject.tag == "Enemy")
        {
            if (lifes > 0)
            {
                lifes--;
            }
            Debug.Log(lifes);
            hpText.text = "HP: " + lifes;

            if(lifes == 0)
            {
                Destroy(gameObject, 2.1f);
            }
            
        }

        if (coll.gameObject.tag == "coin")
        {
            Debug.Log("Collision with coin");
            
            Destroy(coll.gameObject);
            //points++;
            //GameObject.FindGameObjectWithTag("Text").GetComponent<Text>().text = "Score: " + points;
            //score.text = "Score: " + points;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Cherry")
        {
            Destroy(other.gameObject);
            lifes++;
            Debug.Log(lifes);
            hpText.text = "HP: " + lifes;
        }
    }

}
