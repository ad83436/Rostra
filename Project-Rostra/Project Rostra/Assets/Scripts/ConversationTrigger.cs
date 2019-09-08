using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger : MonoBehaviour
{
	//store a dialogue that we can change in the inspector
	public Dialogue dialogue;
	// call this method in order to trigger the conversation
	public void TriggerConvo()
	{
		FindObjectOfType<DialogueManager>().StartConversation(dialogue);
	}
}
