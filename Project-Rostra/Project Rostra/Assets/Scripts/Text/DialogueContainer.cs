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
			Destroy(gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}
}
