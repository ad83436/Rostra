// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
	public static CutsceneManager instance;
    public Fade fade;
	private Cutscene copy;
	private Dialogue[] dialoguesCM;
	private Vector2[] movesCM;
	private float[] timingsCM;
	private float[] dialogueEntranceCM;
	private Actor[] actorsCM;
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
	//private Animator anim;
	private float transitionTime;
	public GameObject pl;
	private Vector2 returnPosition;
	private bool fadeOut = false;
	private int maxCount;
	public ChoiceEnum ce;
	// Start is called before the first frame update
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
            DontDestroyOnLoad(this.gameObject);
            GameManager.instance.listOfUndestroyables.Add(this.gameObject);
        }
		else
		{
			Destroy(gameObject);
		}
	}

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
	{
		
		current = 0;
		entranceCount = 0;
		moveCount = 0;
		actorCount = 0;
		povCount = 0;
		maxCount = 0;
		//audios = GetComponent<AudioSource>();
	}
	// Update is called once per frame
	void Update()
	{
		if (isActive == true)
		{
			NextAction();
		}
	}

	public void StartCutscene(Cutscene cs, Vector2 returnPos, Fade f)
	{
        Debug.Log("start cutscene");
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
		//anim = actorsCM[0].GetComponent<Animator>();
		DialogueManager.instance.canWalk = false;
		fade = f; 

	}

	public void NextAction()
	{
        // if our current step is less than the amount of moves that means we can go ahead with the cutscene
        if (current <= moveLenghtInitial)
        {
            // vector that checks if the current move is outside the view of the camera
            if (actorsCM[actorCount] != null)
            {
                Vector3 canSeen = Camera.main.WorldToViewportPoint(new Vector3(movesCM[current].x, movesCM[current].y, actorsCM[actorCount].transform.localPosition.z));
                // if we aren't already in a cutscene
                if (DialogueManager.instance.isActive == false && moveLenght >= 0)
                {
                    timingsCM[current] -= Time.deltaTime;
                }
                // if our current move has dialogue attached to it
                if (dialogueCount <= dialogueLenght && current == dialogueEntranceCM[entranceCount])
                {
                    DialogueManager.instance.StartConversation(dialoguesCM[dialogueCount]);
                    // nothing should be happening here
                    //anim.ResetTrigger("FadeIn");
                    //anim.ResetTrigger("FadeOut");
                    //anim.SetBool("FadeIn", false);
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
					if (current + 1 < movesCM.Length)
                    {
                        current++;
                    }
                    else
                    {
                        fade.TransitionOutOfACutscene();
                    }
                    moveLenght--;
                    actorsCM[actorCount].MoveActor(new Vector3(movesCM[current].x, movesCM[current].y, actorsCM[actorCount].transform.localPosition.z));
                    // nothing should be happening here either
                }
                /// 
                if (povCount <= povChangesCM.Length - 1 && current == povChangesCM[povCount] && actorsCM.Length > 0 && povChangesCM.Length > 0)
                {
                    if (actorCount + 1 < actorsCM.Length)
                    {
                        actorCount++;
                        povCount++;
                        //anim = actorsCM[actorCount].GetComponent<Animator>();
                    }
                }
            }
            else
            {
                isActive = false;
                //End();
                fade.TransitionOutOfACutscene();
            }
        }
	}
	// this will reset everything to default

	public void End()
	{
		//Debug.Log("End");
		copy = null;
		current = 0;
		entranceCount = 0;
		moveCount = 0;
		actorCount = 0;
		povCount = 0;
		dialogueCount = 0;
		pl.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
		pl.transform.position = returnPosition;
		DialogueManager.instance.canWalk = true;
		DialogueManager.instance.End();
		foreach (Actor g in actorsCM)
		{
			if (g != null)
			{
				Destroy(g.gameObject);
			}
		}
	}

	public void TriggerBool(int c)
	{
		maxCount++;
		if (maxCount == c)
		{
			// this will set our story choice to be equal
			DialogueManager.instance.metAllChars = true;
			DialogueManager.instance.SetChoice(ChoiceEnum.metAllChars, true);
			QuestManager.AddMilestone(3);
		}
	}
}
