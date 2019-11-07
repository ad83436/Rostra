using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatTutorial : MonoBehaviour {
	// singleton
	public static BatTutorial Singleton { get; private set; }

	// ui
	[SerializeField]
	private Text text_battle;
	[SerializeField]
	private GameObject panel_battle;

	// list of events
	[SerializeField]
	[Multiline]
	private List<string> events;
	// current event
	private int curevent;

	private void Awake() {
		// singleton
		if (Singleton == null) {
			Singleton = this;
		} else {
			Debug.LogError("BatTutorial Singleton was not null");
		}

		// init
		ResetTutorial();
	}

	private void OnDestroy() {

		if (Singleton == this) {
			Singleton = null;
		}
	}

	// progressing the tutorial
	public void ResetTutorial() {
		curevent = -1;
		text_battle.text = "";
		panel_battle.SetActive(false);
	}

	public void NextEvent() {
		curevent++;
		if (curevent < events.Count) {
			// change text
			text_battle.text = events[curevent];
			// show panel
			panel_battle.SetActive(true);
		}
	}

	public void Hide() {
		panel_battle.SetActive(false);
	}

}

