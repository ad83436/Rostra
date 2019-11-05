using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public string unloadScene;
	public string sceneToLoad;
	public bool unload;
	public static GameObject player;
    public Canvas canvasToTurnOff; //Need to turn off the world map canvas before going into subareas
    public EnemySpawner enemySpawner; //Sometimes we'll need to have boss battles inside subareas, we need to turn the boolean on

	private void OnTriggerEnter2D(Collider2D col)
	{
		
		if (col.CompareTag("Player"))
		{
            canvasToTurnOff.gameObject.SetActive(false);
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

            if (SceneManager.GetActiveScene().name == "PlayTest2" && unload == false)
			{
				Debug.Log("Disabled player");
				player = col.gameObject;
				player.SetActive(false);
			}

			if (unload == true)
			{
				SceneManager.UnloadSceneAsync(unloadScene);
				player.SetActive(true);
			}
		}
	}
}
