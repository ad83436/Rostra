using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject hilighter;
    public GameObject hPos0;
    public GameObject hPos1;
    private int menuIndex;
    public Image fade;
    private bool startFading;


    void Start()
    {
        Cursor.visible = false;
        hilighter.transform.localPosition = hPos0.transform.localPosition;
        menuIndex = 0;
        fade.fillAmount = 0.0f;
        startFading = false;
    }

    void Update()
    {
        switch(menuIndex)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    hilighter.transform.localPosition = hPos1.transform.localPosition;
                    menuIndex = 1;
                }
                else if(Input.GetKeyDown(KeyCode.Space))
                {
                    startFading = true;
                }
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    hilighter.transform.localPosition = hPos0.transform.localPosition;
                    menuIndex = 0;
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    Application.Quit();
                }
                break;
        }

        if(startFading)
        {
            fade.fillAmount += 0.02f;
            if(fade.fillAmount>=1.0f)
            {
                SceneManager.LoadScene("Demo");
            }
        }
    }
}
