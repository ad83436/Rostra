﻿// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneTrigger : MonoBehaviour
{
    public Fade fade;
	public Cutscene cs;
	private bool hasActivated = false;
	private Vector2 returnPositon;
	public bool isInteractable;
	// these are gonna hold two possible cutscenes for the same trigger
	public ChoiceEnum ce;
	public bool isChoiceDependant;
	public bool willCount; // will this cutscene be counted
	public int maxCount; // count how many cutscenes have passed by
	public bool stay; // if you stay in the cutscene and have it be triggered
	public bool directTrigger; // set to true if you don't wanna collide with the trigger
	public GameObject pl;

	private void Update()
	{
		if (directTrigger == true && isChoiceDependant == true && DialogueManager.instance.GetChoice(ce) == true && hasActivated == false)
		{
			fade.TransitionIntoACutscene(this);
			DialogueManager.instance.canWalk = false;
			pl.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
			hasActivated = true;
		}
	}
	public void TriggerCutscene()
	{
		returnPositon = transform.position;
		CutsceneManager.instance.StartCutscene(cs, returnPositon, fade);
	}
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && hasActivated == false && isInteractable == false && isChoiceDependant == false)
		{
            fade.TransitionIntoACutscene(this);
            DialogueManager.instance.canWalk = false;
            col.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            hasActivated = true;
			if (willCount == true)
			{
				CutsceneManager.instance.TriggerBool(maxCount);
			}
			CutsceneManager.instance.pl = col.gameObject;
		}
		if (col.CompareTag("Player") && hasActivated == false && DialogueManager.instance.GetChoice(ce) == true && isChoiceDependant == true && stay == false)
		{
			Debug.Log("We are in a choice dependant cutscene");
			fade.TransitionIntoACutscene(this);
			DialogueManager.instance.canWalk = false;
			col.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
			hasActivated = true;
			if (willCount == true)
			{
				CutsceneManager.instance.TriggerBool(maxCount);
			}
			CutsceneManager.instance.pl = col.gameObject;
		}
		
	}

	private void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player") && hasActivated == false && isInteractable == true && Input.GetButtonDown("Confirm"))
		{
			fade.TransitionIntoACutscene(this);
			col.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
			DialogueManager.instance.canWalk = false;
			hasActivated = true;
			CutsceneManager.instance.pl = col.gameObject;
		}
		if (col.CompareTag("Player") && hasActivated == false && DialogueManager.instance.GetChoice(ce) == true && stay == true && DialogueManager.instance.isActive == false)
		{
			fade.TransitionIntoACutscene(this);
			col.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
			DialogueManager.instance.canWalk = false;
			hasActivated = true;
			CutsceneManager.instance.pl = col.gameObject;
		}
	}

	// hey listen, if you find your way down here in the code I just want to say...... Hello! That is all......
}
