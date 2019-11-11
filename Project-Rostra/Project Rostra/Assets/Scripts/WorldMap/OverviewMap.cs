using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverviewMap : MonoBehaviour
{
    public Image[] image;

    void Start()
    {
        image[0].enabled = false;
        image[1].enabled = false;
        image[2].enabled = false;
        image[3].enabled = false;
        image[4].enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("y"))
        {
            image[0].enabled = true;
        }
        else if(Input.GetKeyDown("u"))
        {
            image[1].enabled = true;
        }
        else if (Input.GetKeyDown("i"))
        {
            image[2].enabled = true;
            image[3].enabled = true;
            image[4].enabled = true;
        }
    }
}
