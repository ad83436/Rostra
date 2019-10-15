// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	private Animator anim;
	private AnimationClip fadeOut;
	private float transitionTime;
	public GameObject pl;
	private Vector2 returnPosition;
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

	public void StartCutscene(Cutscene cs, Vector2 returnPos)
	{
		// call the copy constructor and save a local copy of what cutscene we passed in
		// set up the current cutscene info
		copy = new Cutscene(cs);
		pl.transform.position = cs.camLocation;
		returnPosition = returnPos;
		pl.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
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
		anim = actorsCM[0].GetComponent<Animator>();
		DialogueManager.instance.canWalk = false;
	}

	public void NextAction()
	{
		// if our current step is less than the amount of moves that means we can go ahead with the cutscene
		if (current <= moveLenghtInitial)
		{
			// if we aren't already in a cutscene
			if (DialogueManager.instance.isActive == false && moveLenght >= 0)
			{
				//fade in
				if (timingsCM[current] <= anim.GetCurrentAnimatorStateInfo(0).length)
				{
					anim.SetBool("FadeIn", true);
				}
				timingsCM[current] -= Time.deltaTime;
			}
			// if our current move has dialogue attached to it
			if (dialogueCount <= dialogueLenght && current == dialogueEntranceCM[entranceCount])
			{
				DialogueManager.instance.StartConversation(dialoguesCM[dialogueCount]);
				anim.SetBool("FadeIn", false);
				if (DialogueManager.instance.nextDialogue == true)
				{
					dialogueCount++;
					entranceCount++;
				}
			}
			if (clip != null && current == songEntrance)
			{
				audios.clip = copy.song;
				audios.Play();
			}
			// if our timings is less than zero move onto next move and count up the timers
			if (isActive == true && timingsCM[current] <= 0 && moveLenght >= 0)
			{
				Debug.Log("Next");
				current++;
				moveLenght--;
				actorsCM[actorCount].transform.position = movesCM[current];
				anim.SetBool("FadeIn", false);
			}
			/// 
			if (povCount <= povChangesCM.Length - 1 && current == povChangesCM[povCount] && actorsCM.Length > 0 && povChangesCM.Length > 0)
			{
				actorCount++;
				povCount++;
				anim = actorsCM[actorCount].GetComponent<Animator>();
			}
		}
		else
		{
			isActive = false;
			End();
			
		}
	}
	// set will reset everything to default

	public void End()
	{
		current = 0;
		entranceCount = 0;
		moveCount = 0;
		actorCount = 0;
		povCount = 0;
		dialogueCount = 0;
		pl.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
		pl.transform.position = returnPosition;
		DialogueManager.instance.canWalk = true;
	}	

	
}
