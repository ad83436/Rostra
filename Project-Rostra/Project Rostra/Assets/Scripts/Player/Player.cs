﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Player stats
    public float atk;
    public float def;
    public float currentHP;
    public float maxHP;
    public bool canRage; //Turned true once the current rage reaches the max rage. Used by UIBTL
    public float currentMP;
    public float maxMP;
    public float agi;
    public float str;
    public float crit;
    public int playerIndex;
    public string name;
    public string[] equippedSkills = new string [4];
    public int range; //Range of player standard attack
    public int initialPos; //Position of the player 0 being Frontline and -1 being Ranged

    //Queue
    public Sprite qImage;
    private int QCounter; //Used to count turns since the player went in rage or decided to go in waiting state.

    //Instances
    private BattleManager battleManager;
    private UIBTL uiBTL;

    //Components
    private Animator playerAnimator;


    //Rage
    public float currentRage;
    public float maxRage;
    public GameObject rageModeIndicator;

    //UI
    public Image hpImage;
    public Image rageImage;

    //Actual stats --> Stats after they've been modified in battle
    private float actualATK;
    private float actualDEF;
    private float actualAgi;
    private float actualCRIT;
    private float actualSTR;
    public bool healable; //False when dead and in rage mode

    //Targeted enemy info
    private Enemy attackingThisEnemy;
    private bool hit; //Hit or miss  

    //Guarding
    private float actualDefBeforeGuard; //What if the player uses a def-increasing skill before making this character go to guard?
                                        //Should be updated correctly if a player is guarding and while doing so gets his/her def increased


    public enum playerState
    {
        Idle, //Player has not issued a command
        Guard, //When a player issues a guard command, lasts until the next turn of this character
        Waiting, //When a player issues a skill that takes more than 1 turn to execute
        Dead, //When a character's HP reaches zero, that character's turn is skipped
        Rage //When in rage mode, the player's attack is doubled, the defense halved, and the player becomes unhealable, and cannot use skills or guard.
    }

    public playerState currentState;

    private void Start()
    {
        //Instances
        battleManager = BattleManager.instance;
        uiBTL = UIBTL.instance;

        //States
        currentState = playerState.Idle;

        //Components
        playerAnimator = gameObject.GetComponent<Animator>();

        //Rage
        currentRage = 0.0f;
        maxRage = 100.0f;
        canRage = false;
        rageModeIndicator.gameObject.SetActive(false);

        //Actual stats
        actualATK = atk;
        actualDefBeforeGuard = actualDEF = def;
        actualAgi = agi;
        actualCRIT = crit;
        actualSTR = str;
        healable = false;

        //Targeted enemy info
        attackingThisEnemy = null;
        hit = false;

        //Temp code until we have stats in the stat file
        battleManager.players[playerIndex].playerIndex = playerIndex;
        battleManager.players[playerIndex].agi = agi;
        battleManager.players[playerIndex].playerReference = this;
        battleManager.players[playerIndex].atk = actualATK;
        battleManager.players[playerIndex].def = actualDEF;
        battleManager.players[playerIndex].crit = actualCRIT;
        battleManager.players[playerIndex].str = actualSTR;
        battleManager.players[playerIndex].name = name;
        battleManager.numberOfPlayers--;

        //UI
        hpImage.fillAmount = currentHP / maxHP;
        rageImage.fillAmount = currentRage / maxRage;


    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            //Temp code until we have stats in the stat file
         //   battleManager.players[playerIndex].playerIndex = playerIndex;
         //   battleManager.players[playerIndex].agi = agi;
         //   battleManager.players[playerIndex].playerReference = this;
         //   battleManager.players[playerIndex].atk = actualATK;
         //   battleManager.players[playerIndex].def = actualDEF;
         //   battleManager.players[playerIndex].crit = actualCRIT;
         //    battleManager.players[playerIndex].str = actualSTR;
            //Debug.Log("Added player to BTL manager");
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            StartBattle();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            //Testing the damage formula and rage calculations
            TakeDamage(30.0f);
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            Heal(0.2f);
        }
    }

    private void StartBattle()
    {
        currentHP = battleManager.players[playerIndex].currentHP;
        currentMP = battleManager.players[playerIndex].currentMP;
        atk = battleManager.players[playerIndex].atk;
        def = battleManager.players[playerIndex].def;
        //agi = battleManager.players[playerIndex].agi;
        str = battleManager.players[playerIndex].str;
        crit = battleManager.players[playerIndex].crit;
        equippedSkills[0] = battleManager.players[playerIndex].skills[0];
        equippedSkills[1] = battleManager.players[playerIndex].skills[1];
        equippedSkills[2] = battleManager.players[playerIndex].skills[2];
        equippedSkills[3] = battleManager.players[playerIndex].skills[3];
    }

    private void EndBattle()
    {
        battleManager.EndOfBattle(playerIndex, currentHP, currentMP);
    }

    //Called from the UIBTL to turn on the animation
    public void MyTurn()
    {
        playerAnimator.SetBool("Turn", true);

        //If the it's my turn again, and I have been guarding, end the guard since guarding only lasts for 1 turn
        if(currentState == playerState.Guard)
        {
            EndGuard();
        }
    }

    //Called from the UI
    public void Attack(Enemy enemyRef)
    {
        playerAnimator.SetBool("Turn", false);
        playerAnimator.SetBool("Attack", true);
        attackingThisEnemy = enemyRef;
    }
    //Called from the animator
    private void CompleteAttack()
    {
        playerAnimator.SetBool("Attack", false);
        //Check hit or miss
        CalculateHit();
        if (hit)
        {
            //Check for critical hits
            if (CalculateCrit() <= crit)
            {
                attackingThisEnemy.TakeDamage(actualATK * 1.2f);
            }
            else
            {
                attackingThisEnemy.TakeDamage(actualATK);
            }
        }
        uiBTL.ResetVisibilityForAllEnemies();
        uiBTL.EndTurn();

        //If the player is in rage state, they can only attack so it makes sense to check if we were in rage mode when attacking
        if(currentState==playerState.Rage)
        {
            QCounter++;
            if(QCounter>=3) //If it's been 3 or more turns since the player raged out, reset rage mode
            {
                ResetPlayerRage();
            }
        }

    }

    //Calculate whether the attack is a hit or a miss
    private void CalculateHit()
    {
        //20 sided die + str <? enemy agility
        if(Random.Range(0.0f,20.0f) + str < attackingThisEnemy.eAgility)
        {
            hit = false;
        }
        else
        {
            hit = true;
        }

        Debug.Log("Hit is " + hit);
    }

    private float CalculateCrit()
    {
        return Random.Range(0.0f, 100.0f);
    }

    //Guard and End Guard are called from the UI. End Guard is called when the player's turn returns
    public void Guard()
    {
        actualDEF = actualDefBeforeGuard * 1.5f;
        currentState = playerState.Guard;
        playerAnimator.SetBool("Turn", false);
        Debug.Log(name + " is Guarding and current def is " + actualDEF);
        uiBTL.EndTurn();
        battleManager.NextOnQueue();
    }
    public void EndGuard()
    {
        actualDEF = actualDefBeforeGuard;
        currentState = playerState.Idle;
    }

    public void TakeDamage(float enemyATK)
    {
        float damage = enemyATK - ((def / (20.0f + def)) * enemyATK);
        currentHP -= damage;
        battleManager.players[playerIndex].currentHP = currentHP; //Update the BTL manager with the new health
        hpImage.fillAmount = currentHP / maxHP;
        if (currentRage < maxRage && currentState!=playerState.Rage) //If there's still capacity for rage while we're not actually in rage, increase the rage meter
        {
            currentRage += damage * 1.2f; //Rage amount is always 20% more than the health lost
            rageImage.fillAmount = currentRage / maxRage;
            
            if(currentRage>=maxRage)
            {
                currentRage = maxRage;
                canRage = true; //Can now go into rage mode
            }
        }
    }

    //Heal function. Different heal skills will heal the player by different percentages
    public void Heal(float percentage)
    {
        float healAmount = percentage * maxHP;
        currentHP += healAmount;
        battleManager.players[playerIndex].currentHP = currentHP;

        if(currentHP>maxHP)
        {
            currentHP = maxHP;
        }
        //If the player could rage, now they could not since they healed
        if(canRage)
        {
            canRage = false;
            uiBTL.RageOptionTextColor();
        }

        currentRage -= healAmount * 1.2f; //Rage goes down by 20% more than the health gained

        if(currentRage < 0.0f)
        {
            currentRage = 0.0f;
        }

        //Update the UI
        hpImage.fillAmount = currentHP / maxHP;
        rageImage.fillAmount = currentRage / maxRage;
        uiBTL.UpdatePlayerHPControlPanel();
    }

    //Called by the UIBTl when the player chooses to go into rage mode
    public void Rage()
    {
        actualATK = atk * 2.0f;
        actualDEF = def / 2.0f;
        healable = false;
        QCounter = 0; //Reset the QCounter
        rageModeIndicator.gameObject.SetActive(true);
        currentState = playerState.Rage;
    }

    public void ResetPlayerRage()
    {
        Debug.Log("Rage has cooled down");
        currentRage = 0.0f;
        actualATK = atk;
        actualDEF = def;
        healable = true;
        QCounter = 0;
        rageModeIndicator.gameObject.SetActive(false);
        rageImage.fillAmount = 0.0f;  //Update the UI
        currentState = playerState.Idle;
    }
}
