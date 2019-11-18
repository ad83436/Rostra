using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WMEnemy : MonoBehaviour
{
    public static bool startTutorial = false;
	public bool tutorial = false;
    public Enemy[] enemies;
    public int[] enemyLevels;
    public Fade fadePanel;
    public GameObject endTestPanel;

    public EnemySpawner enemySpwn;
    private Collider2D enemyCollider;
    private SpriteRenderer enemySpriteRenderer;
    private float reActivateTime = 5.0f;
    private Vector2 startingPosition; //Used to reset the enemy should it not collide with the player in time

    public Sprite backgroundImage;

    private void Start()
    {
        enemySpwn = EnemySpawner.instance;

        if(endTestPanel)
        {
            endTestPanel.gameObject.SetActive(false);
        }
        enemyCollider = gameObject.GetComponent<Collider2D>();
        enemySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingPosition = gameObject.transform.position;

        if(fadePanel)
        {
            fadePanel.tutorial = tutorial;
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

        if(!BattleManager.battleInProgress && !NewWMEnemy.isActive && !enemyCollider.enabled && !PauseMenuController.isPaused) //If the battle has ended, and we're not moving
        {
            InvokeRepeating("SpawnEnemy", reActivateTime, 1);
        }
        else if (!BattleManager.battleInProgress && !NewWMEnemy.isActive && enemyCollider.enabled && !PauseMenuController.isPaused) //If two enemies race towards the player, the one who does not collide with the player should reset
        {
            enemyCollider.enabled = false;
            enemySpriteRenderer.enabled = false; //What if the player passes by the enemy? It must not be seen stuck like an idiot
            gameObject.transform.position = startingPosition;
        }
        else if(!BattleManager.battleInProgress && NewWMEnemy.isActive && !enemyCollider.enabled && !PauseMenuController.isPaused) //If you transition from one place to another, NEW WM enemy will go active, and so should the collider and SR
        {
            enemyCollider.enabled = true;
            if(enemySpriteRenderer!=null)
            enemySpriteRenderer.enabled = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Player"))
        {
            NewWMEnemy.isActive = false;
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
        if (tutorial)
            startTutorial = tutorial;
        SceneManager.LoadScene(tutorial ? "Queue Scene 2" : "Queue Scene", LoadSceneMode.Additive);
        if (endTestPanel)
        {
            endTestPanel.gameObject.SetActive(true);
        }

        enemyCollider.enabled = false;
        if(enemySpriteRenderer!=null)
        enemySpriteRenderer.enabled = false;

            PassInfoIntoBattle.battleBackgroundImage = backgroundImage;

    }

    public void SpawnEnemy()
    {
        if (!BattleManager.battleInProgress) //If the battle is active, don't turn on
        {
            enemyCollider.enabled = true;
            enemySpriteRenderer.enabled = true;
            NewWMEnemy.isActive = true;
        }
        else
        {
            InvokeRepeating("SpawnEnemy", reActivateTime, 1);

        }
    }
}
