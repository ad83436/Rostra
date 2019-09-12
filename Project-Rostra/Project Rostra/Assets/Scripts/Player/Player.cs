using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float atk;
    public float def;
    public float currentHP;
    public float maxHP;
    public bool canRage; //Turned true once the current rage reaches the max rage. Used by UIBTL
    public float mp;
    public float agi;
    public float str;
    public float crit;
    public int playerIndex;
    public string name;
    public string[] equippedSkills = new string [4];
    public int range; //Range of player standard attack
    public int initialPos; //Position of the player 0 being Frontline and -1 being Ranged
    public Sprite qImage;
    private BattleManager battleManager;
    private UIBTL uiBTL;
    private Animator playerAnimator;
    private Enemy attackingThisEnemy;

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


    //Skills and Q
    private int QCounter; //Used to count turns since the player went in range or decided to go in range.

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
        battleManager = BattleManager.instance;
        uiBTL = UIBTL.instance;
        currentState = playerState.Idle;
        playerAnimator = gameObject.GetComponent<Animator>();

        currentRage = 0.0f;
        maxRage = 100.0f;
        canRage = false;
        rageModeIndicator.gameObject.SetActive(false);

        actualATK = atk;
        actualDEF = def;
        actualAgi = agi;
        actualCRIT = crit;
        actualSTR = str;
        healable = false;


        //Temp code until we have stats in the stat file
        battleManager.players[playerIndex].playerIndex = playerIndex;
        battleManager.players[playerIndex].agi = agi;
        battleManager.players[playerIndex].playerReference = this;
        battleManager.players[playerIndex].atk = actualATK;
        battleManager.players[playerIndex].def = actualDEF;
        battleManager.players[playerIndex].crit = actualCRIT;
        battleManager.players[playerIndex].str = actualSTR;
        battleManager.numberOfPlayers--;

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
            TakeDamage(20.0f, 10.0f);
        }
    }

    private void StartBattle()
    {
        currentHP = battleManager.players[playerIndex].hp;
        mp = battleManager.players[playerIndex].mp;
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
        battleManager.EndOfBattle(playerIndex, currentHP, mp);
    }

    //Called from the UIBTL to turn on the animation
    public void MyTurn()
    {
        playerAnimator.SetBool("Turn", true);
    }

    //Called from the UI
    public void Attack(Enemy enemyRef)
    {
        Debug.Log("Attack");
        playerAnimator.SetBool("Turn", false);
        playerAnimator.SetBool("Attack", true);
        attackingThisEnemy = enemyRef;
    }
    //Called from the animator
    private void CompleteAttack()
    {
        Debug.Log("CompleteAttack");
        playerAnimator.SetBool("Attack", false);
        attackingThisEnemy.TakeDamage(actualATK);
        uiBTL.ResetVisibilityForAllEnemies();
        uiBTL.EndTurn();
        battleManager.NextOnQueue(); //Move to the next on Q

        //If the player is in rage state, they can only attack so it makes sense to check if we were in rage mode when attacking
        if(currentState==playerState.Rage)
        {
            QCounter++;
            if(QCounter>=2) //If it's been 3 or more turns since the player raged out, reset rage mode
            {
                ResetPlayerRage();
            }
        }

    }

    public void Guard()
    {
        Debug.Log(name + " is Guarding");
    }

    public void TakeDamage(float damage, float enemyATK)
    {
        Debug.Log("Incoming Damage " + damage + " and enemyATK " + enemyATK);
        damage = enemyATK - ((def / (20.0f + def)) * enemyATK);
        Debug.Log("Modified Damage " + damage);
        currentHP -= damage;
        Debug.Log("Remaining HP " + currentHP);
        hpImage.fillAmount = currentHP / maxHP;
        if (currentRage < maxRage && currentState!=playerState.Rage) //If there's still capacity for rage while we're not actually in rage, increase the rage meter
        {
            currentRage += damage * 1.2f; //Rage amount is always 20% more than the health lost
            Debug.Log("Current Rage " + currentRage);
            rageImage.fillAmount = currentRage / maxRage;
            
            if(currentRage>=maxRage)
            {
                canRage = true; //Can now go into rage mode
            }
        }
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
        actualATK = atk;
        actualDEF = def;
        healable = true;
        QCounter = 0;
        rageModeIndicator.gameObject.SetActive(false);
        currentState = playerState.Idle;
    }
}
