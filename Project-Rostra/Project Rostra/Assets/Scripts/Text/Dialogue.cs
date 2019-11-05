// written by: Sean Fowke
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
[System.Serializable]
public class Dialogue
{
	// this will be the string that we load into the name section of the UI it is an array in case we have
	// have multiple people talking
	public string[] names;
	// this just makes typing in the inspector so much easier as it makes the text field bigger
	[TextArea(3, 15)]
	// An array of strings that hold the actual dialogue
	public string[] sentences;
	// this sprite will determine which sprite will serve as the portrait
	public Sprite[] portrait;
	// which line will the pov change occur on
	public float[] povChange;
	// the speed of the text scroll
	public float typingSpeed;
	//does this dialogue contain a choice
	public bool isChoice;
	// if so what line will it appear on?
	public float choiceLine;
	// contains the triggers for choices one and two
	public ConversationTrigger choice1;
	public ConversationTrigger choice2;
	// will the choice be remembered;
	public bool willRemember;
	// which set of bools will be selected?
	public float choiceSet;
	// the two dialogues that will be spit out depending on your choices
	public ConversationTrigger choiceCare1;
	public ConversationTrigger choiceCare2;
	// contains the text that will be passed to the UI
	[TextArea(3, 15)]
	public string choiceText1;
	[TextArea(3, 15)]
	public string choiceText2;
	// dialogue triggered if both choices are false
	public ConversationTrigger normal;
	// don't change this
	public bool hasPlayed;
	//
	public bool willCount;
	public float maxWillCount;
	//
	public bool isBattle;
	//// will fix later
	//public bool willPlayOnce;
	//public bool narratorText;
	public int triggerBool;
	//
	public bool isOneShot;
	public int addMilestone;
}
