using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    private BattleManager btlManager;
    private bool screenActive;
    private bool fadingIn;
    public Fade fadePanelToWorldMap;

    public Image thisImage;
    public Text victoryTextFore;
    public Text victoryTextBack;
    private Color panelItemsColor;
    public Text fargasLevelUpBack;
    public Text fargasLevelUpFore;
    public Image fargasPortrait;
    public Image fargasPortraitBack;
    public Image fargasHP;
    public Image fargasHPBack;
    public Image fargasMP;
    public Image fargasMPBack;
    public Image fargasExp;
    public Image fargasExpBack;
    private float expStep;
    private float fargasCurrentExp;
    private float fargasMaxExp;
    private float fargasExpStep; //Used to know by how much to increase the exp bar == 1/maxExp
    private int fargasExpGain;
    private bool fargasAddinExp; //Used in update to increase EXP bar
    private bool startFadingFargasIn;

    public Text freaLevelUpBack;
    public Text freaLevelUpFore;
    public Image freaPortrait;
    public Image freaPortraitBack;
    public Image freaHP;
    public Image freaHPBack;
    public Image freaMP;
    public Image freaMPBack;
    public Image freaExp;
    public Image freaExpBack;
    private float freaCurrentExp;
    private float freaMaxExp;
    private float freaExpStep;
    private int freaExpGain;
    private bool freaAddinExp;
    private bool startFadingFreaIn;

    public Text arcelusLevelUpBack;
    public Text arcelusLevelUpFore;
    public Image arcelusPortrait;
    public Image arcelusPortraitBack;
    public Image arcelusHP;
    public Image arcelusHPBack;
    public Image arcelusMP;
    public Image arcelusMPBack;
    public Image arcelusExp;
    public Image arcelusExpBack;
    private float arcelusCurrentExp;
    private float arcelusMaxExp;
    private float arcelusExpStep;
    private int arcelusExpGain;
    private bool arcelusAddinExp;
    private bool startFadingArcelusIn;

    public Text oberonLevelUpBack;
    public Text oberonLevelUpFore;
    public Image oberonPortrait;
    public Image oberonPortraitBack;
    public Image oberonHP;
    public Image oberonHPBack;
    public Image oberonMP;
    public Image oberonMPBack;
    public Image oberonExp;
    public Image oberonExpBack;
    private float oberonCurrentExp;
    private float oberonMaxExp;
    private float oberonExpStep;
    private int oberonExpGain;
    private bool oberonAddinExp;
    private bool startFadingOberonIn;


    void Start()
    {
        screenActive = false;
        fadingIn = false;
        btlManager = BattleManager.instance;
        expStep = 0.0f;

        fargasLevelUpBack.gameObject.SetActive(false);
        fargasLevelUpFore.gameObject.SetActive(false);
        freaLevelUpBack.gameObject.SetActive(false);
        freaLevelUpFore.gameObject.SetActive(false);
        arcelusLevelUpBack.gameObject.SetActive(false);
        arcelusLevelUpFore.gameObject.SetActive(false);
        oberonLevelUpBack.gameObject.SetActive(false);
        oberonLevelUpFore.gameObject.SetActive(false);

        //Everything starts faded out
        startFadingFargasIn = startFadingArcelusIn = startFadingFreaIn = startFadingOberonIn = false;

        panelItemsColor.r = panelItemsColor.g = panelItemsColor.b = 1.0f;
        panelItemsColor.a = 0.0f;

        fargasPortrait.color = fargasPortraitBack.color = fargasHP.color = fargasMP.color = fargasExp.color = fargasHPBack.color = fargasMPBack.color = fargasExpBack.color = 
        freaPortrait.color = freaPortraitBack.color = freaHP.color = freaMP.color = freaExp.color = freaHPBack.color = freaMPBack.color = freaExpBack.color =
        oberonPortrait.color = oberonPortraitBack.color = oberonHP.color = oberonMP.color = oberonExp.color = oberonHPBack.color = oberonMPBack.color = oberonExpBack.color =
        arcelusPortrait.color = arcelusPortraitBack.color = arcelusHP.color = arcelusMP.color = arcelusExp.color = arcelusHPBack.color = arcelusMPBack.color = arcelusExpBack.color =
        victoryTextBack.color = victoryTextFore.color = panelItemsColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (screenActive)
        {
            if(thisImage.fillAmount<1.0f && fadingIn)
            {
                thisImage.fillAmount += 0.05f;
                panelItemsColor.a += 0.08f;
                victoryTextBack.color = panelItemsColor;
                panelItemsColor.r =  0.7058824f;
                panelItemsColor.b =  0.08294977f;
                panelItemsColor.g =  0.07843137f;
                victoryTextFore.color = panelItemsColor;
                panelItemsColor.r = panelItemsColor.g = panelItemsColor.b = 1.0f;
            }
            else if(fadingIn)
            {
                panelItemsColor.r = panelItemsColor.g = panelItemsColor.b = 1.0f;
                panelItemsColor.a = 0.0f;
                fadingIn = false;
                GetNewStats();
            }
            //We still haven't done anybody's EXP yet, so we start with Fargas
            //Fargas
            if (startFadingFargasIn)
            {
                if(FadeInItems(ref fargasPortrait, ref fargasPortraitBack, ref fargasHP, ref fargasHPBack, ref fargasMP, ref fargasMPBack, ref fargasExp, ref fargasExpBack))
                {
                    Debug.Log("Hitttt");
                    fargasAddinExp = true;
                    startFadingFargasIn = false;
                }               
            }
            else if (fargasAddinExp)
            {
                IncreaseExp(ref fargasExp, ref fargasCurrentExp, ref fargasMaxExp, ref fargasExpGain,ref fargasExpStep, 0);
            }
            //Oberon
            else if (startFadingOberonIn)
            {
                if(FadeInItems(ref oberonPortrait, ref oberonPortraitBack, ref oberonHP, ref oberonHPBack, ref oberonMP, ref oberonMPBack, ref oberonExp, ref oberonExpBack))
                {
                    oberonAddinExp = true;
                    startFadingOberonIn = false;
                }
            }

            else if (oberonAddinExp)
            {
                IncreaseExp(ref oberonExp, ref oberonCurrentExp, ref oberonMaxExp, ref oberonExpGain, ref oberonExpStep, 1);
            }
            //Frea
            else if (startFadingFreaIn)
            {
                if (FadeInItems(ref freaPortrait, ref freaPortraitBack, ref freaHP, ref freaHPBack, ref freaMP, ref freaMPBack, ref freaExp, ref freaExpBack))
                {
                    freaAddinExp = true;
                    startFadingFreaIn = false;
                }
            }

            else if (freaAddinExp)
            {
                IncreaseExp(ref freaExp, ref freaCurrentExp, ref freaMaxExp, ref freaExpGain, ref freaExpStep, 2);
            }
            //Arcelus
            else if (startFadingArcelusIn)
            {
                if (FadeInItems(ref arcelusPortrait, ref arcelusPortraitBack, ref arcelusHP, ref arcelusHPBack, ref arcelusMP, ref arcelusMPBack, ref arcelusExp, ref arcelusExpBack))
                {
                    arcelusAddinExp = true;
                    startFadingArcelusIn = false;
                }
            }

            if (arcelusAddinExp && !fargasAddinExp && !oberonAddinExp && !freaAddinExp)
            {
                IncreaseExp(ref arcelusExp, ref arcelusCurrentExp, ref arcelusMaxExp, ref arcelusExpGain, ref arcelusExpStep, 3);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !fargasAddinExp && !freaAddinExp && !oberonAddinExp && !arcelusAddinExp)
            {
                fadePanelToWorldMap.TransitionBackToWorldMapFromBattle();
            }
        }
    }
    
    public void GetNewStats()
    {
        Debug.Log("CALLED GET NEW STATS");
        //Must check if all the players are still alive but for now they are
        //Update the players' stats

        fargasHP.fillAmount = btlManager.players[0].playerReference.hpImage.fillAmount;
        fargasMP.fillAmount = btlManager.players[0].playerReference.currentMP / btlManager.players[0].playerReference.maxMP;
        fargasCurrentExp = btlManager.players[0].exp;
        fargasMaxExp = btlManager.players[0].expNeededForNextLevel;
        fargasExp.fillAmount = btlManager.players[0].exp / fargasMaxExp;
        fargasExpStep = 1.0f / fargasMaxExp;
        fargasExpGain = btlManager.expGain;


        oberonHP.fillAmount = btlManager.players[1].playerReference.hpImage.fillAmount;
        oberonMP.fillAmount = btlManager.players[1].playerReference.currentMP / btlManager.players[1].playerReference.maxMP;
        oberonCurrentExp = btlManager.players[1].exp;
        oberonMaxExp = btlManager.players[1].expNeededForNextLevel;
        oberonExp.fillAmount = btlManager.players[1].exp / oberonMaxExp;
        oberonExpStep = 1.0f / oberonMaxExp;
        oberonExpGain = btlManager.expGain;


        freaHP.fillAmount = btlManager.players[2].playerReference.hpImage.fillAmount;
        freaMP.fillAmount = btlManager.players[2].playerReference.currentMP / btlManager.players[2].playerReference.maxMP;
        freaCurrentExp = btlManager.players[2].exp;
        freaMaxExp = btlManager.players[2].expNeededForNextLevel;
        freaExp.fillAmount = btlManager.players[2].exp / freaMaxExp;
        freaExpStep = 1.0f / freaMaxExp;
        freaExpGain = btlManager.expGain;


        arcelusHP.fillAmount = btlManager.players[3].playerReference.hpImage.fillAmount;
        arcelusMP.fillAmount = btlManager.players[3].playerReference.currentMP / btlManager.players[3].playerReference.maxMP;
        arcelusCurrentExp = btlManager.players[3].exp;
        arcelusMaxExp = btlManager.players[3].expNeededForNextLevel;
        arcelusExp.fillAmount = btlManager.players[3].exp / arcelusMaxExp;
        arcelusExpStep = 1.0f / arcelusMaxExp;
        arcelusExpGain = btlManager.expGain;

        startFadingFargasIn = true;
    }

    private bool FadeInItems(ref Image portrait, ref Image portraitBack, ref Image hp, ref Image hpBack, ref Image mp, ref Image mpBack, ref Image exp, ref Image expBack )
    {
        panelItemsColor.a += 0.05f;
        portrait.color = portraitBack.color = hp.color = hpBack.color = mp.color = mpBack.color = exp.color = expBack.color = panelItemsColor;

        if (panelItemsColor.a >= 1.0f)
        {
            panelItemsColor.a = 0.0f;
            return true;
        }
        else
        {
            return false;
        }
    }


    private void IncreaseExp(ref Image expBar, ref float currentExp, ref float maxExp, ref int expGain, ref float expStep , int playerIndex)
    {
        if (expBar.fillAmount < 1.0f && currentExp < maxExp && expGain > 0)
        {
            currentExp++;
            expGain--;
            expBar.fillAmount += expStep;
        }
        //If the player has leveled up, get the new max exp and 
        else if (currentExp >= maxExp || expBar.fillAmount >= 1.0f)
        {
            btlManager.LevelUp(playerIndex);
            maxExp = btlManager.players[playerIndex].expNeededForNextLevel;
            expStep = 1.0f / maxExp;
            expBar.fillAmount = 0.0f;
            switch (playerIndex)
            {
                case 0:
                    fargasLevelUpBack.gameObject.SetActive(true);
                    fargasLevelUpFore.gameObject.SetActive(true);
                    break;
                case 1:
                    oberonLevelUpBack.gameObject.SetActive(true);
                    oberonLevelUpFore.gameObject.SetActive(true);
                    break;
                case 2:
                    freaLevelUpBack.gameObject.SetActive(true);
                    freaLevelUpFore.gameObject.SetActive(true);
                    break;
                case 3:
                    arcelusLevelUpBack.gameObject.SetActive(true);
                    arcelusLevelUpFore.gameObject.SetActive(true);
                    break;
            }

        }
        //If we have reached the exp gain, stop
        else if (expGain <= 0)
        {
            switch (playerIndex)
            {
                case 0:
                    fargasAddinExp = false;
                    startFadingOberonIn = true;
                    break;
                case 1:
                    oberonAddinExp = false;
                    startFadingFreaIn = true;
                    break;
                case 2:
                    freaAddinExp = false;
                    startFadingArcelusIn = true;
                    break;
                case 3:
                    arcelusAddinExp = false;
                    break;
            }
        }
    }

    public void VictoryFadeIn()
    {
        screenActive = true;
        fadingIn = true;
    }


}
