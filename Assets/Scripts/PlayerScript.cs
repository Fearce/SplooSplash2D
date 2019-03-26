using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Player script
    /// </summary>
    public class PlayerScript : MonoBehaviour
    {
        public GunTypes CurrentWeapon = GunTypes.Pistol1;

        public int Lives;

        public Text HpText;
        public GameObject DeadPanel;

        // StickDirection is set in Joystick.cs on drag
        public static Vector2 StickDirection { get; set; }
        private float StickX => StickDirection.x;
        private float StickY => StickDirection.y;

        // Default speed and jump force
        public float MoveSpeed = 0.05f;
        public float JumpForce = 8f;
        
        private bool isJumping;

        private bool isDead;

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

        /// <summary>
        /// Resets isJumping every third second.
        /// Can be improved upon.
        /// </summary>
        DateTime lastJump = DateTime.MinValue;

        void FixJumpingBug()
        {
            if (PlayerBody.velocity.y < 1f && (lastJump == DateTime.MinValue || DateTime.UtcNow > lastJump.AddSeconds(3)))
            {
                lastJump = DateTime.UtcNow;
                isJumping = false;
                Debug.Log("jump fixed");
            }
        }

        // Update is called once per frame
        void Update()
        {
            FixJumpingBug();
            HpText.text = "HP: " + Lives;

            // Ded on 0 lives
            if (Lives == 0)
            {
                isDead = true;
                DeadPanel.SetActive(true);
            }

            // If not ded
            if (!isDead)
            {
                // Center camera on player
                GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
                cam.transform.position = new Vector3(Player.position.x, cam.transform.position.y, -10f);

                // Fix rotation so we don't tilt
                gameObject.transform.rotation = new Quaternion();

                // Movement
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
                if ((StickY > 0.5 || (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))) && isJumping == false )
                {
                    PlayerBody.AddForce(transform.up * JumpForce * 100);
                    isJumping = true;
                    //Player.position = new Vector3(Player.position.x - MoveSpeed, Player.position.y);
                }
            }
            else
            {
                // Set death animation
                myAnim.SetBool("death", true);
            }
        }


        // Collision
        void OnCollisionEnter2D(Collision2D coll)
        {
            // Reset isJumping when touching floor to allow new jump
            if (coll.gameObject.tag == "Obstacles")
            {
                // Debug.Log("touching floor");
                isJumping = false;
            }

            // On hitting ghost
            if(coll.gameObject.tag == "Enemy")
            {
                if (Lives > 0)
                {
                    Lives--;
                }
                Debug.Log(Lives);
                HpText.text = "HP: " + Lives;

                if(Lives == 0)
                {
                    // Destroy game object in 2.1 seconds
                    Destroy(gameObject, 2.1f);
                }
            
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            // Hitting cherries. Should points be in here too instead of separate script?
            if (other.gameObject.tag == "Cherry")
            {
                Destroy(other.gameObject);
                Lives++;
                Debug.Log(Lives);
                HpText.text = "HP: " + Lives;
            }
        }

    }
}
