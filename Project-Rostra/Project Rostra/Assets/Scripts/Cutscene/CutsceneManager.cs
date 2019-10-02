// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
	public static CutsceneManager instance;
	private Cutscene copy;
	private Dialogue[] dialoguesCM;
	private Vector2[] movesCM;
	private float[] timingsCM;
	private float[] dialogueEntranceCM;
	private GameObject[] actorsCM;
	private float[] povChangesCM;
	private int current;
	private int moveCount;
	private int entranceCount;
	private int dialogueCount;
	private float dialogueLenght;
	private int timingCount;
	private float moveLenght;
	private float moveLenghtInitial;
	private int actorCount;
	private int povCount;
	public bool isActive = false;
	private float songEntrance;
	private AudioClip clip;
	private AudioSource audios;

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
		audios = GetComponent<AudioSource>();
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
		copy = new Cutscene(cs);

		dialoguesCM = copy.dialogues;
		dialogueEntranceCM = copy.dialogueEntrance;
		timingsCM = copy.timings;
		actorsCM = copy.actors;
		povChangesCM = copy.povChanges;
		movesCM = copy.moves;
		moveLenght = movesCM.Length - 1;
		moveLenghtInitial = movesCM.Length - 1;
		isActive = true;
		current = 0;
		entranceCount = 0;
		moveCount = 0;
		actorCount = 0;
		povCount = 0;
		dialogueCount = 0;
		dialogueLenght = copy.dialogueEntrance.Length - 1;
		clip = copy.song;
		songEntrance = copy.songEntrance;
	}

	public void NextAction()
	{
		if (current <= moveLenghtInitial)
		{
			if (DialogueManager.instance.isActive == false && moveLenght >= 0)
			{
				actorsCM[actorCount].GetComponent<Rigidbody2D>().velocity = movesCM[current];
				timingsCM[current] -= Time.deltaTime;
			}
			if (dialogueCount <= dialogueLenght && current == dialogueEntranceCM[entranceCount])
			{
				actorsCM[actorCount].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
				DialogueManager.instance.StartConversation(dialoguesCM[dialogueCount]);
				if (DialogueManager.instance.nextDialogue == true)
				{
					dialogueCount++;
					current++;
					Debug.Log("Triggered Dialogue");
					entranceCount++;
					Debug.Log("triggered dialogue and entrance count upped");
					
				}
			}
			if (clip != null && current == songEntrance)
			{
				audios.clip = copy.song;
				audios.Play();
				Debug.Log("Triggered Song");
			}
			if (isActive == true && timingsCM[current] <= 0 && moveLenght >= 0)
			{
				current++;
				moveLenght--;
			}
			if (povCount <= povChangesCM.Length - 1 && current == povChangesCM[povCount] && actorsCM.Length > 0 && povChangesCM.Length > 0)
			{

				actorCount++;
				povCount++;
				actorsCM[actorCount - 1].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			}
			
		}
		else
		{
			actorsCM[actorsCM.Length - 1].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			isActive = false;
			End();
			
		}
	}

	public void End()
	{
		current = 0;
		entranceCount = 0;
		moveCount = 0;
		actorCount = 0;
		povCount = 0;
		dialogueCount = 0;
	}
		
}
