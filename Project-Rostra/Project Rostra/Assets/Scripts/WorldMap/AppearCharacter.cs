using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearCharacter : MonoBehaviour
{
	public ChoiceEnum ce;
	void Update()
	{
		if (DialogueManager.instance.GetChoice(ce) == true)
		{
			this.gameObject.SetActive(true);
		}
	}
}
