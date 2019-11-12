using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisapearCharacterAfterBool : MonoBehaviour
{
	public ChoiceEnum ce;
    void Update()
    {
		if (DialogueManager.instance.GetChoice(ce) == true)
		{
			this.gameObject.SetActive(false);
		}
    }
}
