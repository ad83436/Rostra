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

}
