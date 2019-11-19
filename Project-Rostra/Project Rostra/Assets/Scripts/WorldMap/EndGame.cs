//Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
	public ChoiceEnum ce;
	public string scene;
	private void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player") && DialogueManager.instance.GetChoice(ce) == true)
		{
			AudioManager.instance.PlayThisClip("End");
			SceneManager.LoadScene(scene);
		}
	}
}
