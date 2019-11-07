using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WMEnemy : MonoBehaviour
{
	public bool tutorial = false;
    public Enemy[] enemies;
    public int[] enemyLevels;
    public Fade fadePanel;
    public GameObject endTestPanel;

    public EnemySpawner enemySpwn;

    private void Start()
    {
        enemySpwn = EnemySpawner.instance;

        if(endTestPanel)
        {
            endTestPanel.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (endTestPanel)
            {
                endTestPanel.gameObject.SetActive(false);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Player"))
        {
            fadePanel.FlipFadeToBattle(this);
            BattleManager.battleInProgress = true;
        }
    }

    public void TransitionIntoBattle()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemySpwn.AddEnemyToSpawn(enemies[i], i, enemyLevels[i]);
        }
        SceneManager.LoadScene(tutorial ? "Queue Scene 2" : "Queue Scene", LoadSceneMode.Additive);
        if (endTestPanel)
        {
            endTestPanel.gameObject.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
