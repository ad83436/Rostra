using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// written by: Sean Fowke
public class ConversationTrigger : MonoBehaviour
{
	//store a dialogue that we can change in the inspector
	public Dialogue dialogue;
	// is this text dependant on a choice made previously
	public float choiceCare;
	// these two dialogues will be spit out depending on the choice that was made. 
	// call this method in order to trigger the conversation
	public bool isChoiceDepend;
	// does the conversation only trigger once
	public bool isOneShot;
	public void TriggerConvo()
	{
		if (isOneShot == true)
		{
			DialogueManager.instance.StartConversation(dialogue);
			isOneShot = false;
		}
		else
		{
			//FindObjectOfType<DialogueManager>().StartConversation(dialogue);
		}
	}
	// 1 = dwarf, 2 = guild, 3 = kill, 4 = spare, 5 = tell, 6 = lie
	public void TriggerChoiceDependantConvo()
	{
		DialogueManager.instance.ChoiceDependantConvo(choiceCare, dialogue);
	}

	public void TriggerNormalDialogue()
	{
		DialogueManager.instance.PlayNormalDialogue(dialogue);
	}
}
