// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum ChoiceEnum : byte
{
	// hey sean why does the list start at 10 then goes to 1? I mean what kind of shitty programming is that?
	// I don't wanna have to add an extra bool to check to see if a convo wants to flip a bool if zero do nothing
	// in an ideal world I wouldn't do this but i'm running out of time and fuck it. If whoever is reading this wants to do it
	// maybe i'll buy lunch 



	// Nevermind was bugging me too so I moved it down
	guild = 1,
	kill =  2,
	spare = 3,
	tell =  4,
	lie =   5,
	demo =  6,
	talkToCo = 7,
	metAllChars = 8,
	sawJournal = 9,
	dwarf = 10,
	talkToContact = 11,
	battleFarea = 12,
	unTriggerable = 15, // this is a bool that will literally never be set to true
}


public class DialogueManager : MonoBehaviour
{
	public static DialogueManager instance;
	// link to name UI
	public Text charName;
	// text ui
	public Text text;
	// portrait ui
	public Image portrait;
	// a queue containing all of our sentences
	private Queue<string> textElements;
	// what line will the change occur
	private float change;
	// how many changes are we in so far?
	private int currentChange;
	// how many boxes have gone by
	private int boxCount;
	// local version of the dialgue that was passed to the manager
	private Dialogue dia;
	// what is the lenght of the array
	private float lenght;
	// stores the continue button
	public GameObject continueButton;
	// goes up by one every loop
	private float continueCount;
	// the number of char's in the sentence
	private float continueCountTotal;
	//what line will the choice apppear on
	private float choiceCount;
	public Text choice1;
	public Text choice2;
	public Animator anim;
	// can you skip to next box
	public bool canEnter;
	private int choiceNum;
	// highlighting boxes
	public GameObject highlight1;
	public GameObject highlight2;
	// a few bools that are relevant to the text and story progression of the game
	public bool dwarf;
	public bool guild;
	// set 1 ^ 
	public bool kill;
	public bool spare;
	// set 2 ^
	public bool tell;
	public bool lie;
	//set 3 ^
	public bool talkedToCo; // 7
	public bool metAllChars; // 8 
	public bool sawJournal; // 9
	public bool battleFarea; // 12
	public bool talkedToContact;//11
	// stores a local copy of which choice set we will be using
	private float choiceSet;
	public bool[] choices;
	public bool[] normalChoices; // determines if this will fire a normal convo or not
	// determines weather the player can walk or not
	public bool canWalk;
	// tells the DM if we should be checking for input
	private bool startUpdating;
	// can the next dialogue start
	public bool nextDialogue;
	// when can the next dialogue start
	private float nextDialogueCount;
	public float nextDialogueCountInitial;
	// is a textbox active
	public bool isActive;
	// how many conversations have we had
	public float willCount;
	// has the will count been triggered
	private bool hasCountTriggered;
	// a dialogue container that has any we need that's not attached to a specific object
	public DialogueContainer dc;
	//refernce to the fade object
	public Fade fade;
	// will this convo result in a battle
	private bool battle;
	// a story bool i use only for the demo
	public bool demo;
	// returns the story choice that you want
	private float zTypeSpeed;
	public bool GetChoice(ChoiceEnum choice)
	{
		return choices[(int)choice];
	}
	public void SetChoice(ChoiceEnum choice, bool b)
	{
		choices[(int)choice] = b;
	}

	private void Awake()
	{
		// singleton notation
		if (instance == null)
		{
			instance = this;
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


    void Start()
    {
		DontDestroyOnLoad(this.gameObject);
        // set everything to its default 
        textElements = new Queue<string>();
		choices = new bool[18]; // was 7
		normalChoices = new bool[7];
		change = 0;
		currentChange = 0;
		boxCount = 0;
		continueCount = 0;
		continueCountTotal = 0;
		choiceCount = 0;
		text.text = "";
		charName.text = "";
		choice1.gameObject.SetActive(false);
		choice2.gameObject.SetActive(false);
		canEnter = false;
		choiceNum = 0;
		highlight1.SetActive(false);
		highlight2.SetActive(false);
		canWalk = true;
		startUpdating = false;
		nextDialogue = true;
		nextDialogueCount = nextDialogueCountInitial;
		isActive = false;
		willCount = 0;
		hasCountTriggered = false;
		battle = false;
		demo = false;
		talkedToCo = false;
		sawJournal = false;
		talkedToContact = false;
		metAllChars = false;
		battleFarea = false;
		zTypeSpeed = 1;
	}

	public void StartConversation(Dialogue d)
	{
		Debug.Log("Start Convo");
		canEnter = false;
		if (d.willCount == true)
		{
			willCount++;
			d.willCount = false;
		}
		
		if (d.isChoice == true)
		{
			d.hasPlayed = true;
		}

		if (nextDialogue == true && isActive == false)
		{
			if (d.addMilestone > 0)
			{
				AddMilestone(d.addMilestone);
			}
			text.text = "";
			//turn off the highlighting and set everything to default in case it wasn't reset
			highlight1.SetActive(false);
			highlight2.SetActive(false);
			canEnter = false;
			choiceNum = 0;
			anim.SetBool("isOpen", true);
			// save a local copy of the dialogue we pass in
			dia = d;
			// if we need to add an item do it now so won't have to worry about it later 
			if (dia.addItem == true && dia.itemId > 0)
			{
				AddItem(dia.itemId, dia.itemNum);
			}
			// display the first name and portraits
			charName.text = d.names[0];
			portrait.sprite = d.portrait[0];
			textElements.Clear(); // wipe the slate clean make sure 
			// this loop is going to go through each individual text and add it to the queue
			foreach (string s in d.sentences)
			{
				// loop and queue up the elements
				textElements.Enqueue(s);
			}

			NextSentence(); // show the next sentence call the end script if no more text
							// get the lenght of our povchange array
			
			lenght = d.povChange.Length;
			continueButton.SetActive(false);
			//if we set the boolean to enable a choice tree we save a local copy of it
			if (dia.isChoice == true)
			{
				choiceCount = dia.choiceLine;
			}
			if (choiceCount > dia.sentences.Length && dia.isChoice == true)
			{
				Debug.LogError("You set the choice to be on a line that doesn't exist, fix it");
			}
			// disable the choice markers
			choice1.gameObject.SetActive(false);
			choice2.gameObject.SetActive(false);
			// check to see if the choice presented will be remembered and which choice set will we be remembering
			if (d.willRemember == true)
			{
				choiceSet = d.choiceSet;
			}
			else
			{
				choiceSet = 0;
			}
			canWalk = false;
			startUpdating = true;
			isActive = true;
			// trigger bool was right here in case it breaks
			
			d.hasPlayed = true;
			
		}

	}

	public void NextSentence()
	{
		print("talking");
		// wipe the previous text
		text.text = "";
		continueButton.SetActive(false);
		choice1.gameObject.SetActive(false);
		choice2.gameObject.SetActive(false);
		// set a local variable to be equal to the variable stored in the dialogue trigger
		if (dia.povChange.Length > 0)
		{
			change = dia.povChange[currentChange];
		}
		else
		{
			change = -1;
		}
		// if the count of our queue is zero go home
		if (textElements.Count == 0)
		{
			End();
			return;
		}
		else if (boxCount == change)
		{
			currentChange++;
			charName.text = dia.names[currentChange];
			portrait.sprite = dia.portrait[currentChange];
		}
		if (change > dia.sentences.Length && dia.isChoice == true)
		{
			change = -1;
			Debug.LogError("The Line change float is more lines than the array contains, fix it!"); 
		}
		// this is just going to take the first sentence out of the queue.
		string sentence = textElements.Dequeue();
		StartCoroutine(TypeLetters(sentence));
		boxCount++;
	}
	// go home you done
	public void End()
	{
		Debug.Log("End");
		if (dia.triggerBool > 0)
		{
			Debug.Log(dia.triggerBool);
			switch (dia.triggerBool)
			{
				case 1:
					guild = true;
					SetChoice(ChoiceEnum.guild, true);
					talkedToContact = true;
					SetChoice(ChoiceEnum.talkToContact, true);
					Debug.Log("Chose the Guild");
					break;
				case 2:
					kill = true;
					SetChoice(ChoiceEnum.kill, true);
					Debug.Log("Killed Farya");
					break;
				case 3:
					spare = true;
					SetChoice(ChoiceEnum.spare, true);
					Debug.Log("Spared Farya");
					break;
				case 6:
					demo = true;
					SetChoice(ChoiceEnum.demo, true);
					Debug.Log("We can leave the tavern");
					break;
				case 7:
					talkedToCo = true;
					SetChoice(ChoiceEnum.talkToCo, true);
					Debug.Log("We talked to the commander!");
					break;
				case 9:
					sawJournal = true;
					SetChoice(ChoiceEnum.sawJournal, true);
					Debug.Log("Saw the journal");
					break;
				case 10:
					dwarf = true;
					SetChoice(ChoiceEnum.dwarf, true);
					talkedToContact = true;
					SetChoice(ChoiceEnum.talkToContact, true);
					Debug.Log("Chose the Dwarves");
					break;
				case 12:
					battleFarea = true;
					SetChoice(ChoiceEnum.battleFarea, true);
					Debug.Log("Battled the Farea");
					break;

			}
		}
		charName.text = "";
		text.text = "";
		portrait.sprite = null;
		currentChange = 0;
		boxCount = 0;
		lenght = 0;
		continueCountTotal = 0;
		continueCount = 0;
		anim.SetBool("isOpen", false);
		choiceNum = 0;
		highlight1.SetActive(false);
		highlight2.SetActive(false);
		canWalk = true;
		startUpdating = false;
		nextDialogue = false;
		isActive = false;
		choiceCount = 0;
		if (willCount == dia.maxWillCount && hasCountTriggered == true)
		{
			willCount = 0;
			dia = null;
			hasCountTriggered = false;
		}
		if (dia != null && dia.isBattle == true)
		{
			fade = GameObject.Find("Fade").GetComponent<Fade>();
			battle = true;
		}
		
	}
	// this is a coroutine that will take our chars from the string and print one at a time 
	IEnumerator TypeLetters(string s)
	{
		canEnter = false;
		text.text = "";
		if (s.Contains("<i>") == true)
		{
			text.fontStyle = FontStyle.Italic;
			s = s.Replace("<i>", "");
		}
		else
		{
			text.fontStyle = FontStyle.Normal;
		}
		continueCountTotal = 0;
		continueCountTotal = s.ToCharArray().Length;
		foreach (char l in s.ToCharArray())
		{
			//canEnter = false;
			text.text += l;
			continueCount++;
			// if the string has stopped printing then you can continue
			if (continueCount == continueCountTotal)
			{
				continueButton.SetActive(true);
				canEnter = true;
				continueCountTotal = 0;
				continueCount = 0;
				Debug.Log("DoneTyping");
			}
			
			// diable the continue and show our choices
			// added the check for is choice
			if (boxCount == choiceCount && dia.isChoice == true)
			{
				Debug.Log("Choice reached");
				continueButton.SetActive(false);
				choice1.gameObject.SetActive(true);
				choice2.gameObject.SetActive(true);
				choice1.text = dia.choiceText1;
				choice2.text = dia.choiceText2;
			}
			yield return new WaitForSeconds(dia.typingSpeed * zTypeSpeed /*(Input.GetButton("Confirm") ? 0.01f : 1f)*/);
		}
	}
	// did you pick door 1 
	public void SelectFirstChoice()
	{
		if (dia.willRemember == true)
		{
			switch (choiceSet)
			{
				case 1:
					dwarf = true;
					normalChoices[1] = true;
					SetChoice(ChoiceEnum.dwarf, true);
					break;
				case 2:
					kill = true;
					normalChoices[2] = true;
					SetChoice(ChoiceEnum.kill, true);
					break;
				case 3:
					tell = true;
					normalChoices[3] = true;
					SetChoice(ChoiceEnum.tell, true);
					break;
				default:
					Debug.LogError("You wanted a story choice but passed no choice set, fix it you idiot");
					break;
			}
			End();
			if (dia.choice1 != null && dia.choice1.dialogue != null)
			{
				End();
				nextDialogue = true;
				StartConversation(dia.choice1.dialogue);

			}
		}
		

	}
	// or door two
	public void SelectSecondChoice()
	{
		if (dia.willRemember == true)
		{
			switch (choiceSet)
			{
				case 1:
					guild = true;
					normalChoices[4] = true;
					SetChoice(ChoiceEnum.guild, true);
					break;
				case 2:
					spare = true;
					normalChoices[5] = true;
					SetChoice(ChoiceEnum.spare, true);
					break;
				case 3:
					lie = true;
					normalChoices[6] = true;
					SetChoice(ChoiceEnum.lie, true);
					break;
				default:
					Debug.LogError("You wanted a story choice but passed no choice set");
					break;
			}
			End();
			if  (dia.choice2 != null && dia.choice2.dialogue != null)
			{
				End();
				nextDialogue = true;
				StartConversation(dia.choice2.dialogue);
			}
		}
		
	}
	// call this method when a conversation depends of choices made
	public void ChoiceDependantConvo(float choice, Dialogue d)
	{
		dia = d;
		// if the choice is more than half of the array take away half the array to get it's counterpart
		if (d.choiceCare1.dialogue.hasPlayed == true && d.choiceCare1.dialogue.isOneShot == true)
		{
			StartConversation(dia.choiceCare1.dialogue.normal.dialogue);
			if (dia.normal.dialogue.addMilestone > 0)
			{
				AddMilestone(dia.choice1.dialogue.normal.dialogue.addMilestone);
			}
			if (dia.normal.dialogue.addItem == true && dia.normal.dialogue.itemId > 0)
			{
				AddItem(dia.normal.dialogue.itemId, dia.normal.dialogue.itemNum);
			}
		}
		else
		{
			if (choice > normalChoices.Length / 2 && (normalChoices[(int)choice] == false && normalChoices[(int)choice - normalChoices.Length / 2] == false))
			{
				StartConversation(dia.normal.dialogue);
				if (dia.normal.dialogue.addMilestone > 0)
				{
					AddMilestone(dia.choice1.dialogue.normal.dialogue.addMilestone);
				}
				if (dia.normal.dialogue.addItem == true && dia.normal.dialogue.itemId > 0)
				{
					AddItem(dia.normal.dialogue.itemId, dia.normal.dialogue.itemNum);
				}
				Debug.Log((int)choice - normalChoices.Length / 2);
			}
			// if it's less than half add half
			else if (choice <= normalChoices.Length / 2 && (normalChoices[(int)choice] == false && normalChoices[(int)choice + normalChoices.Length / 2] == false))
			{
				StartConversation(dia.normal.dialogue);
				if (dia.normal.dialogue.addItem == true && dia.normal.dialogue.itemId > 0)
				{
					AddItem(dia.normal.dialogue.itemId, dia.normal.dialogue.itemNum);
				}

				Debug.Log((int)choice + normalChoices.Length / 2);
			}
			// init dialogue 1
			else if (normalChoices[(int)choice] == true)
			{
				StartConversation(dia.choiceCare1.dialogue);
				dia.choiceCare1.dialogue.hasPlayed = true;
				if (dia.choiceCare1.dialogue != null && dia.choiceCare1.dialogue.addItem == true && dia.choiceCare1.dialogue.itemId > 0)
				{
					AddItem(dia.choiceCare1.dialogue.itemId, dia.choiceCare1.dialogue.itemNum);
				}
				if (dia.choice1.dialogue.addMilestone > 0)
				{
					AddMilestone(dia.choice1.dialogue.addMilestone);
				}
			}
			// init dialogue 2
			else if (normalChoices[(int)choice] == false)
			{
				StartConversation(dia.choiceCare2.dialogue);
				dia.choiceCare1.dialogue.hasPlayed = true;
			}

		}
	}

	public void PlayNormalDialogue(Dialogue d)
	{
		StartConversation(d.normal.dialogue);
		if ( d.normal != null && d.normal.dialogue.addItem == true && d.normal.dialogue.itemId > 0)
		{
			AddItem(d.normal.dialogue.itemId, d.normal.dialogue.itemNum);
		}
	}
	// check our keyboard pressed and do things accordingly
	public void CheckInput()
	{
		if (Input.GetButtonDown("Confirm") && canEnter == true && boxCount != choiceCount)
		{
			NextSentence();
		}
		if (dia.isChoice == true && boxCount == choiceCount && Input.GetKeyDown(KeyCode.LeftArrow) && canEnter == true)
		{
			choiceNum = 1;
		}
		else if (dia.isChoice == true && boxCount == choiceCount && Input.GetKeyDown(KeyCode.RightArrow) && canEnter == true)
		{
			choiceNum = 2;
		}
		if (Input.GetButtonDown("Confirm") && choiceNum == 1 && boxCount == choiceCount && dia.isChoice == true && canEnter == true)
		{
			SelectFirstChoice();
		}
		else if (Input.GetButtonDown("Confirm") && choiceNum == 2 && boxCount == choiceCount && dia.isChoice == true && canEnter == true)
		{
			SelectSecondChoice();
		}
		if (choiceNum == 1)
		{
			highlight1.SetActive(true);
			highlight2.SetActive(false);
			choice1.color = Color.white;
			choice2.color = Color.yellow;
		}
		else if (choiceNum == 2)
		{
			highlight1.SetActive(false);
			highlight2.SetActive(true);
			choice1.color = Color.yellow;
			choice2.color = Color.white;
		}
		if (Input.GetButton("Confirm"))
		{
			zTypeSpeed = 0.01f;
		}
		else
		{
			zTypeSpeed = 1f;
		}
	}
	// Update..... I don't know what to put here
	public void Update()
	{
		if (startUpdating == true)
		{
			CheckInput();

		}
		if (nextDialogue == false)
		{
			nextDialogueCount -= Time.deltaTime;
		}
		if (nextDialogueCount < 0)
		{
			nextDialogue = true;
			nextDialogueCount = nextDialogueCountInitial;
		}
		if (dia != null && willCount == dia.maxWillCount && nextDialogue == true && isActive == false && dia.maxWillCount != 0)
		{
			hasCountTriggered = true;
			StartConversation(dc.doneTalkingToNPCS);
			// temporary just for demo
			choices[6] = true;
		}
		if (battle == true && isActive == false)
		{
			fade.FlipFadeToBattle();
			battle = false;
		}
	}

	public void AddMilestone(int i)
	{
		QuestManager.AddMilestone(i);
	}
	public void AddItem(int id, int num)
	{
			MainInventory.invInstance.AddItem(id, num);
	}
}
