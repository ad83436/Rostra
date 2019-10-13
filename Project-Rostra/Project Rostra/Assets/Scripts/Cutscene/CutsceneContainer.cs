using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneContainer : MonoBehaviour
{
	public static CutsceneContainer instance;
	// Start is called before the first frame update
	public Cutscene test;
    void Awake()
    {
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
    }

	public void StartCutscene(string name)
	{
		switch (name)
		{
			case "test":
				CutsceneManager.instance.StartCutscene(test);
				break;
			case null:
				Debug.LogError("Hey cock sucker you forgot to input the string god damn it");
				break;
		}
	}
}
