using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts
{
    /// <summary>
    /// Script for Gun & Bullet stuff
    /// Der kan evt addes flere properties der kan bruges f.eks Spread til RPG/shotguns eller noget.
    /// Måske AK47 skulle have inaccuracy også?
    /// </summary>
    public class BulletScript : MonoBehaviour
    {
        public int Damage;
        public int BulletForce;
        public float ReloadSpeed;

        public float velX = 5f;
        private float velY = 0f;
        private Rigidbody2D rb;
        Stopwatch sw = new Stopwatch();

        // Start is called before the first frame update
        void Start()
        {
            // Set Weapon properties
            switch (PlayerScript.CurrentWeapon)
            {
                // MaxAmmo is set in PlayerScript.cs SetWeapon()
                case GunTypes.Pistol1:
                    Damage = 50;
                    BulletForce = 10;
                    Joystick.FireRate = 0.8f;
                    ReloadSpeed = 3;
                    break;
                case GunTypes.Pistol2:
                    Damage = 55;
                    BulletForce = 12;
                    Joystick.FireRate = 0.55f;
                    ReloadSpeed = 2.5f;
                    break;
                case GunTypes.Pistol3:
                    Damage = 30;
                    BulletForce = 8;
                    Joystick.FireRate = 0.2f;
                    ReloadSpeed = 1.5f;
                    break;
                case GunTypes.Pistol4:
                    break;
                case GunTypes.Pistol5:
                    break;
                case GunTypes.Pistol6:
                    break;
                case GunTypes.Pistol7:
                    break;
                case GunTypes.AK47:
                    break;
                case GunTypes.Gattlegun1:
                    break;
                case GunTypes.Gattlegun2:
                    break;
                case GunTypes.Machinegun1:
                    break;
                case GunTypes.Machinegun2:
                    break;
                case GunTypes.Machinegun3:
                    break;
                case GunTypes.Machinegun4:
                    break;
                case GunTypes.Machinegun5:
                    break;
                case GunTypes.Machinegun6:
                    break;
                case GunTypes.Machinegun7:
                    break;
                case GunTypes.Machinegun8:
                    break;
                case GunTypes.Machinegun9:
                    break;
                case GunTypes.Machinegun10:
                    break;
                case GunTypes.Machinegun11:
                    break;
                case GunTypes.Machinegun12:
                    break;
                case GunTypes.Machinegun13:
                    break;
                case GunTypes.Revolver1:
                    break;
                case GunTypes.Revolver2:
                    break;
                case GunTypes.Rifle1:
                    break;
                case GunTypes.Rifle2:
                    break;
                case GunTypes.RPG1:
                    break;
                case GunTypes.RPG2:
                    break;
                case GunTypes.Scout1:
                    break;
                case GunTypes.Sniper1:
                    break;
                case GunTypes.Sniper2:
                    break;
                case GunTypes.Sniper3:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            //velocity = Vector2.zero;
            // Ignore bullet collision with Player
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
                Debug.Log($"Bullet fr {Joystick.FireRate}, bf {BulletForce}, dmg {Damage} - survived for {sw.Elapsed}");
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
}
