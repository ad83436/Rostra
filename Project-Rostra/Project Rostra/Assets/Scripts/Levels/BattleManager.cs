using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //Player information is updated first from the CharacterStatus struct at start
    [System.Serializable]
    public class PlayerInformtion
    {
        public int playerIndex;
        public float hp;
        public float mp;
        public string[] skills = new string[4];
        public float atk;
        public float def;
        public float agi;
        public float str;
        public float crit;
        public string name;
        public int exp;
        public int expNeededForNextLevel;
        public Player playerReference;
        public Enemy enemyReference;
    }

    //At the beginning of each battle, each player and enemy will use the singleton to update their stats
    #region singleton

    public static BattleManager instance;

    private UIBTL uiBtl;
    private EnemySpawner enemySpawner;

    public PlayerInformtion[] players;
    public PlayerInformtion[] enemies;
    private List<PlayerInformtion> battleQueue;

    private List<float> pAgilities;
    private List<float> eAgilities;

    private float maxPlayerAgi = 0;
    private int maxPlayerIndex = 0;
    private int[] removedPlayerIndexes; //We need to keep track of which players and enemies have been accounted for in the queue
    private float maxEnemyAgi = 0;
    private int maxEnemyIndex = 0;
    private int[] removedEnemyIndexes;
    public int numberOfEnemies; // Updated from the world map. Need to make sure all enemies are added before building the Q
    private bool allEnemiesAdded = false; //Used to make sure that enemies and players are added before building the Q
    private bool allPlayersAdded = false;
    public int numberOfPlayers = 4; //Should be private. Public for testing purposes as its updated from the player's side

    public GameObject[] enemyPos = new GameObject[5];

    public bool addEnemies;


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

        players = new PlayerInformtion[4]; //max 4 players
        enemies = new PlayerInformtion[5];//max 5 enemies
        pAgilities = new List<float>();
        eAgilities = new List<float>();
        battleQueue = new List<PlayerInformtion>();
        removedPlayerIndexes = new int[4];
        removedEnemyIndexes = new int[5];

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new PlayerInformtion();
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = new PlayerInformtion();
        }

        for (int i = 0; i < removedPlayerIndexes.Length; i++)
        {
            removedPlayerIndexes[i] = -1;
        }
        for (int i = 0; i < removedEnemyIndexes.Length; i++)
        {
            removedEnemyIndexes[i] = -1;
        }
        addEnemies = false;
    }

    #endregion

    private void Start()
    {
        uiBtl = UIBTL.instance;
        enemySpawner = EnemySpawner.instance;
        for(int i =0;i<5;i++)
        {
            enemySpawner.AddPos(enemyPos[i], i);
        }
        //Each player and enemy would have an index that stores their information
    }


    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.S))
        {
           StartBattle();
           Debug.Log("Start battle");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            BuildQueue();
            Debug.Log("Build queue");
            Debug.Log("Q size: " + battleQueue.Count);
        }
        */

        //Temp code

        if(numberOfPlayers<=0 && allPlayersAdded == false)
        {
            allPlayersAdded = true;
            addEnemies = true;
            for (int i = 0; i < 5; i++)
            {
                enemySpawner.AddPos(enemyPos[i],i);
            }
            enemySpawner.SpawnEnemies();

        }

        //Only start the battle when all players and enemies have been added
        if(allPlayersAdded && allEnemiesAdded)
        {
            Debug.Log("Start Battle");
            StartBattle();
            allPlayersAdded = false;
            allEnemiesAdded = false;
            numberOfPlayers = 4; //Ready for the next battle
            addEnemies = false;
        }

    }

    //Called at the beginning of the battle to store references to current enemies. Needed to be able to update the queue
    public void AddEnemy(int enemyIndex, int agi, int str, int crit, Enemy enemyRef, string name)
    {
            enemies[enemyIndex].playerIndex = enemyIndex;
            enemies[enemyIndex].agi = agi;
            enemies[enemyIndex].str = str;
            enemies[enemyIndex].crit = crit;
            enemies[enemyIndex].enemyReference = enemyRef;
            enemies[enemyIndex].name = name;


        numberOfEnemies--; //Update the number of enemies after adding an enemy. This number is obtained from the WM
        Debug.Log("Number ofenemies " + numberOfEnemies);

        if (numberOfEnemies<=0)
        {
            allEnemiesAdded = true;
        }

        //This will probably need to change to avoid race conditions between startbattle and build Q
        Debug.Log("Indexxxxx  "+ enemyIndex);
        uiBtl.enemies[enemyIndex] = enemyRef; //Update the UI system with the enemy
    }

    public void StartBattle()
    {
        //Store and sort the agilities of the players and enemies in ascending order

        foreach(PlayerInformtion p in players)
        {
            if (p.playerReference != null)//Make sure all the entries have players (i.e. what if we have less than 4 players)
            {
                pAgilities.Add(p.agi);
            }
        }
            

        pAgilities.Sort();


        foreach (PlayerInformtion e in enemies)
        {
            if (e.enemyReference != null) //Make sure all the entries have enemies (i.e. what if we have less than 5 enemies)
            {
                eAgilities.Add(e.agi);
            }
        }

        eAgilities.Sort();


        BuildQueue();
        NextOnQueue();
    }

    public void NextOnQueue()
    {
        //Temp code

        //Should check if this is a player, but for now, this is a player
        uiBtl.showThisPlayerUI(battleQueue[0].playerIndex, battleQueue[0].name, battleQueue[0].playerReference);
        //Add it to the end of the Q
        battleQueue.Add(battleQueue[0]);
        //Remove it from the start of the Q 
        battleQueue.RemoveAt(0);
    }

    public void BuildQueue()
    {
        //Compare the player with the highest agility to the enemy with the highest agility
        if (pAgilities.Count > 0)
        {
            //Since the list is sorted, the last element has the highest agility
            maxPlayerAgi = pAgilities[pAgilities.Count - 1];

            foreach (PlayerInformtion e in players)
            {
                //Is this the player that has the highest agility?
                if (maxPlayerAgi == e.agi)
                {
                    //What if two players have the same agility? Make sure you are checking a different player each time
                    if (e.playerIndex == removedPlayerIndexes[0] || e.playerIndex == removedPlayerIndexes[1]
                        || e.playerIndex == removedPlayerIndexes[2] || e.playerIndex == removedPlayerIndexes[3])
                    {
                        //If it is a player we've already added to the queue, move on...
                        continue;
                        
                    }
                    else
                    {
                        //Otherwise, this player is potentially the next on Q (still need to compare with the enemies)
                        maxPlayerIndex = e.playerIndex;                        
                    }
                    
                }
            }
        }

        if (eAgilities.Count > 0)
        {
            //Enemy list is sorted. Last element has the highest agility
            maxEnemyAgi = eAgilities[eAgilities.Count - 1];

            foreach (PlayerInformtion e in enemies)
            {
                //Is this the enemy with the highest agility?
                if (maxEnemyAgi == e.agi)
                {
                    //What if two enemies have the same agility? Make sure we don't add the same enemy to the Q twice
                    if (e.playerIndex == removedEnemyIndexes[0] || e.playerIndex == removedEnemyIndexes[1] 
                        || e.playerIndex == removedEnemyIndexes[2] || e.playerIndex == removedEnemyIndexes[3] 
                        || e.playerIndex == removedEnemyIndexes[4])
                    {
                        //This enemy has already been added, move on
                        continue;
                    }
                    else
                    {
                        //New enemy to be added to the Q
                        maxEnemyIndex = e.playerIndex;
                    }
                   
                }
                
            }
        }

        //If both player agility and enemy agility lists are not empty, see which of them has the character with the highest agility
        if (pAgilities.Count > 0 && eAgilities.Count > 0)
        {
            if (maxPlayerAgi >= maxEnemyAgi)
            {
               //The player has a higher agility
               battleQueue.Add(players[maxPlayerIndex]);
               //Add the player's image to the UI
               uiBtl.AddImageToQ(players[maxPlayerIndex].playerReference.qImage);
               //Remove the player's agility from the list
               pAgilities.RemoveAt(pAgilities.Count - 1);
               //Add the player's index to the array of removed players
               removedPlayerIndexes[maxPlayerIndex] = maxPlayerIndex;

            }
            else
            {
                //The enemy has the higher agility
                battleQueue.Add(enemies[maxEnemyIndex]);
                //Add the enemy's image to the UI
                uiBtl.AddImageToQ(enemies[maxEnemyIndex].enemyReference.qImage);
                //Remove the enemy's agility from the list
                eAgilities.RemoveAt(eAgilities.Count - 1);
                //Add the enemy's index to the array of removed enemy
                removedEnemyIndexes[maxEnemyIndex] = maxEnemyIndex;

            }
        }
        //If all the enemies have been added to the Q already, add the remaining players to the Q directly
        else if(pAgilities.Count>0 && eAgilities.Count<=0)
        {
            battleQueue.Add(players[maxPlayerIndex]);
            //Add the player's image to the UI
            uiBtl.AddImageToQ(players[maxPlayerIndex].playerReference.qImage);
            pAgilities.RemoveAt(pAgilities.Count - 1);
            removedPlayerIndexes[maxPlayerIndex] = maxPlayerIndex;
        }
        //If all the players have already been added to the Q, add the remaining enemies to the Q directly.
        else if(pAgilities.Count<=0 && eAgilities.Count>0)
        {
            battleQueue.Add(enemies[maxEnemyIndex]);
            //Add the enemy's image to the UI
            uiBtl.AddImageToQ(enemies[maxEnemyIndex].enemyReference.qImage);
            eAgilities.RemoveAt(eAgilities.Count - 1);
            removedEnemyIndexes[maxEnemyIndex] = maxEnemyIndex;
        }

        //If either of the agility lists isn't empty, run the function again
        if (pAgilities.Count>0 || eAgilities.Count>0)
        {
            BuildQueue();
        }
        //Otherwise, tell the UI system to show the Q
        else
        {
            uiBtl.QueueIsReady();
        }
    }


    //Called by each player at the end of each battle
    public void EndOfBattle(int playerIndex, float remainingHP, float remainingMP )
    {
        players[playerIndex].hp = remainingHP;
        players[playerIndex].mp = remainingMP;
        players[playerIndex].exp += 5; //Arbitrary number. How will we decide how much exp we'll get from each battle?
        
        if(players[playerIndex].exp>= players[playerIndex].expNeededForNextLevel)
        {
            LevelUp(playerIndex);
        }

        //Clear out the enemies array
        for (int i =0;i<5;i++)
        {
            enemies[i] = null;
        }
    }

    //Called by the inventory manager to update the player's stats when the player changes gear and on awake
    public void UpdateFromInv(int playerIndex,int hp, int mp, int atk, int def, int agi, int str, int crit)
    {
        players[playerIndex].hp = hp;
        players[playerIndex].mp = mp;
        players[playerIndex].atk = atk;
        players[playerIndex].def = def;
        players[playerIndex].agi = agi;
        players[playerIndex].crit = crit;
        players[playerIndex].str = str;
    }

    //Called by the exp manager on awake and when the player's level changes
    public void UpdateFromExp(int playerIndex, int currentExp, int maxExp)
    {
        players[playerIndex].exp = currentExp;
        players[playerIndex].expNeededForNextLevel = maxExp;

        //Update UI for max EXP
    }
    

    private void LevelUp(int playerIndex)
    {
        //The new EXP is what remains after reaching the new level
        players[playerIndex].exp = players[playerIndex].exp - players[playerIndex].expNeededForNextLevel;
        //Update UI and do level up audio

        //Call the EXP manager to level up --> This should update the expNeededForNextLevel

        //If the player gains enough EXP to level up more than once
        if (players[playerIndex].exp >= players[playerIndex].expNeededForNextLevel)
        {
            LevelUp(playerIndex);
        }
    }
}
