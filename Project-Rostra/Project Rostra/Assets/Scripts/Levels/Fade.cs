using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    private Image thisImage;
    public WMEnemy enemyHolder;
    private bool fadeOut;
    private bool transitionToBattle;
    private bool transitionToVictory;
    private bool transitionToDefeat;
    private bool transitionToWorldMap;
    private bool transitionToEndTest;
    private bool canGoToSurvey;

    public VictoryScreen victoryPanel;
    public GameObject defeatPanel;
    public GameObject endTestPanel;
    private UIBTL uiBtl;


    void Start()
    {
        thisImage = gameObject.GetComponent<Image>();
        fadeOut = false;
        transitionToBattle = false;
        transitionToVictory = false;
        transitionToDefeat = false;
        transitionToWorldMap = false;
        transitionToEndTest = false;
        canGoToSurvey = false;
        uiBtl = UIBTL.instance;

        if (endTestPanel)
        {
            endTestPanel.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canGoToSurvey)
        {
            Application.OpenURL("https://drive.google.com/open?id=1HRdDsVZKcSQmuYScziiHemY-gXOa5k7R1_4w4gTy1oc");
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            FlipFadeToBattle();
        }

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
                if (transitionToBattle)
                {
                    transitionToBattle = false;
                    TransitionIntoBattle();
                    fadeOut = false;
                }
                else if (transitionToVictory)
                {
                    transitionToVictory = false;
                    TransitionIntoVictory();
                    uiBtl.StartShowingEndScreen(true); //Show the victory screen stats now
                }
                else if (transitionToDefeat)
                {
                    transitionToDefeat = false;
                    TransitionIntoDefeat();
                    uiBtl.StartShowingEndScreen(false); //Show the defeat screen
                }
                else if (transitionToWorldMap)
                {
                    transitionToWorldMap = false;
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Queue Scene"));
                }
                else if (transitionToEndTest)
                {
                    transitionToEndTest = false;
                    endTestPanel.gameObject.SetActive(true);
                    canGoToSurvey = true;
                }
            }
        }
    }

    public void FlipFadeToBattle(WMEnemy enemyCollidingWithPlayer)
    {
        enemyHolder = enemyCollidingWithPlayer;
        fadeOut = true;
        transitionToBattle = true;
    }
    //Two version of flip fade, one for controlled situations where the WM is assigned from the editor and one for the world map
    public void FlipFadeToBattle()
    {
        fadeOut = !fadeOut;
        transitionToBattle = true;
    }

    public void FlipFadeToVictory()
    {
        fadeOut = !fadeOut;
        transitionToVictory = true;
    }

    public void FlipFadeToDefeat()
    {
        fadeOut = !fadeOut;
        transitionToDefeat = true;
    }

    public void TransitionIntoBattle()
    {
        enemyHolder.TransitionIntoBattle();
    }

    public void TransitionIntoVictory()
    {
        victoryPanel.VictoryFadeIn();
    }

    public void TransitionIntoDefeat()
    {
        defeatPanel.gameObject.SetActive(true);
    }

    public void TransitionBackToWorldMapFromBattle()
    {
        fadeOut = !fadeOut;
        transitionToWorldMap = true;
    }

    public void FlipToEndTest()
    {
        fadeOut = !fadeOut;
        transitionToEndTest = true;
    }
}