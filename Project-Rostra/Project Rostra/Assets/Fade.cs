using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Fade : MonoBehaviour
{
    private Image thisImage;
    public WMEnemy enemyHolder;
    private bool fadeOut;
    // Start is called before the first frame update
    void Start()
    {
        thisImage = gameObject.GetComponent<Image>();
        fadeOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Fadeout or Fadein?
        if (!fadeOut)
        {
            thisImage.fillAmount -= 0.02f;
        }
        else
        {
            thisImage.fillAmount += 0.02f;
            if (thisImage.fillAmount >= 1.0f)
            {
                TransitionIntoBattle();
                gameObject.SetActive(false);
            }
        }
    }

    public void flipFade()
    {
        fadeOut = !fadeOut;
    }

    public void TransitionIntoBattle()
    {
        enemyHolder.TransitionIntoBattle();
    }
}
