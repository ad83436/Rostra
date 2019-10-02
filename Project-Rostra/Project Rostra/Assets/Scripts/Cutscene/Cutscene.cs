// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cutscene
{
	// what dialogues are we gonna store
    public Dialogue[] dialogues;
	// what vectors are we gonna pass to the player
	public Vector2[] moves;
	// how long will each move last for. 
	public float[] timings;
	// during what amount of moves will the dialogue be shown.
	public float[] dialogueEntrance;
	public GameObject[] actors;
	public float[] povChanges;
	public float songEntrance;
	public AudioClip song;
	// copy constructor
	public Cutscene(Cutscene other)
	{
		dialogues = new Dialogue[other.dialogues.Length];
		for (int i = 0; i < other.dialogues.Length; i++)
		{
			dialogues[i] = other.dialogues[i];
		}

		moves = new Vector2[other.moves.Length];
		for (int i = 0; i < other.moves.Length; i++)
		{
			moves[i] = other.moves[i];
		}

		timings = new float[other.timings.Length];
		for (int i = 0; i < other.timings.Length; i++)
		{
			timings[i] = other.timings[i];
		}

		dialogueEntrance = new float[other.dialogueEntrance.Length];
		for (int i = 0; i < other.dialogueEntrance.Length; i++)
		{
			dialogueEntrance[i] = other.dialogueEntrance[i];
		}

		actors = new GameObject[other.actors.Length];
		for (int i = 0; i < other.actors.Length; i++)
		{
			actors[i] = other.actors[i];
		}

		povChanges = new float[other.povChanges.Length];
		for (int i = 0; i < other.povChanges.Length; i++)
		{
			povChanges[i] = other.povChanges[i];
		}
		other.songEntrance = songEntrance;
		other.song = song;
	}

}
