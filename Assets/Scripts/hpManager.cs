using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class hpManager : MonoBehaviour
{
    public int lifes;

    public Text hpText;

    private Animator hitAnim;

    private void Start()
    {
        hitAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        hpText.text = "HP: " + lifes;

        if (lifes == 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Cherry")
        {
            Destroy(other.gameObject);
            lifes++;
            Debug.Log(lifes);
            hpText.text = "HP: " + lifes;
        }

        if(other.gameObject.tag == "Enemy")
        {
            hitAnim.SetBool("hit", true);
            lifes--;
            Debug.Log(lifes);
            hpText.text = "HP: " + lifes;
            //hitAnim.SetBool("hit", false);
        }
    }
}
