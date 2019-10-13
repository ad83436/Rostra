// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneTrigger : MonoBehaviour
{
	public Cutscene cs;
	public void TriggerCutscene()
	{
		CutsceneManager.instance.StartCutscene(cs);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			TriggerCutscene();
		}

	}
}
