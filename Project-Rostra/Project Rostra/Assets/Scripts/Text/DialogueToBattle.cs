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
			Debug.Log("Entered Dialogue");
			DialogueManager.instance.StartConversation(ct.dialogue);
		}
	}












//Feel the fire where she walks
//Lola Montez so beautiful
//Shady and a tempered dame
//Blinding your eyes with her spider dance
//Her performance utterly erotic subversive to all ideas
//And for public morality
//And cool as she was she didn't care
//See the miner throw his gold
//Lifting her skirt howling loud like a wolf
//Hell raising and full of sin
//When Lola was dancing and showing her skin
//Wherever she walks
//She will be captivating all the men
//Don't look in her eyes
//You might fall and find the love of your life heavenly
//But she'll catch you in her web
//The love of your life
//Well notorious I have been
//But never for fame that's what she said
//Dear Henry taste my whip
//Never to see any words you print
//Oh Lola I'm sure that the love would have been
//The key to all your pain
//No words will later come
//Did she spider bite your tongue
//We will surely not forget the Lola spider dance
}
