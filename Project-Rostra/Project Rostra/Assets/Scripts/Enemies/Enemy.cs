using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public int enemyIndexInBattleManager;
    private BattleManager battleManager;
    public int agi;
    public int str;
    public int crit;
    public string name;

    private void Start()
    {
        battleManager = BattleManager.instance; 
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            battleManager.AddEnemy(enemyIndexInBattleManager, agi, str, crit, this,name);
            Debug.Log("Added enemy to battlemanager");
        }
    }

    private void EndBattle()
    {
        battleManager.DeleteEnemyFromQueue(enemyIndexInBattleManager);
    }

    public void mynameis()
    {
        Debug.Log("Enemy " + name);
    }


}
