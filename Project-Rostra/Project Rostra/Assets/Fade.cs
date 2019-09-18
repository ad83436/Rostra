using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Fade : MonoBehaviour
{
    private Image thisImage;
    // Start is called before the first frame update
    void Start()
    {
        thisImage = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        thisImage.fillAmount -= 0.02f;
        if(thisImage.fillAmount<=0.0f)
        {
            gameObject.SetActive(false);
        }
    }
}
