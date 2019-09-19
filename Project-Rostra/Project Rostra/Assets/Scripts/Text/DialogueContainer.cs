using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueContainer : MonoBehaviour
{
	public static DialogueContainer instance;
	public Dialogue doneTalkingToNPCS;
	public Dialogue doneFight;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogError("The Dialogue Container is missing and sweet Jesus I can't find him");
		}
		DontDestroyOnLoad(this.gameObject);
	}
  
}
