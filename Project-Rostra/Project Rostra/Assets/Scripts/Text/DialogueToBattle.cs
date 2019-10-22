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
	public Collider2D battle;
	public Collider2D endDemo;
    // Start is called before the first frame update
    void Start()
    {
        dm = DialogueManager.instance;
		ct = GetComponent<ConversationTrigger>();
		endDemo.enabled = false;
    }
	
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && dm.demo == true && battle.enabled == true)
		{
			Debug.Log("Transition To Battle");
            BattleManager.battleInProgress = true;
			fade.FlipFadeToBattle();
			battle.enabled = false;
			endDemo.enabled = true;
		}
		else if (col.CompareTag("Player") && dm.demo == true && endDemo.enabled == true)
		{
			fade.FlipToEndTest();
		}
	}
}
