using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// oo oo ah ah
public class WMEnemyP : MonoBehaviour
{
    public float speed;
    public Transform[] patrolPos; // Array that holds all preset positions for patrolling enemies
    private int randomPos; //
    private float stopTime; // Time enemy will stop at each preset position
    public float startStopTime;
    public Transform player;
    public Transform interactable;
    float minDistance = 1.5f;

    void Start()
    {
        stopTime = startStopTime;
        randomPos = Random.Range(0, patrolPos.Length);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolPos[randomPos].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolPos[randomPos].position) < 0.2f)
        {
            if (stopTime <= 0)
            {
                randomPos = Random.Range(0, patrolPos.Length);
                stopTime = startStopTime;
            }
            else
            {
                stopTime -= Time.deltaTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && (transform.position - interactable.position).magnitude <= minDistance)
        {
            SceneManager.LoadScene("WorldMap", LoadSceneMode.Additive);
            Debug.Log("Oo oo ah ah");
        }

    }
    
}

