using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//oo oo ah ah
public class NewWMEnemy : MonoBehaviour
{
    public static bool isActive = true; //Changed by WM script after colliding with player. All enemies should stop when engaged in a battle
    public float speed;
    public float rSpeed;
    public float radius;

    public Transform enemy;
    private Transform target;
    public GameObject player;

    private bool isPatrolling = false;
    private bool isRotatingL = false;
    private bool isRotatingR = false;
    private bool isMoving = false;
    private bool isChasing = false;
  

    void Start()
    {
        target = player.GetComponent<Transform>();
    }

    void Update()
    {
        if (isActive && !PauseMenuController.isPaused) //If we open the pause menu, the enemies should stop
        {
            if (isPatrolling == false && isChasing == false)
            {
                StartCoroutine(Patrol());
            }

            if ((target.position - enemy.position).magnitude <= radius)
            {
                Chase();
            }

            if (isRotatingR == true)
            {
                transform.Rotate(0, 0, rSpeed * Time.deltaTime);
            }

            if (isRotatingL == true)
            {
                transform.Rotate(0, 0, -rSpeed * Time.deltaTime);
            }

            if (isMoving == true)
            {
                transform.position += (transform.up * speed * Time.deltaTime);
            }

        }

    }

    IEnumerator Patrol()
    {
        //Random between set numbers, ex: rTime picks a random number between 1 and 2
        int rTime = Random.Range(1, 2); 
        int rDelay = Random.Range(1, 4);
        int rotateLR = Random.Range(1, 2);
        int moveDelay = Random.Range(1, 4);
        int moveTime = Random.Range(1, 5);

        isPatrolling = true;

        yield return new WaitForSeconds(moveDelay);
        isMoving = true;
        yield return new WaitForSeconds(moveTime);
        isMoving = false;
        yield return new WaitForSeconds(rDelay);

        if (rotateLR == 1)
        {
            isRotatingR = true;
            yield return new WaitForSeconds(rTime);
            isRotatingR = false;
        }

        if (rotateLR == 2)
        {
            isRotatingL = true;
            yield return new WaitForSeconds(rTime);
            isRotatingL = false;
        }

        isPatrolling = false;
    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        isChasing = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemy.position, radius);
    }

}
