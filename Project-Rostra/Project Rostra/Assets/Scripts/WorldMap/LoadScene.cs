using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public string unloadScene;
	public string sceneToLoad;
	public bool unload;
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
			SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
			if (unload == true)
			{
				SceneManager.UnloadSceneAsync(unloadScene);
			}

		}
	}
}
