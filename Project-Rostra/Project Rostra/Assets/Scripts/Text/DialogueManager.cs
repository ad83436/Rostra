// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
	// stores a local copy of which choice set we will be using
	private float choiceSet;
	private bool[] choices;
	// determines weather the player can walk or not
	public bool canWalk;
	private bool startUpdating;
	public bool nextDialogue;
	private float nextDialogueCount;
	public float nextDialogueCountInitial;
	public bool isActive;
	void Awake()
    {
		// singleton notation
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogError("The Dialogue Guy is missing and sweet Jesus I can't find him");
		}
		DontDestroyOnLoad(this.gameObject);
		// set everything to its default 
		textElements = new Queue<string>();
		choices = new bool[6];
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
	}

	public void StartConversation(Dialogue d)
	{
		if (nextDialogue == true)
		{
			text.text = "";
			//turn off the highlighting and set everything to default in case it wasn't reset
			highlight1.SetActive(false);
			highlight2.SetActive(false);
			canEnter = false;
			choiceNum = 0;
			anim.SetBool("isOpen", true);
			// save a local copy of the dialogue we pass in
			dia = d;
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
		}
		
	}

	public void NextSentence()
	{
		canEnter = false;
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
	}
	// this is a coroutine that will take our chars from the string and print one at a time 
	IEnumerator TypeLetters(string s)
	{
		text.text = "";
		continueCountTotal = s.ToCharArray().Length;
		foreach (char l in s.ToCharArray())
		{
			text.text += l;
			yield return new WaitForSeconds(dia.typingSpeed);
			continueCount++;
			// if the string has stopped printing then you can continue
			if (continueCount == continueCountTotal)
			{
				continueButton.SetActive(true);
				canEnter = true;
				continueCountTotal = 0;
				continueCount = 0;
			}
			// diable the continue and show our choices
			if (boxCount == choiceCount)
			{
				continueButton.SetActive(false);
				canEnter = false;
				
				choice1.gameObject.SetActive(true);
				choice2.gameObject.SetActive(true);
				choice1.text = dia.choiceText1;
				choice2.text = dia.choiceText2;
			}
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
					choices[1] = true;
					break;
				case 2:
					kill = true;
					choices[2] = true;
					break;
				case 3:
					tell = true;
					choices[3] = true;
					break;
				default:
					Debug.LogError("You wanted a story choice but passed no choice set, fix it you idiot");
					break;
			}
			End();
			if (dia.choice1 != null && dia.choice1.dialogue != null)
			{
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
					choices[4] = true;
					break;
				case 2:
					spare = true;
					choices[5] = true;
					break;
				case 3:
					lie = true;
					choices[6] = true;
					break;
				default:
					Debug.LogError("You wanted a story choice but passed no choice set, fix it you idiot");
					break;
			}
			End();
			if  (dia.choice2 != null && dia.choice2.dialogue != null)
			{
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
		if (choice > choices.Length / 2 && (choices[(int)choice] == false && choices[(int)choice - choices.Length / 2] == false))
		{
			StartConversation(dia.normal.dialogue);
			Debug.Log((int)choice - choices.Length / 2);
		}
		// if it's less than half add half
		else if (choice <= choices.Length / 2 && (choices[(int)choice] == false && choices[(int)choice + choices.Length / 2] == false))
		{
			StartConversation(dia.normal.dialogue);
			Debug.Log((int)choice + choices.Length / 2);
		}
		// init dialogue 1
		else if (choices[(int)choice] == true)
		{
			StartConversation(dia.choiceCare1.dialogue);
		}
		// init dialogue 2
		else if (choices[(int)choice] == false)
		{
			StartConversation(dia.choiceCare2.dialogue);
		}
	}
	// check our keyboard pressed and do things accordingly
	public void CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.Return) && canEnter == true)
		{
			NextSentence();
		}
		if (boxCount == choiceCount && Input.GetKeyDown(KeyCode.LeftArrow))
		{
			choiceNum = 2;
		}
		else if (boxCount == choiceCount && Input.GetKeyDown(KeyCode.RightArrow))
		{
			choiceNum = 1;
		}
		if (Input.GetKeyDown(KeyCode.Return) && choiceNum == 1 && boxCount == choiceCount && dia.isChoice == true)
		{
			SelectSecondChoice();
		}
		else if (Input.GetKeyDown(KeyCode.Return) && choiceNum == 2 && boxCount == choiceCount && dia.isChoice == true)
		{
			SelectFirstChoice();
		}
		if (choiceNum == 1)
		{
			highlight1.SetActive(true);
			highlight2.SetActive(false);
		}
		else if (choiceNum == 2)
		{
			highlight1.SetActive(false);
			highlight2.SetActive(true);
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
	}
}
