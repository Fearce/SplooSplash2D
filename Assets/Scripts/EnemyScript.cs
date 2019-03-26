using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Script for all Enemies
    /// </summary>
    public class EnemyScript : MonoBehaviour
    {

        // Hmm, setter vi nogensinde Speed & Distance? Så vidt jeg kan se er de altid 0
        public float Speed;
        public float Distance;
        
        public Splatter Splatter;
        public Transform Target;

        public Rigidbody2D GhostBody;
        public float JumpForce = 8f;
        public int Health = 100;

        private bool isJumping;

        private Animator pacAnim;
        // Start is called before the first frame update
        void Start()
        {
            GhostBody = gameObject.GetComponent<Rigidbody2D>();
            Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            pacAnim = Target.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            // If health <= 0 ghost is dead
            if (Health <= 0)
            {
                Debug.Log("ded ghost");
                Vector3 pos = new Vector3(transform.position.x, transform.position.y, -3.29f);
                Instantiate(BloodSplash, pos, Quaternion.identity);
                Splatter splatterObj = (Splatter)Instantiate(Splatter, pos, Quaternion.identity);
                Destroy(gameObject);
            }

            // Make sure ghost isn't tilted
            gameObject.transform.rotation = new Quaternion();

            // Move towards target
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);
            }

            // Jump if target is above
            if (Target != null && (Vector2.Distance(transform.position, Target.position) < Distance && isJumping == false))
            {
                GhostBody.AddForce(transform.up * JumpForce * 100);
                isJumping = true;
            }
        }

        public GameObject BloodSplash;

        void OnCollisionEnter2D(Collision2D coll)
        {
            // Reset isJumping when touching the floor
            if (coll.gameObject.tag == "Obstacles")
            {
                // Debug.Log("touching floor");
                isJumping = false;
            }

            // Take damage from bullet
            if (coll.gameObject.tag.Contains("Bullet"))
            {
                Debug.Log("ghost takin dmg!");
                Health -= coll.gameObject.GetComponent<BulletScript>().Damage;
            }

            // When colliding with player (redundant?)
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
}
