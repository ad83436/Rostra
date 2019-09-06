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

    private void Start()
    {
        battleManager = BattleManager.instance;
        battleManager.enemies[enemyIndexInBattleManager] = new BattleManager.PlayerInformtion();
        battleManager.AddEnemy(enemyIndexInBattleManager,agi,str,crit);
    }

    private void EndBattle()
    {
        battleManager.DeleteEnemyFromQueue(enemyIndexInBattleManager);
    }


}
