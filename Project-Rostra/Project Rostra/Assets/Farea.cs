using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farea : Enemy
{

    private int attackChance = 0; //Used to determine whether a skil or a normal attack will be used.
    private bool isThereADeadPlayer = false; //One of the Farea's skills relies on one of the players being dead
    private int bossPhase = 1; //Used to know which skills are available for the boss to use at which phase
    private float totalDamageSustained = 0.0f; //Used for Mother's Pain Skill. Becomes zero after the skill is used
    private float totalDamageThreshold = 300.0f; //When the Farea has sustained 300 or more damage and is in phase 2, it will do Mother's Pain Next


    private enum fareaSkills //Only the skills that will require waiting for a number of turns before executing
    {
        none,
        wails,
        lullabyOfDepsair,
        mothersPain,
        youAreNotMine

    }
    private fareaSkills chosenSkill = fareaSkills.none;

    //J&W --> (The official competitor of A&W)
    public GameObject jObj;
    public GameObject wObj;

    //Wail
    public GameObject wailWait;
    private string[] statToDebuff; //Used to store all possible stats that WoF can debuff
    private string chosenStat; //Used to store the choice of the stat to debuff

    //Lullaby
    public GameObject lullWait;


    protected override void Start()
    {
        battleManager = BattleManager.instance;
        objPooler = ObjectPooler.instance;
        uiBTL = UIBTL.instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
        animator = gameObject.GetComponent<Animator>();

        damageText.gameObject.SetActive(false);
        healText.gameObject.SetActive(false);

        haveAddedMyself = false;
        hit = false;
        dead = false;
        
        currentState = EnemyState.idle;
        waitTurnsText.gameObject.SetActive(false);
        jObj.gameObject.SetActive(false);
        wObj.gameObject.SetActive(false);
        wailWait.gameObject.SetActive(false);
        lullWait.gameObject.SetActive(false);

        statToDebuff = new string[2];
        statToDebuff[0] = "Defense";
        statToDebuff[1] = "Attack";
    }


    protected override void Update()
    {
        base.Update();
    }

    public override void EnemyTurn()
    {
        //Check if we're waiting on a skill first
        if (currentState == EnemyState.waiting)
        {
            waitQTurns--;
            waitTurnsText.text = waitQTurns.ToString(); //Update the UI
            if (waitQTurns <= 0)
            {
                waitTurnsText.gameObject.SetActive(false); //Turn off the text. Don't forget to enable it when the enemy goes to waiting state
                currentState = EnemyState.idle; //Change the state
                animator.SetInteger("WaitingIndex", 1); //After waiting, the index is always going to be 1
       }
            else
            {
                //End the turn
                uiBTL.EndTurn();
            }

        }
        else
        {
                //Only update the attackChance when no skill is on the waiting list
                //attackChance = Random.Range(0, 100);
                attackChance = 80; //Testing

            if (bossPhase == 1)
            {
                if (attackChance >= 0 && attackChance < 10) //Normal attack
                {
                    DumbAttack();
                }
                else if (attackChance >= 10 && attackChance < 40) //Wails of Frailty
                {
                    GoToWaitState(fareaSkills.wails, 2, 2);
                    //Summon skill effect
                    wailWait.gameObject.SetActive(true);
                    uiBTL.UpdateActivityText("Wails of Frailty");
                }
                else if(attackChance >= 40 && attackChance < 70) //Judgment and Wrath
                {
                    attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;
                    animator.SetBool("JudgementAndWrath", true);
                    uiBTL.UpdateActivityText("Judgement & Wrath");
                }
                else if (attackChance >= 70 && attackChance <= 100) // Lullaby Of Despair
                {
                    GoToWaitState(fareaSkills.lullabyOfDepsair, 1, 2);
                    //Summon skill effect
                    lullWait.gameObject.SetActive(true);
                    uiBTL.UpdateActivityText("Lullaby Of Despair");
                }
                    
            }
            else if(bossPhase == 2)
            {
                //Check if one of the players is dead --> if yes, there's a chance to use You Are Not Mine
                for (int i = 0; i < uiBTL.playersDead.Length; i++)
                {
                    if (uiBTL.playersDead[i] == true)
                    {
                        isThereADeadPlayer = true;
                        break;
                    }
                }
            }
        }
    }


    //-----------------------------Skills------------------------------//
    private void GoToWaitState(fareaSkills skill, int turnsToWait, int waitingIndex)
    {
        chosenSkill = skill;
        waitQTurns = turnsToWait;
        waitTurnsText.gameObject.SetActive(true);
        waitTurnsText.text = waitQTurns.ToString();
        animator.SetInteger("WaitingIndex", waitingIndex);
        currentState = EnemyState.waiting;
        uiBTL.EndTurn();
    }


    //Judgement & Wrath
    private void Judgement()
    {
        //Disable J and apply the damage.
        //Should change this to use animations
        objPooler.SpawnFromPool("JudgeAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
        jObj.gameObject.SetActive(false);
        attackThisPlayer.TakeDamage(eAttack);
    }

    private void Wrath()
    {
        //Disable W and apply the damage.
        //Should change this to use animations
        objPooler.SpawnFromPool("WrathAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
        wObj.gameObject.SetActive(false);
        attackThisPlayer.TakeDamage(eAttack * 1.2f); //Wrath does a little more damage
        animator.SetBool("JudgementAndWrath", false);
        uiBTL.DisableActivtyText();
        uiBTL.EndTurn();
    }

    private void JudgementAndWrathEffect()
    {
        jObj.gameObject.SetActive(true);
        wObj.gameObject.SetActive(true);
    }

    //Called from the animator to check which skill to execute after waiting

    private void SkillEffect()
    {
        switch (chosenSkill)
        {
            case fareaSkills.wails:
                //Choose at random whether you want to damage attack or defense
                chosenStat = statToDebuff[Random.Range(0, statToDebuff.Length)];
                //Update the UI
                uiBTL.UpdateActivityText("Wails of Frailty");
                //Damage all players
                for (int i = 0; i < battleManager.players.Length; i++)
                {
                    if(!battleManager.players[i].playerReference.dead)
                    {
                        battleManager.players[i].playerReference.TakeDamage(eAttack, 1, chosenStat, Player.playerAilments.none, null, 0.3f, 3, true);
                        //Summon debuff object
                        objPooler.SpawnFromPool("WailAttack", battleManager.players[i].playerReference.transform.position, gameObject.transform.rotation);
                    }
                }
                //Reset animator
                chosenSkill = fareaSkills.none;
                animator.SetInteger("WaitingIndex", 0);
                wailWait.gameObject.SetActive(false);
                uiBTL.EndTurn();
                break;
            case fareaSkills.lullabyOfDepsair:
                //Update the UI
                uiBTL.UpdateActivityText("Lullaby Of Despair");
                //Choose a player at random
                attackThisPlayer = battleManager.players[Random.Range(0, battleManager.players.Length)].playerReference;

                //If the player has no ailments, then affect thme with fear
                if(attackThisPlayer.currentAilment == Player.playerAilments.none) //Make sure you target 
                {
                    attackThisPlayer.TakeDamage(0.0f, 0, "", Player.playerAilments.fear, null, 0, 3, false);
                }
                else
                {
                    //Otherwise just damage the player
                    attackThisPlayer.TakeDamage(eAttack * 1.5f);
                }
                //Reset the animator
                chosenSkill = fareaSkills.none;
                animator.SetInteger("WaitingIndex", 0);
                lullWait.gameObject.SetActive(false);
                uiBTL.EndTurn();
                break;
            case fareaSkills.mothersPain:
                break;
            case fareaSkills.youAreNotMine:
                break;
        }

    }
}
