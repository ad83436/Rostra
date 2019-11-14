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









//Palpatine: Did you ever hear the tragedy of Darth Plagueis The Wise?

//Anakin: No?

//Palpatine: I thought not.It's not a story the Jedi would tell you. It's a Sith legend.Darth Plagueis was a Dark Lord of the Sith, so powerful and so wise he could use the Force to influence the midichlorians to create life… He had such a knowledge of the dark side, he could even keep the ones he cared about from dying.

//Anakin: He could actually save people from death?

//Palpatine: The dark side of the Force is a pathway to many abilities some consider to be unnatural.

//Anakin: What happened to him?

//Palpatine: He became so powerful… the only thing he was afraid of was losing his power, which eventually, of course, he did. Unfortunately, 
//he taught his apprentice everything he knew, then his apprentice killed him in his sleep. Ironic. He could save others from death, but not himself.

//Anakin: Is it possible to learn this power?

//Palpatine: Not from a Jedi.
}
