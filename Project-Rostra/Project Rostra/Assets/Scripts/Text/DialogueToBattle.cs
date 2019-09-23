// written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueToBattle : MonoBehaviour
{
	DialogueManager dm;
	ConversationTrigger ct;
	public ChoiceEnum ce;
    // Start is called before the first frame update
    void Awake()
    {
		dm = DialogueManager.instance;
		ct = GetComponent<ConversationTrigger>();
    }
	
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && dm.GetChoice(ce) == true)
		{
			TriggerEvent();
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
