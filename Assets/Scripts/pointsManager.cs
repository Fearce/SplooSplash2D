using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pointsManager : MonoBehaviour
{
    public int points;

    public Text pointText;

    private void Update()
    {
        pointText.text = "POINTS: " + points;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Points")
        {
            Destroy(other.gameObject);
            points++;
            pointText.text = "POINTS: " + points;
        }
    }
}
