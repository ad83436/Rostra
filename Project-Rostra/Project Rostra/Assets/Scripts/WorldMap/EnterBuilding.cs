using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EnterBuilding : MonoBehaviour
{//oo oo ah ah
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            Debug.Log("aksdjsa");
            SceneManager.LoadScene("DungeonTest", LoadSceneMode.Additive);
        }
    }
}

