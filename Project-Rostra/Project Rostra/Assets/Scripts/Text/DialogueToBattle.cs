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
    void Start()
    {
        dm = DialogueManager.instance;
		ct = GetComponent<ConversationTrigger>();
    }

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && DialogueManager.instance.GetChoice(ce) == true)
		{
			
			DialogueManager.instance.StartConversation(ct.dialogue);
            gameObject.SetActive(false); //Disable the trigger, it no longer needs to run now
		}
	}
}
