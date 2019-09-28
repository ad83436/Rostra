// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
	public Cutscene cs;
	// Start is called before the first frame update
	public void TriggerCutscene()
	{
		CutsceneManager.instance.StartCutscene(cs);
	}
}
