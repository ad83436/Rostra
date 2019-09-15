using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WMEnemy : MonoBehaviour
{
    public Enemy[] enemies;
    public int[] enemyLevels;

    public EnemySpawner enemySpwn;

    private void Start()
    {
        enemySpwn = EnemySpawner.instance;

        for (int i = 0; i < enemies.Length; i++)
        {
            Debug.Log("Enemy Level :" + enemyLevels[i]);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            for(int i =0;i<enemies.Length;i++)
            {
                enemySpwn.AddEnemyToSpawn(enemies[i], i, enemyLevels[i]);
            }
            SceneManager.LoadScene("Queue Scene");
        }

    }
}
