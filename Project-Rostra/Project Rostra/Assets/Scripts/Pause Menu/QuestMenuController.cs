using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMenuController : SubMenu {

	// array of map locations

	// text components
	public UnityEngine.UI.Text Test_QuestName;
	public UnityEngine.UI.Text Test_QuestMile;
	public UnityEngine.UI.Text Test_QuestDesc;

    public GameObject Location_BrennasHouse;
    public GameObject Location_Hadria;
    public GameObject Location_MyHouse;

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	private void Awake() {
		canOpen = false; // prevent the menu from opening
		QuestManager.AddMilestone(1);
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	private void UpdateCurrentQuest() {
		Test_QuestName.text = QuestManager.questName;
		Test_QuestMile.text = QuestManager.milestoneName;
		Test_QuestDesc.text = QuestManager.description;
        // set the locations
        Location_Hadria.SetActive(QuestManager.visitedLocals["Town"]);
        Location_BrennasHouse.SetActive(QuestManager.visitedLocals["Brenna's House"]);
        Location_MyHouse.SetActive(QuestManager.visitedLocals["Domel's house"]);
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public override void MenuUpdate() {
		if (Cancel) {
			ExitMenu();
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public override void OnActive() { }
	public override void OnInactive() { }
	public override void OnInvisible() { }
	public override void OnVisible() {
		UpdateCurrentQuest();
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
