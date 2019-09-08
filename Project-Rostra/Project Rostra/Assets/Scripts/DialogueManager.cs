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
		// set everything to its default 
		textElements = new Queue<string>();
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
	}

	public void StartConversation(Dialogue d)
	{
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
		// disable the choice markers
		choice1.gameObject.SetActive(false);
		choice2.gameObject.SetActive(false);
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
	public void selectFirstChoice()
	{
		End();
		StartConversation(dia.choice1.dialogue);
	}
	// or door two
	public void selectSecondChoice()
	{
		End();
		StartConversation(dia.choice2.dialogue);
	}
	// check our keyboard pressed and do things accordingly
	public void checkInput()
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
		if (Input.GetKeyDown(KeyCode.Return) && choiceNum == 1 && boxCount == choiceCount)
		{
			selectSecondChoice();
		}
		else if (Input.GetKeyDown(KeyCode.Return) && choiceNum == 2 && boxCount == choiceCount)
		{
			selectFirstChoice();
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
		checkInput();
	}
}
