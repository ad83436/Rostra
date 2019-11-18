// written: by Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueAfterBattle : MonoBehaviour
{
	public Dialogue d;
	public ChoiceEnum ce;
	public Fade fade;
	public bool played = false;

	private void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("Player") && DialogueManager.instance.GetChoice(ce) == true && played == false && SceneManager.GetActiveScene().name != "Queue Scene" && DialogueManager.instance.isActive == false && BattleManager.battleInProgress == false)
		{
			Debug.Log("Triggered Choice Dialogue");
			DialogueManager.instance.StartConversation(d);
			played = true;
			//col.gameObject.transform.position = new Vector2(col.gameObject.transform.position.x + 0.01f, col.gameObject.transform.position.y);
		}
	}
}
