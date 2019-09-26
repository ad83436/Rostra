// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
	public static CutsceneManager instance;
	Cutscene copy;
	Dialogue[] dialogues;
	Vector2[] moves;
	private float[] timings;
	private float[] dialogueEntrance;
	private GameObject[] actors;
	private float[] povChanges;
	private int current;
	private int moveCount;
	private int entranceCount;
	private int dialogueCount;
	private int timingCount;
	private float moveLenght;
	private float moveLenghtInitial;
	private int actorCount;
	private int povCount;
	public bool isActive = false;

	// Start is called before the first frame update
	void Awake()
    {
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
		current = 0;
		entranceCount = 0;
		moveCount = 0;
		actorCount = 0;
		povCount = 0;
	}

    // Update is called once per frame
    void Update()
    {
		if (isActive == true)
		{
			NextAction();
		}
    }

	public void StartCutscene(Cutscene cs)
	{
		copy = cs;
		moves = cs.moves;
		timings = cs.timings;
		dialogues = cs.dialogues;
		dialogueEntrance = cs.dialogueEntrance;
		timings = cs.timings;
		actors = cs.actors;
		povChanges = cs.povChanges;
		moveLenght = moves.Length - 1;
		moveLenghtInitial = moves.Length - 1;
		isActive = true;
		current = 0;
		entranceCount = 0;
		moveCount = 0;
		actorCount = 0;
		povCount = 0;
	}

	public void NextAction()
	{
		if (current <= moveLenghtInitial)
		{
			if (DialogueManager.instance.isActive == false && moveLenght >= 0)
			{
				actors[actorCount].GetComponent<Rigidbody2D>().velocity = moves[current];
				timings[current] -= Time.deltaTime;
			}
			if (current == dialogueEntrance[entranceCount])
			{
				actors[actorCount].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
				DialogueManager.instance.StartConversation(dialogues[dialogueCount]);
				if (DialogueManager.instance.nextDialogue == true)
				{
					dialogueCount++;
					current++;
					if (entranceCount <= povChanges.Length - 1)
					{
						entranceCount++;
					}
				}
			}
			if (isActive == true && timings[current] <= 0 && moveLenght >= 0)
			{
				current++;
				moveLenght--;
			}
			if (povCount <= povChanges.Length - 1 && current == povChanges[povCount] && actors.Length > 0 && povChanges.Length > 0)
			{

				actorCount++;
				povCount++;
				actors[actorCount - 1].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			}
			if (moveLenght <= 0)
			{
				
				
			}
		}
		else
		{
			actors[actors.Length - 1].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			isActive = false;
		}
	}
		
}
