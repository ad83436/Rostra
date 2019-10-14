// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneTrigger : MonoBehaviour
{
	public Cutscene cs;
	private bool hasActivated = false;
	private Vector2 returnPositon;
	public void TriggerCutscene()
	{
		returnPositon = transform.position;
		CutsceneManager.instance.StartCutscene(cs, returnPositon);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && hasActivated == false)
		{
			TriggerCutscene();
			hasActivated = true;
		}

	}
}
