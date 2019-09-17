using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public int index;
    public string levelName;
    void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene("BattleSceneTest", LoadSceneMode.Additive);
                Debug.Log("asfkj");

            }
        }
    
}

