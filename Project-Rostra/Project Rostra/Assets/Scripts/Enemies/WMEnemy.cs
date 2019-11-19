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
    public Collider2D enemyCollider;
    public Collider2D enemyPhysicalCollider;
    private SpriteRenderer enemySpriteRenderer;
    private Vector2 startingPosition; //Used to reset the enemy should it not collide with the player in time
    private NewWMEnemy moveScript;
    public Sprite backgroundImage;
    private bool collidedWithPlayer = false;
    private float collisionDelay = 7.8f;

    private void Start()
    {
        enemySpwn = EnemySpawner.instance;

        if (endTestPanel)
        {
            endTestPanel.gameObject.SetActive(false);
        }
        enemyCollider = gameObject.GetComponent<Collider2D>();
        enemySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        moveScript = gameObject.GetComponent<NewWMEnemy>();
        startingPosition = gameObject.transform.position;

        if (fadePanel)
        {
            fadePanel.tutorial = tutorial;
        }

    }

    private void Update()
    {
        if (!collidedWithPlayer)
        {
            if (BattleManager.battleInProgress && !NewWMEnemy.isActive && enemyCollider.enabled && !PauseMenuController.isPaused) //If two enemies race towards the player, the one who does not collide with the player should reset
            {
                if (enemyCollider != null)
                    enemyCollider.enabled = false;
                if (enemySpriteRenderer != null)
                    enemySpriteRenderer.enabled = false; //What if the player passes by the enemy? It must not be seen stuck like an idiot\
                if (enemyPhysicalCollider != null)
                    enemyPhysicalCollider.enabled = false;
                gameObject.transform.position = startingPosition;
            }
            else if (!BattleManager.battleInProgress && NewWMEnemy.isActive && enemyCollider.enabled && !PauseMenuController.isPaused) //If you transition from one place to another, NEW WM enemy will go active, and so should the collider and SR
            {
                ActivateEnemy();
                gameObject.transform.position = startingPosition;
                if (moveScript != null)
                {
                    moveScript.idleDelay = 0.0f;
                }
            }
            else if (!BattleManager.battleInProgress && NewWMEnemy.isActive && !enemyCollider.enabled && !PauseMenuController.isPaused)
            {
                ActivateEnemy();
            }
        }
        else if (collidedWithPlayer && !BattleManager.battleInProgress)
        {
            if (collisionDelay > 0.0f)
            {
                collisionDelay -= Time.deltaTime;
            }
            else
            {
                collisionDelay = 7.8f;
                collidedWithPlayer = false;
                ActivateEnemy();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals("Player") && !collidedWithPlayer)
        {
            NewWMEnemy.isActive = false;
            fadePanel.FlipFadeToBattle(this);
            BattleManager.battleInProgress = true;
            gameObject.transform.position = startingPosition;
            if (moveScript != null)
            {
                moveScript.currentState = NewWMEnemy.WMState.idle;
                moveScript.idleDelay = 5.0f;
            }
            if (enemyCollider != null)
                enemyCollider.enabled = false;
            if (enemyPhysicalCollider != null)
                enemyPhysicalCollider.enabled = false;
            if (enemySpriteRenderer != null)
                enemySpriteRenderer.enabled = false;
            collidedWithPlayer = true;
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
        if (enemyCollider != null)
            enemyCollider.enabled = false;
        if (enemySpriteRenderer != null)
            enemySpriteRenderer.enabled = false;
        if (enemyPhysicalCollider != null)
            enemyPhysicalCollider.enabled = false;

        PassInfoIntoBattle.battleBackgroundImage = backgroundImage;

    }

    public void ActivateEnemy()
    {
        if (enemyCollider != null)
            enemyCollider.enabled = true;
        if (enemySpriteRenderer != null)
            enemySpriteRenderer.enabled = true;
        if (enemyPhysicalCollider != null)
            enemyPhysicalCollider.enabled = true;
    }
}