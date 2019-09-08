using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBTL : MonoBehaviour
{
    private BattleManager btlManager;
    public GameObject playerTurnIndicator;
    public GameObject enemyToAttackIndicator;
    public GameObject playerIndicatorPos0;
    public GameObject playerIndicatorPos1;
    public GameObject playerIndicatorPos2;
    public GameObject playerIndicatorPos3;
    public GameObject enemyIndicatorPos0;
    public GameObject enemyIndicatorPos1;
    public GameObject enemyIndicatorPos2;
    public GameObject enemyIndicatorPos3;
    public GameObject enemyIndicatorPos4;
    public GameObject controlsPanel; //Needs to be disabled after choosing a command and re-enabled when it's a player's turn
    public GameObject highlighter;
    public GameObject highlighterPos0;
    public GameObject highlighterPos1;
    public GameObject highlighterPos2;
    public GameObject highlighterPos3;
    public GameObject highlighterPos4; //Used to go back to the basic menu when inside the skills/items menu
    public Text playerName;
    private int controlsIndicator; //Used to know which command has been chosen
    private int enemyIndicatorIndex;//Used to know which enemy is being chosen to be attacked
    private int previousEnemyIndicatorIndex; //Used to limit calls to become less visible
    [HideInInspector]
    public int currentPlayerTurnIndex; //Updated from the btl manager to know which player turn it is

    private enum btlUIState
    {
        choosingBasicCommand, //Player still choosing which command to use
        choosingSkillsCommand, //Player chooses between skills
        choosingItemsCommand, //Player chooses items
        choosingEnemy, //Player has chosen an offense command
        choosingPlayer //Player has chosen a supporting command
    }

    private btlUIState currentState;

    public Enemy[] enemies;

    private Player playerInControl;

    #region singleton
    public static UIBTL instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        btlManager = BattleManager.instance;
        controlsIndicator = 0; //Start at Attack
        highlighter.transform.position = highlighterPos0.transform.position;
        currentState = btlUIState.choosingBasicCommand;
        playerName.text = "";
        enemyToAttackIndicator.gameObject.SetActive(false);
        playerTurnIndicator.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        enemies = new Enemy[5];
        enemyIndicatorIndex = 0;
        previousEnemyIndicatorIndex = 0;
    }


    void Update()
    {
        switch(currentState)
        {
            case btlUIState.choosingBasicCommand:
                choosingBasicCommand();
                break;
            case btlUIState.choosingSkillsCommand:
                choosingSkillsCommand();
                break;
            case btlUIState.choosingItemsCommand:
                choosingItemsCommand();
                break;
            case btlUIState.choosingEnemy:
                choosingEnemy();
                break;
            case btlUIState.choosingPlayer:
                break;
        }
    }

    //Called from the BTL Manager to update the UI based on which player's turn it is
    public void showThisPlayerUI(int playerIndex, string name, Player playerReference)
    {
        Debug.Log("Show this player UI " + name);
        playerTurnIndicator.SetActive(true);
        controlsPanel.gameObject.SetActive(true);

        playerName.text = name;
        playerInControl = playerReference;

        switch (playerIndex)
        {
            case 0:
                playerTurnIndicator.transform.position = playerIndicatorPos0.transform.position;
                break;
            case 1:
                playerTurnIndicator.transform.position = playerIndicatorPos1.transform.position;
                break;
            case 2:
                playerTurnIndicator.transform.position = playerIndicatorPos2.transform.position;
                break;
            case 3:
                playerTurnIndicator.transform.position = playerIndicatorPos3.transform.position;
                break;
        }
    }

    private void choosingBasicCommand()
    {
        //Debug.Log("Choosing Basic Commands");
        switch(controlsIndicator)
        {
            case 0://Highlighter is at attack
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    controlsIndicator = 1;
                    highlighter.transform.position = highlighterPos1.transform.position;
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    controlsIndicator = 2;
                    highlighter.transform.position = highlighterPos2.transform.position;
                }
                else if(Input.GetKeyDown(KeyCode.Space)) //Player has chosen attack
                {
                    currentState = btlUIState.choosingEnemy;
                    enemyToAttackIndicator.SetActive(true);
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos0.transform.position;
                    enemyIndicatorIndex = 0;
                    makeChosenEnemyMorePrompt(enemyIndicatorIndex);
                    Debug.Log("Go to choosing enemy");
                }
                break;

            case 1://Highlighter is at Guard
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    controlsIndicator = 0;
                    highlighter.transform.position = highlighterPos0.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    controlsIndicator = 3;
                    highlighter.transform.position = highlighterPos3.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Guard
                {
                    playerInControl.Guard();
                    EndTurn();
                }
                break;

            case 2:
                //Highlighter is at Skills
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    controlsIndicator = 3;
                    highlighter.transform.position = highlighterPos3.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    controlsIndicator = 0;
                    highlighter.transform.position = highlighterPos0.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Skills
                {
                    currentState = btlUIState.choosingSkillsCommand;
                }
                break;
            case 3:
                //Highlighter is at Items
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    controlsIndicator = 2;
                    highlighter.transform.position = highlighterPos2.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    controlsIndicator = 1;
                    highlighter.transform.position = highlighterPos1.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Skills
                {
                    currentState = btlUIState.choosingItemsCommand;
                }
                break;
        }
    }

    private void choosingSkillsCommand()
    {
        switch (controlsIndicator)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    private void choosingItemsCommand()
    {

    }

    private void choosingEnemy()
    {
        //If we're at the same enemy, don't call the visible function again
        if (previousEnemyIndicatorIndex != enemyIndicatorIndex)
        {
            makeChosenEnemyMorePrompt(enemyIndicatorIndex);
        }

        switch (enemyIndicatorIndex)
        {
            case 0:
                if(Input.GetKeyDown(KeyCode.DownArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 1;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos1.transform.position;
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 3;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos3.transform.position;
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 2;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos2.transform.position;
                }
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 2;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos2.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 4;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos4.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 0;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos0.transform.position;
                }
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 0;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos0.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 3;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos3.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 1;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos1.transform.position;
                }
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 4;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos4.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 0;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos0.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 4;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos4.transform.position;
                }
                break;
            case 4:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 3;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos3.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 2;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos2.transform.position;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    previousEnemyIndicatorIndex = enemyIndicatorIndex;
                    enemyIndicatorIndex = 3;
                    enemyToAttackIndicator.transform.position = enemyIndicatorPos3.transform.position;
                }
                break;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerInControl.Attack(enemies[enemyIndicatorIndex]);
        }
    }

    public void makeChosenEnemyMorePrompt(int enemyIndex) //When the player chooses an enemy to attack, the other enemies should be less visible
    {
        enemies[enemyIndex].resetVisibility();

        for(int i =0;i<5;i++)
        {
            if(i!=enemyIndex)
            {
                enemies[i].becomeLessVisbile();
            }
        }
    }

    private void choosingPlayer()
    {

    }

    private void EndTurn()
    {
        playerTurnIndicator.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
    }


}
