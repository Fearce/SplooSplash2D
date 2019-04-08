using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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

        // Sound Effect Objects
        public GameObject pointsSFX;
        public GameObject healthSFX;
        public GameObject newWeaponSFX;
        public GameObject playerDeathSFX;
        public GameObject hurtSFX;

        // High Score
        public Text highScore;

        // Start is called before the first frame update
        void Start()
        {
            // Start timer
            if (EndScreen.Stopwatch == null)
            {
                EndScreen.Stopwatch = new Stopwatch();
                EndScreen.Stopwatch.Start();
            }

            // HighScore
            highScore = GameObject.FindGameObjectWithTag("HighScoreText").GetComponent<Text>();
            highScore.text = "Highscore: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
            // DeadPanel = GameObject.FindGameObjectWithTag("DeadBoi");

            sprites = Resources.LoadAll<Sprite>("Guns/drawn-gun-sprite-sheet-542860-7331932");
            pointText = GameObject.FindGameObjectWithTag("PointsText").GetComponent<Text>();
            Weapon = GameObject.FindGameObjectWithTag("Weapon");
            DeadPanel = GameObject.FindGameObjectWithTag("DeadBoi");
            DeadPanel.SetActive(false);
            Player = GetComponent<Transform>();
            HpText = GameObject.FindGameObjectWithTag("HPText").GetComponent<Text>();
            PlayerBody = gameObject.GetComponent<Rigidbody2D>();
            myAnim = GetComponent<Animator>();
            HurtPanel = GameObject.FindGameObjectWithTag("HurtPanel");
            HurtPanel.SetActive(false);
            SetWeapon("Begin!", GunTypes.Pistol1, sprites[18], 8);  //pistol4
            //GameObject.FindGameObjectWithTag("CrosshairLeft").SetActive(false);

            if (EndScreen.score != 0)
            {
                points = EndScreen.score;
            }

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
            // Position weapon
            var pos = gameObject.transform.position;
            GameObject.FindGameObjectWithTag("Weapon").transform.position = new Vector3(pos.x,(float) (pos.y-0.221));

            // Reset statustext
            if (weaponUnlocked + 2 < Time.time &&
                GameObject.FindGameObjectWithTag("StatusText").transform.localScale == Vector3.one &&
                GameObject.FindGameObjectWithTag("StatusText").GetComponent<Text>().text != "Reloading!")
            {
                GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.zero;
                weaponUnlocked = Time.time;
            }

            pointText.text = "POINTS: " + points;
            FixJumpingBug();
            HpText.text = "HP: " + Lives;

            // Ded on 0 lives
            if (Lives == 0)
            {
                isDead = true;
                DeadPanel.SetActive(true);
                GameObject.FindGameObjectWithTag("FinalScoreText").GetComponent<Text>().text = "SCORE: " + points;
            }

            // If not ded
            if (!isDead)
            {
                // Center camera on player
                GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
                cam.transform.position = new Vector3(Player.position.x, Player.position.y, -10f);

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
                    lastJump = DateTime.UtcNow;
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

        private float weaponUnlocked;
        private Sprite[] sprites;


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
                    PlayerBody.AddForce(-transform.up*500);
                }
                else if (coll.gameObject.name == "TrapFloor")
                {
                    PlayerBody.AddForce(transform.up * 500);
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
                    Instantiate(hurtSFX, transform.position, Quaternion.identity);
                    Lives--;
                    
                }
                Debug.Log(Lives);
                HpText.text = "HP: " + Lives;

                if(Lives == 0)
                {
                    Handheld.Vibrate();
                    Instantiate(playerDeathSFX, transform.position, Quaternion.identity);
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
                GameObject sound = Instantiate(pointsSFX, transform.position, Quaternion.identity);
                Destroy(sound, 3);
                Destroy(other.gameObject);
                points++;
                pointText.text = "POINTS: " + points;
                if (points % 50 == 0)
                {
                    Lives++;
                    GameObject.FindGameObjectWithTag("StatusText").GetComponent<Text>().text = "Life gained!";
                    GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.one;
                    weaponUnlocked = Time.time;
                }

                // High Score
                if (points > PlayerPrefs.GetInt("HighScore", 0))
                {
                    PlayerPrefs.SetInt("HighScore", points);
                    highScore.text = "Highscore: " + points.ToString();
                    GameObject.FindGameObjectWithTag("StatusText").GetComponent<Text>().text = "NEW HIGHSCORE!";
                    GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.one;

                    // High Score Menu
                    GameObject.FindGameObjectWithTag("HighScoreTextMenu").GetComponent<Text>().text = points.ToString();
                }
            }
            // Hitting cherries
            if (other.gameObject.tag == "Cherry")
            {
                Instantiate(healthSFX, transform.position, Quaternion.identity);
                Destroy(other.gameObject);
                Lives++;
                Debug.Log(Lives);
                HpText.text = "HP: " + Lives;
                GameObject.FindGameObjectWithTag("StatusText").GetComponent<Text>().text = "Life gained!";
                GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.one;
                weaponUnlocked = Time.time;
            }

            // Hitting weapons
            if (other.gameObject.tag == "GunPickup")
            {
                Instantiate(newWeaponSFX, transform.position, Quaternion.identity);

                switch (other.gameObject.name)
                {
                    case "Tec9":
                        SetWeapon("Tec9 unlocked!",GunTypes.Tec9, other ,12);  //Tec9
                        break;
                    case "SMG":
                        SetWeapon("SMG unlocked!", GunTypes.SMG, other, 40);  //SMG
                        break;
                    case "AK47":
                        SetWeapon("AK47 unlocked!", GunTypes.AK47, other, 30);  //AK47
                        break;
                    case "AutoSniper":
                        SetWeapon("AutoSniper unlocked!", GunTypes.AutoSniper, other, 10);  //AutoSniper
                        break;
                    case "SCAR20":
                        SetWeapon("SCAR-20 unlocked!", GunTypes.SCAR20, other, 30);  //SCAR-20
                        break;
                    case "M1918":
                        SetWeapon("M1918 unlocked!", GunTypes.M1918, other, 55);  //M1918
                        break;
                    case "Minigun":
                        SetWeapon("Minigun unlocked!", GunTypes.Minigun, other, 100);  //Minigun
                        break;
                    case "RevolverGrey":
                        SetWeapon("Revolver unlocked!", GunTypes.RevolverGrey, other, 6);  //Revolver
                        break;
                    case "MP7":
                        SetWeapon("MP7 unlocked!", GunTypes.MP7, other, 40);  //MP7
                        break;
                    case "Galil":
                        SetWeapon("Galil unlocked!", GunTypes.Galil, other, 25);  //Galil
                        break;
                    case "Scout":
                        SetWeapon("Scout unlocked!", GunTypes.Scout, other, 10);  //Scout
                        break;
                    case "SemiColt":
                        SetWeapon("Colt unlocked!", GunTypes.SemiColt, other, 30);  //SemiColt
                        break;
                    case "SniperGrey":
                        SetWeapon("Sniper unlocked!", GunTypes.SniperGrey, other, 10);  //SniperGrey
                        break;
                    case "GattlingGun":
                        SetWeapon("Gattlinggun unlocked!", GunTypes.GattlingGun, other, 100);  //GattlingGun
                        break;
                    case "PistolSMG":
                        SetWeapon("Pistol unlocked!", GunTypes.PistolSMG, other, 30);  //PistolSMG
                        break;
                    case "SMGGrey6":
                        SetWeapon("SMG unlocked!", GunTypes.SMGGrey6, other, 35);  //SMGGrey6
                        break;
                    case "MachinegunOldSchool":
                        SetWeapon("Machinegun unlocked!", GunTypes.MachinegunOldSchool, other, 35);  //MachinegunOldSchool
                        break;
                    case "RifleGrey":
                        SetWeapon("Rifle unlocked!", GunTypes.RifleGrey, other, 20);  //RifleGrey
                        break;
                    case "MachineGrey2":
                        SetWeapon("Machinegun unlocked!", GunTypes.MachineGrey2, other, 40);  //MachineGrey2
                        break;
                    case "SniperBrown":
                        SetWeapon("Sniper unlocked!", GunTypes.SniperBrown, other, 10);  //SniperBrown
                        break;
                    case "PistolGod":
                        SetWeapon("Pistol unlocked!", GunTypes.PistolGod, other, 200);  //PistolGod
                        break;

                }
            }
        }

        void SetWeapon(string statusText, GunTypes gunType, Collider2D sprite, int maxAmmo)
        {
            GameObject.FindGameObjectWithTag("StatusText").GetComponent<Text>().text = statusText;
            GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.one;
            weaponUnlocked = Time.time;
            CurrentWeapon = gunType;
            Weapon.GetComponent<SpriteRenderer>().sprite = sprite.gameObject.GetComponent<SpriteRenderer>().sprite;
            Joystick.MaxAmmo = maxAmmo;
            Joystick.AmmoCount = maxAmmo;
            Destroy(sprite.gameObject);
        }

        void SetWeapon(string statusText, GunTypes gunType, Sprite sprite, int maxAmmo)
        {
            GameObject.FindGameObjectWithTag("StatusText").GetComponent<Text>().text = statusText;
            GameObject.FindGameObjectWithTag("StatusText").transform.localScale = Vector3.one;
            weaponUnlocked = Time.time;
            CurrentWeapon = gunType;
            Weapon.GetComponent<SpriteRenderer>().sprite = sprite;
            Joystick.MaxAmmo = maxAmmo;
            Joystick.AmmoCount = maxAmmo;
        }
    }
}
