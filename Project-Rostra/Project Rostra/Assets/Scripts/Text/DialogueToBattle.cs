// written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueToBattle : MonoBehaviour
{
	DialogueManager dm;
	ConversationTrigger ct;
	public ChoiceEnum ce;
	public Fade fade;
    // Start is called before the first frame update
    void Awake()
    {
		dm = FindObjectOfType<DialogueManager>();
		ct = GetComponent<ConversationTrigger>();
    }
	
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && dm.demo == true)
		{
			Debug.Log("Transition To Battle");
			fade.FlipFadeToBattle();
		}
	}

	public void TriggerEvent()
	{
		if (dm.GetChoice(ce) == true)
		{
			dm.SetChoice(ce, false);
			dm.StartConversation(ct.dialogue);
		}
	}
}
