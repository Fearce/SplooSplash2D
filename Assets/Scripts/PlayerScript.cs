using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Player script
    /// </summary>
    public class PlayerScript : MonoBehaviour
    {
        public static bool FacingRight = true;
        public static GunTypes CurrentWeapon = GunTypes.Pistol1;
        private GameObject Weapon;

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
        public GameObject HurtPanel;

        public Transform Player { get; set; }

        public Rigidbody2D PlayerBody;
        // Start is called before the first frame update
        void Start()
        {
            pointText = GameObject.FindGameObjectWithTag("PointsText").GetComponent<Text>();
            Weapon = GameObject.FindGameObjectWithTag("Weapon");
            //DeadPanel = GameObject.FindGameObjectWithTag("DeadBoi");
            //DeadPanel.SetActive(false);
            Player = GetComponent<Transform>();
            HpText = GameObject.FindGameObjectWithTag("HPText").GetComponent<Text>();
            PlayerBody = gameObject.GetComponent<Rigidbody2D>();
            myAnim = GetComponent<Animator>();
            HurtPanel = GameObject.FindGameObjectWithTag("HurtPanel");
            HurtPanel.SetActive(false);
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
                // Debug.Log("jump fixed");
            }
        }

        // Update is called once per frame
        void Update()
        {
            pointText.text = "POINTS: " + points;
            if (points % 50 == 0)
            {
                SetWeapon();
            }
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
                    FacingRight = true;
                    gameObject.transform.localScale = new Vector3(3, 3, 1);
                    Player.position = new Vector3((Player.position.x + MoveSpeed), Player.position.y);
                }
                // Left
                if (StickX < -0.2 || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    FacingRight = false;
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

        private void SetWeapon()
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Guns/drawn-gun-sprite-sheet-542860-7331932");
            if (points >= 100 && Weapon.GetComponent<SpriteRenderer>().sprite != sprites[18])
            {
                CurrentWeapon = GunTypes.Pistol3;
                Weapon.GetComponent<SpriteRenderer>().sprite = sprites[18];
                Joystick.MaxAmmo = 20;
                Joystick.AmmoCount = Joystick.MaxAmmo;

            }
            else if (points < 100 && points >= 50 && Weapon.GetComponent<SpriteRenderer>().sprite != sprites[17])
            {
                CurrentWeapon = GunTypes.Pistol2;
                Weapon.GetComponent<SpriteRenderer>().sprite = sprites[17];
                Joystick.MaxAmmo = 12;
                Joystick.AmmoCount = Joystick.MaxAmmo;
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
            if (coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Trap") 
            {
                if (coll.gameObject.name == "TrapRoof")
                {
                    Debug.Log("pew");
                    PlayerBody.AddForce(-transform.up*1000);
                }
                else if (coll.gameObject.name == "TrapFloor")
                {
                    PlayerBody.AddForce(transform.up * 1000);
                }
                isJumping = false;
                if (Lives > 0)
                {
                    //Starts the method HurtPanelActive
                    StartCoroutine("HurtPanelActive");
                    //Starts the method Hurt
                    StartCoroutine("Hurt");
                    //Set the HurtPanel to blink when hit
                    HurtPanelActive();
                    //Set the parameter hit to true, after 1.5 sec to false
                    Hurt();
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

        IEnumerator HurtPanelActive()
        {
            if (Lives != 1)
            {
                HurtPanel.SetActive(true);
                HurtPanel.transform.localScale = Vector3.one;
                Debug.Log("Hit");
                yield return new WaitForSeconds(0.1f);
                HurtPanel.SetActive(false);
                HurtPanel.transform.localScale = Vector3.zero;
            }
            else
            {
                HurtPanel.SetActive(false);
                HurtPanel.transform.localScale = Vector3.zero;
            }
        }

        IEnumerator Hurt()
        {
            if (Lives != 1)
            {
                myAnim.SetBool("hit", true);
                //HurtPanel.SetActive(true);
                Debug.Log("Hit");
                yield return new WaitForSeconds(1f);
                //HurtPanel.SetActive(false);
                myAnim.SetBool("hit", false);
            }
            else
            {
                myAnim.SetBool("hit", false);
            }
        }

        public int points;

        public Text pointText;

        public void OnTriggerEnter2D(Collider2D other)
        {
            // Hitting points
            if (other.gameObject.tag == "Points")
            {
                Destroy(other.gameObject);
                points++;
                pointText.text = "POINTS: " + points;
            }
            // Hitting cherries
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
