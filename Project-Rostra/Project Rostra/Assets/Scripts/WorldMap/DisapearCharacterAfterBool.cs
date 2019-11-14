// written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisapearCharacterAfterBool : MonoBehaviour
{
	public ChoiceEnum ce;
	public bool appear;
    void Update()
    {
		if (DialogueManager.instance.GetChoice(ce) == true && appear == false)
		{
			this.gameObject.SetActive(false);
		}
		else if (DialogueManager.instance.GetChoice(ce) == true && appear == true)
		{
			this.gameObject.SetActive(true);
		}
		
    }
}
