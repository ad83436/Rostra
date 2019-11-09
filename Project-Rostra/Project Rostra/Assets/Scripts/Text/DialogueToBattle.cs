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
	private bool battle;

    // Start is called before the first frame update
    void Start()
    {
        dm = DialogueManager.instance;
		ct = GetComponent<ConversationTrigger>();
		battle = false;
    }

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && DialogueManager.instance.GetChoice(ce) == true && battle == false)
		{
			Debug.Log("Entered Dialogue");
			DialogueManager.instance.StartConversation(ct.dialogue);
		}
	}

	private void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player") && DialogueManager.instance.GetChoice(ce) == true && DialogueManager.instance.isActive == false && battle == false)
		{
			Debug.Log("Transition To Battle");
			BattleManager.battleInProgress = true;
			fade.FlipFadeToBattle();
			battle = true;
			battle = true;
			Debug.Log("Transition To Battle");
			BattleManager.battleInProgress = true;
			fade.FlipFadeToBattle();
		}
	}
}
