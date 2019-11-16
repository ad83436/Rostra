using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDemo : MonoBehaviour
{
	public ChoiceEnum ce;
	public Fade fade;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (DialogueManager.instance.isActive == false && DialogueManager.instance.GetChoice(ce) == true)
		{
			fade.FlipToEndTest();
		}
	}
}
