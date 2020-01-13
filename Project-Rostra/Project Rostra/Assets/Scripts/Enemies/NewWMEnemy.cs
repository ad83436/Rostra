using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//oo oo ah ah
public class NewWMEnemy : MonoBehaviour
{
    public static bool isActive = true; //Changed by WM script after colliding with player. All enemies should stop when engaged in a battle
    public float speed;

    public GameObject player;

    public enum WMState
    {
        patrolling, //Determine new position to move to
        moving, //Move to new position
        chasing, //Chase the player
        idle //Go inactive for a while
    }

    public WMState currentState = WMState.patrolling;

    private float radius = 5.0f;
    private Vector2 newPos;
    private Vector2 startPos;
    private float direction = -1.0f;
    public float idleDelay = 8.0f;
    private float chaseTimer = 3.0f;

    void Start()
    {
        idleDelay = 8.0f;
        currentState = WMState.patrolling;
        startPos = gameObject.transform.position;
        newPos = new Vector2(Random.Range(startPos.x, startPos.x + radius * direction), Random.Range(startPos.y, startPos.y + radius * direction));
    }

    void Update()
    {
        if (isActive && !PauseMenuController.isPaused && !BattleManager.battleInProgress && !DialogueManager.instance.isActive && !CutsceneManager.instance.isActive) //If we open the pause menu, the enemies should stop
        {
            switch (currentState)
            {
                case WMState.patrolling: //Choose a new pos to go to
                    direction *= -1.0f; //Flip the direction every turn
                    startPos = gameObject.transform.position;
                    newPos.x = Random.Range(startPos.x, startPos.x + radius * direction);
                    newPos.y = Random.Range(startPos.y, startPos.y + radius * direction);
                    currentState = WMState.moving;
                    break;
                case WMState.moving: //Move towards new pos
                    transform.position = Vector2.MoveTowards(gameObject.transform.position, newPos, speed * Time.deltaTime);

                    if (Mathf.Abs(player.transform.position.x - gameObject.transform.position.x) <= radius) //If the player is close enough, give chase
                    {
                        currentState = WMState.chasing;
                        chaseTimer = 3.0f;
                    }

                    else if (direction > 0.0f)
                    {
                        if (transform.position.x >= newPos.x && transform.position.y >= newPos.y)
                        {
                            currentState = WMState.idle; //Stop
                        }
                    }
                    else if (direction < 0.0f)
                    {
                        if (transform.position.x <= newPos.x && transform.position.y <= newPos.y)
                        {
                            currentState = WMState.idle; //Stop
                        }
                    }

                    break;
                case WMState.chasing: //Chase player

                    if (Mathf.Abs(player.transform.position.x - gameObject.transform.position.x) <= radius) //If the player is within range, keep chasing
                    {
                        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
                    }
                    else //If the player is out of range, chase for a while then give up
                    {
                        if (chaseTimer > 0.0f)
                        {
                            chaseTimer -= Time.deltaTime;
                            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
                        }
                        else
                        {
                            chaseTimer = 3.0f;
                            currentState = WMState.idle;
                        }
                    }

                    break;
                case WMState.idle: //Stay still for a while

                    if (idleDelay > 0.0f)
                    {
                        idleDelay -= Time.deltaTime;
                    }
                    else
                    {
                        idleDelay = 8.0f;
                        currentState = WMState.patrolling;
                    }
                    break;
            }
        }

    }

}