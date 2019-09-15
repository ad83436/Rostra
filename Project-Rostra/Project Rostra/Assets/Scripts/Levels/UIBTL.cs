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
    public GameObject rageModeIndicator1;
    public GameObject rageModeIndicator2;
    public GameObject highlighter;
    public GameObject highlighterPos0;
    public GameObject highlighterPos1;
    public GameObject highlighterPos2;
    public GameObject highlighterPos3;
    public GameObject highlighterPos4; //Used to go back to the basic menu when inside the skills/items menu
    private int controlsIndicator; //Used to know which command has been chosen
    private int enemyIndicatorIndex;//Used to know which enemy is being chosen to be attacked
    private int activeRange; // Are we using the player's standard range of a skill's range?
    private int previousEnemyIndicatorIndex; //Used to limit calls to become less visible
    [HideInInspector]
    public int currentPlayerTurnIndex; //Updated from the btl manager to know which player turn it is

    //Control Panel
    public Text rageText; //Needs to be disabled when it's not usable
    public Text playerName;
    public Text attackText;
    public Text skillsText;
    public Text itemsText;
    public Text guardText;
    public Text currentHP;
    public Text maxHP;
    public Image hpBar;
    public Text currentMP;
    public Text maxMP;
    public Image mpBar;


    //Q UI Images

    private List<Sprite> imagesQ; //Filled by the BTL manager
    public Image image0;
    public Image image1;
    public Image image2;
    public Image image3;
    public Image image4;
    public Image image5;
    public Image image6;
    public Image image7;
    public Image image8;
    private Vector2 imageRecyclePos; //To which position do images go when recycled?
    private Vector2 targetPos; //Used to calculate the distance each image travels in the Q
    private float imageMovementSpeed;
    private float imageMaxDistance; //Distance to be moved by each image
    private bool moveImagesNow; //Toggled on end turn and when the first image hits the recycler

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
    public bool [] enemiesDead;
    public int numberOfEnemies;

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

        enemies = new Enemy[5]; //Filled by the BTL Manager in Add Enemy
        enemiesDead = new bool[5]; //Every entry is turned to true by the enemy that dies
        controlsIndicator = 0; //Start at Attack
        highlighter.transform.position = highlighterPos0.transform.position;
        currentState = btlUIState.choosingBasicCommand;
        playerName.text = "";
        enemyToAttackIndicator.gameObject.SetActive(false);
        playerTurnIndicator.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        enemyIndicatorIndex = 0;
        previousEnemyIndicatorIndex = 0;
        activeRange = 0;

        imagesQ = new List<Sprite>();
        imageRecyclePos = image8.gameObject.transform.localPosition;
        targetPos = image0.transform.localPosition;

        imageMovementSpeed = 250.0f;
        imageMaxDistance = 149.0f;
        moveImagesNow = false;

        //Rage mode is not available when the battle starts
        rageText.color = Color.gray;
        rageModeIndicator1.gameObject.SetActive(false);
        rageModeIndicator2.gameObject.SetActive(false);

        //All the enemies are alive at the beginning
        for(int i=0;i<enemiesDead.Length;i++)
        {
            enemiesDead[i] = false;
        }
    }


    void Update()
    {

        if(moveImagesNow)
        {
            //Called on End Turn
            moveQImages();
        }

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

    public void AddImageToQ(Sprite nextOnQImage)
    {
        //Called from the BTL manager when adding characters to the Q

        //Debug.Log("Adding image on Q");

        imagesQ.Add(nextOnQImage);
    }

    public void QueueIsReady()
    {
        //Called from the BTL manager when the Q has been built

        //Debug.Log("Queue is Ready!  " + imagesQ.Count);

        //Fill up the Q until its of size 9. Only 6 will be on screen at a time however.
        switch(imagesQ.Count)
        {
            //Minimum size of Q is 5 since we will not be removing the images when characters die
            case 5:
                imagesQ.Add(imagesQ[0]);
                imagesQ.Add(imagesQ[1]);
                imagesQ.Add(imagesQ[2]);
                imagesQ.Add(imagesQ[3]);
                break;
            case 6:
                imagesQ.Add(imagesQ[0]);
                imagesQ.Add(imagesQ[1]);
                imagesQ.Add(imagesQ[2]);
                break;
            case 7:
                imagesQ.Add(imagesQ[0]);
                imagesQ.Add(imagesQ[1]);
                break;
            case 8:
                imagesQ.Add(imagesQ[0]);
                break;
        }
        image0.sprite = imagesQ[0];
        image1.sprite = imagesQ[1];
        image2.sprite = imagesQ[2];
        image3.sprite = imagesQ[3];
        image4.sprite = imagesQ[4];
        image5.sprite = imagesQ[5];
        image6.sprite = imagesQ[6];
        image7.sprite = imagesQ[7];
        image8.sprite = imagesQ[8];
    }

    public void moveQImages()
    {
        //Move all the images an amount of imageMaxDistance to the right

        targetPos.x = image0.transform.localPosition.x + imageMaxDistance;
        image0.transform.localPosition = Vector2.MoveTowards(image0.transform.localPosition, targetPos, imageMovementSpeed * Time.deltaTime);

        targetPos.x = image1.transform.localPosition.x + imageMaxDistance;
        image1.transform.localPosition = Vector2.MoveTowards(image1.transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = image2.transform.localPosition.x + imageMaxDistance;
        image2.transform.localPosition = Vector2.MoveTowards(image2.transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = image3.transform.localPosition.x + imageMaxDistance;
        image3.transform.localPosition = Vector2.MoveTowards(image3.transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = image4.transform.localPosition.x + imageMaxDistance;
        image4.transform.localPosition = Vector2.MoveTowards(image4.transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = image5.transform.localPosition.x + imageMaxDistance;
        image5.transform.localPosition = Vector2.MoveTowards(image5.transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = image6.transform.localPosition.x + imageMaxDistance;
        image6.transform.localPosition = Vector2.MoveTowards(image6.transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = image7.transform.localPosition.x + imageMaxDistance;
        image7.transform.localPosition = Vector2.MoveTowards(image7.transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = image8.transform.localPosition.x + imageMaxDistance;
        image8.transform.localPosition = Vector2.MoveTowards(image8.transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

    }

    //Called when the image at the far right of the Q collides with the recycle image collider
    public void imageRecycle(int imageIndex)
    {
        //We've hit the recycler, stop moving!
        moveImagesNow = false;

        switch (imageIndex) //Which image hit the recycler?
        {
            case 0:
                image0.gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 1:
                image1.gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 2:
                image2.gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 3:
                image3.gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 4:
                image4.gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 5:
                image5.gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 6:
                image6.gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 7:
                image7.gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 8:
                image8.gameObject.transform.localPosition = imageRecyclePos;
                break;

        }
    }

    //Called from the BTL Manager to update the UI based on which player's turn it is
    public void showThisPlayerUI(int playerIndex, string name, Player playerReference)
    {
        if(playerReference.currentState != Player.playerState.Waiting)
        {
            currentState = btlUIState.choosingBasicCommand;
        }

        //Debug.Log("Player index " + playerIndex);

        playerTurnIndicator.SetActive(true);
        controlsPanel.gameObject.SetActive(true);

        playerName.text = name;
        playerInControl = playerReference;
        UpdatePlayerHPControlPanel();
        UpdatePlayerMPControlPanel();
        playerInControl.MyTurn();
        RageOptionTextColor();

       
        //Turn off the indicator if the player in question is not in rage mode
        if(playerInControl.currentState!=Player.playerState.Rage)
        {
            rageModeIndicator1.gameObject.SetActive(false);
            rageModeIndicator2.gameObject.SetActive(false);
            skillsText.color = Color.white;
            itemsText.color = Color.white;
            guardText.color = Color.white;
        }

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
        if (!moveImagesNow) //Don't allow the player to choose a command until the Q has settled down
        {
            switch (controlsIndicator)
            {
                //If the player is in range mode, only "Attack" can be chosen
                case 0://Highlighter is at attack
                    if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow)) && playerInControl.currentState!=Player.playerState.Rage)
                    {
                        controlsIndicator = 1;
                        highlighter.transform.position = highlighterPos1.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow) && playerInControl.currentState != Player.playerState.Rage)
                    {
                        controlsIndicator = 2;
                        highlighter.transform.position = highlighterPos2.transform.position;
                    }
                    else if(Input.GetKeyDown(KeyCode.LeftArrow) && playerInControl.canRage && playerInControl.currentState != Player.playerState.Rage)
                    {
                        controlsIndicator = 4;
                        highlighter.transform.position = highlighterPos4.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen attack
                    {
                        currentState = btlUIState.choosingEnemy;
                        enemyToAttackIndicator.SetActive(true);
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos0.transform.position;
                        enemyIndicatorIndex = 0;
                        activeRange = playerInControl.range;
                        MakeChosenEnemyMorePrompt(enemyIndicatorIndex);
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
                    }
                    break;

                case 2:
                    //Highlighter is at Skills
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        controlsIndicator = 3;
                        highlighter.transform.position = highlighterPos3.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        controlsIndicator = 0;
                        highlighter.transform.position = highlighterPos0.transform.position;
                    }
                    else if(Input.GetKeyDown(KeyCode.RightArrow) && playerInControl.canRage)
                    {
                        controlsIndicator = 4;
                        highlighter.transform.position = highlighterPos4.transform.position;
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
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        controlsIndicator = 1;
                        highlighter.transform.position = highlighterPos1.transform.position;
                    }
                    else if(Input.GetKeyDown(KeyCode.RightArrow) && playerInControl.canRage)
                    {
                        controlsIndicator = 4;
                        highlighter.transform.position = highlighterPos4.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Skills
                    {
                        currentState = btlUIState.choosingItemsCommand;
                    }
                    break;
                case 4://Hilighter is at RAGE
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        controlsIndicator = 3;
                        highlighter.transform.position = highlighterPos3.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        controlsIndicator = 2;
                        highlighter.transform.position = highlighterPos2.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        controlsIndicator = 0;
                        highlighter.transform.position = highlighterPos0.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Skills
                    {
                        rageText.color = Color.yellow;
                        skillsText.color = Color.gray;
                        itemsText.color = Color.gray;
                        guardText.color = Color.gray;
                        playerInControl.Rage(); //Go into rage mode
                        rageModeIndicator1.gameObject.SetActive(true);
                        rageModeIndicator2.gameObject.SetActive(true);
                        highlighter.transform.position = highlighterPos0.transform.position;
                        controlsIndicator = 0;
                    }
                    break;
            }
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
            MakeChosenEnemyMorePrompt(enemyIndicatorIndex);
        }

        switch (enemyIndicatorIndex)
        {
            case 0:
                if(Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[1] != null && enemiesDead[1]==false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 1;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos1.transform.position;
                    }
                }
                else if((Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.LeftArrow)) && (activeRange + playerInControl.initialPos >= 2))
                {
                    if (enemies[3] != null && enemiesDead[3] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 3;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos3.transform.position;
                    }
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[2] != null && enemiesDead[2] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 2;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos2.transform.position;
                    }
                }
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[2] != null && enemiesDead[2] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 2;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos2.transform.position;
                    }
                }
                else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) && (activeRange + playerInControl.initialPos >= 2))
                {
                    if (enemies[4] != null && enemiesDead[4] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 4;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos4.transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[0] != null && enemiesDead[0] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 0;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos0.transform.position;
                    }
                }
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[0] != null && enemiesDead[0] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 0;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos0.transform.position;
                    }
                }
                else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) && (activeRange + playerInControl.initialPos >= 2))
                {
                    if (enemies[3] != null && enemiesDead[3] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 3;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos3.transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[1] != null && enemiesDead[1] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 1;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos1.transform.position;
                    }
                }
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[4] != null && enemiesDead[4] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 4;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos4.transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (enemies[0] != null && enemiesDead[0] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 0;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos0.transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[4] != null  && enemiesDead[4] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 4;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos4.transform.position;
                    }
                }
                break;
            case 4:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[3] != null && enemiesDead[3] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 3;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos3.transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (enemies[2] != null && enemiesDead[2] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 2;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos2.transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[3] != null && enemiesDead[3] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 3;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPos3.transform.position;
                    }
                }
                break;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerInControl.Attack(enemies[enemyIndicatorIndex]);
        }
    }

    public void MakeChosenEnemyMorePrompt(int enemyIndex) //When the player chooses an enemy to attack, the other enemies should be less visible
    {
        if (enemies[enemyIndex] != null)
        {
            enemies[enemyIndex].resetVisibility();
        }

        for(int i =0;i<5;i++)
        {
            if(i!=enemyIndex && enemies[i]!=null)
            {
                enemies[i].becomeLessVisbile();
            }
        }
    }

    public void ResetVisibilityForAllEnemies()
    {
        //Reset the visibility of all enemies
        for (int i = 0; i < 5; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].resetVisibility();
            }
        }

    }

    private void choosingPlayer()
    {

    }

    public void RageOptionTextColor()
    {
        if (playerInControl.canRage)
        {
            rageText.color = Color.white;
        }
        else
        {
            rageText.color = Color.gray;
        }

    }

    public void UpdatePlayerHPControlPanel()
    {
        currentHP.text = Mathf.RoundToInt(playerInControl.currentHP).ToString();
        maxHP.text = " / " + Mathf.RoundToInt(playerInControl.maxHP).ToString();
        hpBar.fillAmount = playerInControl.hpImage.fillAmount;
    }

    public void UpdatePlayerMPControlPanel()
    {
        currentMP.text = Mathf.RoundToInt(playerInControl.currentMP).ToString();
        maxMP.text = " / " + Mathf.RoundToInt(playerInControl.maxMP).ToString();
        mpBar.fillAmount = playerInControl.currentMP / playerInControl.maxMP;
    }

    public void EndTurn()
    {
        playerTurnIndicator.SetActive(false);
        enemyToAttackIndicator.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        moveImagesNow = true;
    }

    public void EnemyIsDead(int enemyIndex)
    {
        enemiesDead[enemyIndex] = true;

        for(int i = 0, j=0; j<enemies.Length;j++)
        {
            if(enemies[j]!=null && enemiesDead[j]==true)
            {
                i++;
                if(i>=numberOfEnemies)
                {
                    Debug.Log("Victorrryyyy");
                }
            }
        }
    }


}
