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
	public bool isInteractable;
	// these are gonna hold two possible cutscenes for the same trigger
	public ChoiceEnum ce;
	public bool isChoiceDependant;
	public void TriggerCutscene()
	{
		returnPositon = transform.position;
		CutsceneManager.instance.StartCutscene(cs, returnPositon);
	}
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && hasActivated == false && isInteractable == false && isChoiceDependant == false)
		{
			TriggerCutscene();
			hasActivated = true;
		}
		
	}

	private void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player") && hasActivated == false && isInteractable == true && Input.GetKeyDown(KeyCode.Z))
		{
			TriggerCutscene();
			hasActivated = true;
		}
		if (col.CompareTag("Player") && hasActivated == false && DialogueManager.instance.GetChoice(ce) == true && isChoiceDependant == true)
		{
			TriggerCutscene();
			hasActivated = true;
		}
	}
}
