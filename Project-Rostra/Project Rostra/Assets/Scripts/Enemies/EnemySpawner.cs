using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public Enemy[] enemiesToSpawn = new Enemy[5];
    public int[] enemyLevels = new int[5];
    public GameObject[] enemyPos = new GameObject[5]; //Provided by the BTL Manager before the battle starts
    private Enemy enemySpawned;
    public int numberOfEnemies = 0;
    public bool isBoss;

    #region singleton
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            GameManager.instance.listOfUndestroyables.Add(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    #endregion

    //Called from the World Map
    public void AddEnemyToSpawn(Enemy enemyToAdd, int index, int currentEnemyLevel)
    {

        enemiesToSpawn[index] = enemyToAdd;
        enemiesToSpawn[index].enemyIndexInBattleManager = index;
        enemyLevels[index] = currentEnemyLevel;
        numberOfEnemies++;
    }

    //Called from BTL Manager
    public void AddPos(GameObject pos, int index)
    {
        enemyPos[index] = pos;
    }

    //Called from BTL Manager
    public void SpawnEnemies()
    {
        if (isBoss) //If it is a boss, summon it in the middle spot. Turned true by a trigger
        {
            AudioManager.instance.PlayThisClip("BossMusic1");            
            enemySpawned = Instantiate(enemiesToSpawn[0], enemyPos[1].transform.position, gameObject.transform.rotation);
            enemySpawned.enemyIndexInBattleManager = 1;
            enemySpawned.IncreaseStatsBasedOnLevel(enemyLevels[0]);
            isBoss = false; //After you leave the trigger, go false
        }
        else
        {
            AudioManager.instance.PlayThisClip("BattleMusic1");
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i] != null)
                {
                    enemySpawned = Instantiate(enemiesToSpawn[i], enemyPos[i].transform.position, gameObject.transform.rotation);
                    enemySpawned.enemyIndexInBattleManager = i;
                    enemySpawned.IncreaseStatsBasedOnLevel(enemyLevels[i]);
                    enemiesToSpawn[i] = null; //reset
                }
            }
        }
        
    }
}
