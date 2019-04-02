using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    private GameObject Target;
    private bool Fighting = false;

    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine("WaitForPlayer");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WaitForPlayer()
    {
        while (true)
        {
            // Wait for player to reach x == 200 before beginning AI
            if (Target.transform.position.x > 200 && !Fighting)
            {
                StartCoroutine("FightPlayer");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    IEnumerator FightPlayer()
    {
        if (!Fighting)
        {
            Fighting = true;
            while (Fighting)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
