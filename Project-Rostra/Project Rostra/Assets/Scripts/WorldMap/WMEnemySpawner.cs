using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// oo oo ah ah
public class WMEnemySpawner : MonoBehaviour
{
    public GameObject WMEnemy;
    public bool delaySpawn = false;
    public float spawnTime;
    public float spawnDelay;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", spawnTime, spawnDelay);

    }

    public void SpawnEnemy()
    {
        Instantiate(WMEnemy, transform.position, Quaternion.identity);
        if (delaySpawn)
        {
            CancelInvoke("SpawnEnemy");
        }
    }
}
