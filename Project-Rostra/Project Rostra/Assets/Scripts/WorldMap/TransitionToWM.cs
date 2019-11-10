﻿// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionToWM : MonoBehaviour
{
	public Vector2 loadPosition;
	public GameObject player;
	// If you've set this to true you need to 
	public bool needsBool;
	public ChoiceEnum ce;
	public bool transitionToWM;
	public bool stopsPlayer;
	public Collider2D collision;
	public Dialogue dia;
	// Start is called before the first frame update
	void Start()
    {
		if (stopsPlayer == false)
		{
			collision.enabled = false;
		}
    }

    // check to see if our actual collider needs to be turned off if our bool is now true
    void Update()
    {
		UpdateCollision();
	}

	// the scale of the player in the sub areas is three
	// we'll set it to one on the world map and see what happens

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && needsBool == true && DialogueManager.instance.GetChoice(ce) == true && transitionToWM == true && stopsPlayer == false)
		{
			player.transform.position = loadPosition;
			player.transform.localScale = new Vector3(2, 2, player.transform.localScale.z);
		}
		else if (col.CompareTag("Player") && needsBool == true && DialogueManager.instance.GetChoice(ce) == true && transitionToWM == false && stopsPlayer == false)
		{
			player.transform.position = loadPosition;
			player.transform.localScale = new Vector3(3, 3, player.transform.localScale.z);
		}
		else if (col.CompareTag("Player") && needsBool == false && transitionToWM == true && stopsPlayer == false)
		{
			player.transform.position = loadPosition;
			player.transform.localScale = new Vector3(2, 2, player.transform.localScale.z);
		}
		else if (col.CompareTag("Player") && needsBool == false && transitionToWM == false && stopsPlayer == false)
		{
			player.transform.position = loadPosition;
			player.transform.localScale = new Vector3(3, 3, player.transform.localScale.z);
		}
	}
	// start a conversation if your being blocked by a real collider... We'll need this for Hadria Domel's house
	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Player") && stopsPlayer == true)
		{
			DialogueManager.instance.StartConversation(dia);
		}
	}

	public void UpdateCollision()
	{
		if (DialogueManager.instance.GetChoice(ce) == true)
		{
			stopsPlayer = false;
			collision.enabled = false;
		}
	}
}