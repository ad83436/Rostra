using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //Player information is updated first from the EXP system at Awake
    [System.Serializable]
    public class PlayerInformtion
    {
        public int playerIndex;
        public int hp;
        public int mp;
        public string[] skills = new string[4];
        public int atk;
        public int def;
        public int agi;
        public int str;
        public int crit;
        public int exp;
        public int expNeededForNextLevel;
    }

    //At the beginning of each battle, each player and enemy will use the singleton to update their stats
    #region singleton

    public static BattleManager instance;

    #endregion

    public PlayerInformtion[] players;
    public PlayerInformtion[] enemies;
    private Queue<PlayerInformtion> battleQueue;
    private List<int> listOfAgilitiesOfCharactersAndEnemiesInBattle;

    private List<int> pAgilities; 
    private List<int> eAgilities; 

    private int maxPlayerAgi = 0;
    private int maxPlayerIndex = 0;
    private int previousMaxPlayerIndex = -1;
    private int maxEnemyAgi = 0;
    private int maxEnemyIndex = 0;
    private int previousMaxEnemyIndex = -1;

    private int test;

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

        //Each player and enemy would have an index that stores their information
        players = new PlayerInformtion[4]; //max 4 players
        enemies = new PlayerInformtion[5];//max 5 enemies
        pAgilities = new List<int>();
        eAgilities = new List<int>();
        battleQueue = new Queue<PlayerInformtion>();
        listOfAgilitiesOfCharactersAndEnemiesInBattle = new List<int>();
        listOfAgilitiesOfCharactersAndEnemiesInBattle.Sort();

    }


    private void Update()
    {
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

        if(Input.GetKeyDown(KeyCode.D))
        {
            test = battleQueue.Dequeue().agi;
            Debug.Log(test);
        }
    }

    //Called at the beginning of the battle to store references to current enemies. Needed to be able to update the queue
    public void AddEnemy(int enemyIndex, int agi, int str, int crit)
    {
            enemies[enemyIndex].playerIndex = enemyIndex;
            enemies[enemyIndex].agi = agi;
            enemies[enemyIndex].str = str;
            enemies[enemyIndex].crit = crit;
       
    }

    public void StartBattle()
    {
        //Sort the agilities of the players and enemies in ascending order

        for (int i = 0; i < players.Length; i++)
        {
            pAgilities.Add(players[i].agi);
        }
            

        pAgilities.Sort();


        foreach (PlayerInformtion e in enemies)
        {
            eAgilities.Add(e.agi);
        }

        eAgilities.Sort();

    }

    public void BuildQueue()
    {
        //Compare the player with the highest agility with the enemy with the highest agility
        if (pAgilities.Count > 0)
        {
            Debug.Log("Count :" + pAgilities.Count);
            maxPlayerAgi = pAgilities[pAgilities.Count - 1];
            Debug.Log(maxPlayerAgi);

            foreach (PlayerInformtion e in players)
            {
                if (maxPlayerAgi == e.agi)
                {
                    //What if two players have the same agility
                    if (e.playerIndex != previousMaxPlayerIndex)
                    {
                       // previousMaxPlayerIndex = maxPlayerIndex = e.playerIndex;
                    }
                    else
                    {
                       // previousMaxPlayerIndex = maxPlayerIndex = e.playerIndex + 1;
                    }
                    previousMaxPlayerIndex = maxPlayerIndex = e.playerIndex;
                }
            }
        }

        if (eAgilities.Count > 0)
        {
            maxEnemyAgi = eAgilities[eAgilities.Count - 1];

            foreach (PlayerInformtion e in enemies)
            {
                if (maxEnemyAgi == e.agi)
                {
                    //What if two enemies have the same agility
                    if (e.playerIndex != previousMaxEnemyIndex)
                    {
                        //previousMaxEnemyIndex = maxEnemyIndex = e.playerIndex;
                    }
                    else
                    {
                       // previousMaxEnemyIndex = maxEnemyIndex = e.playerIndex + 1;
                    }
                    previousMaxEnemyIndex = maxEnemyIndex = e.playerIndex;
                }
                
            }
        }

        if(maxPlayerAgi>=maxEnemyAgi)
        {
            if (maxPlayerAgi > 0)
            {
                Debug.Log("Max player index: " + maxPlayerIndex);
                battleQueue.Enqueue(players[maxPlayerIndex]);
                if (pAgilities.Count > 0)
                {
                    pAgilities.RemoveAt(pAgilities.Count - 1);
                }
                else
                {
                    maxPlayerAgi = 0; //If there are no more players, reset this to zero, just incase there are still enemies
                }
            }
            
        }
        else
        {
            if (maxEnemyAgi > 0)
            {
                battleQueue.Enqueue(enemies[maxEnemyIndex]);
            }
            if (eAgilities.Count > 0)
            {
                eAgilities.RemoveAt(eAgilities.Count - 1);
            }
            else
            {
                maxEnemyAgi = 0;
            }

        }

        if (pAgilities.Count>0 || eAgilities.Count>0)
        {
            BuildQueue();
        }
    }

    //Once an enemy is defeated, the enemy should be removed from the queue
    public void DeleteEnemyFromQueue(int enemyIndex)
    {
        enemies[enemyIndex] = null;
        battleQueue.Clear();
    }

    public void UpdateQueue()
    {
        //Sort agilities
        //Add players/enemies to the queue
    }

    //Called by each player at the end of each battle
    public void EndOfBattle(int playerIndex, int remainingHP, int remainingMP )
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
        UpdateToInv(playerIndex); //Update the inventory system
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
    
    //Update the inventory. Called after the battle has ended. Basically, the battle manager and inventory need to always be in sync
    public void UpdateToInv(int playerIndex)
    {
        //Update inventory with hp and mp
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
