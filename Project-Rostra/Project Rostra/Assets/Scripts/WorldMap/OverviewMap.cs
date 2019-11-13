using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//oo oo ah ah
public class OverviewMap : MonoBehaviour
{
    public Image[] image;
    public Text text;
    public GameObject savepoint;

    void Start()
    {
        savepoint.SetActive(true);
        text.enabled = false;
        image[0].enabled = false;
        image[1].enabled = false;
        image[2].enabled = false;
        image[3].enabled = false;
        image[4].enabled = false;
        image[5].enabled = false;
        image[6].enabled = false;
        image[7].enabled = false;
        image[8].enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (savepoint == true)
            {
                text.enabled = true;
                image[0].enabled = true;
                image[1].enabled = true;
                image[2].enabled = true;
                image[3].enabled = true;
                image[4].enabled = true;
                image[5].enabled = true;
                image[6].enabled = true;
                image[7].enabled = true;
                image[8].enabled = true;
            }
        }
    }
}
