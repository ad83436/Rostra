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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D col)
	{
		
		if (col.CompareTag("Player"))
		{
			if (SceneManager.GetActiveScene().name == "PlayTest2" && unload == false)
			{
				Debug.Log("Disabled player");
				player = col.gameObject;
				player.SetActive(false);
			}
			SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

			if (unload == true)
			{
				SceneManager.UnloadSceneAsync(unloadScene);
				player.SetActive(true);
			}

		}
	}
}
