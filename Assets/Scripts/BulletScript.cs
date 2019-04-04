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
                    ReloadSpeed = 2;
                    break;
                case GunTypes.Tec9:
                    Damage = 30;
                    BulletForce = 8;
                    Joystick.FireRate = 0.25f;
                    ReloadSpeed = 1.5f;
                    break;
                case GunTypes.AK47:
                    Damage = 50;
                    BulletForce = 13;
                    Joystick.FireRate = 0.25f;
                    ReloadSpeed = 2f;
                    break;
                case GunTypes.SMG:
                    Damage = 20;
                    BulletForce = 8;
                    Joystick.FireRate = 0.15f;
                    ReloadSpeed = 2.5f;
                    break;
                case GunTypes.AutoSniper:
                    Damage = 100;
                    BulletForce = 15;
                    Joystick.FireRate = 0.5f;
                    ReloadSpeed = 3f;
                    break;
                case GunTypes.SCAR20:
                    Damage = 50;
                    BulletForce = 11;
                    Joystick.FireRate = 0.22f;
                    ReloadSpeed = 2f;
                    break;
                case GunTypes.M1918:
                    Damage = 90;
                    BulletForce = 12;
                    Joystick.FireRate = 0.3f;
                    ReloadSpeed = 4f;
                    break;
                case GunTypes.Minigun:
                    Damage = 30;
                    BulletForce = 8;
                    Joystick.FireRate = 0.08f;
                    ReloadSpeed = 4f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameObject[] bullets = GameObject.FindGameObjectsWithTag("BulletToRight");
            //velocity = Vector2.zero;
            // Ignore bullet collision with Player
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(),GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
            if (bullets != null)
                foreach (var bullet in bullets)
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
                }

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
