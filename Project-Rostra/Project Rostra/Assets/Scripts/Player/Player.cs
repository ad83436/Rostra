using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int atk;
    public int def;
    public int hp;
    public int mp;
    public int agi;
    public int str;
    public int crit;
    public int playerIndex;
    public string name;
    public string[] equippedSkills = new string [4];
    private BattleManager battleManager;
    private UIBTL uiBTL;
    private Animator playerAnimator;
    private Enemy attackingThisEnemy;

    public enum playerState
    {
        Idle, //Player has not issued a command
        Guard, //When a player issues a guard command, lasts until the next turn of this character
        Waiting, //When a player issues a skill that takes more than 1 turn to execute
        Dead //When a character's HP reaches zero, that character's turn is skipped
    }

    public playerState currentState;

    private void Start()
    {
        battleManager = BattleManager.instance;
        uiBTL = UIBTL.instance;
        currentState = playerState.Idle;
        playerAnimator = gameObject.GetComponent<Animator>();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            battleManager.players[playerIndex].playerIndex = playerIndex;
            battleManager.players[playerIndex].agi = agi;
            battleManager.players[playerIndex].playerReference = this;
            battleManager.players[playerIndex].name = name;
            Debug.Log("Added player to BTL manager");
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            StartBattle();
        }
    }

    private void StartBattle()
    {
        hp = battleManager.players[playerIndex].hp;
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
        battleManager.EndOfBattle(playerIndex, hp, mp);
    }

    //Called from the UI
    public void Attack(Enemy enemyRef)
    {
        Debug.Log("Attack");
        playerAnimator.SetBool("Attack", true);
        attackingThisEnemy = enemyRef;
    }
    //Called from the animator
    private void CompleteAttack()
    {
        Debug.Log("CompleteAttack");
        playerAnimator.SetBool("Attack", false);
        attackingThisEnemy.TakeDamage(20.0f);
        uiBTL.ResetVisibilityForAllEnemies();
        uiBTL.EndTurn();
        battleManager.NextOnQueue(); //Move to the next on Q

    }

    public void mynameis()
    {
        Debug.Log("Player " + name);
    }

    public void Guard()
    {
        Debug.Log(name + " is Guarding");
    }
}
