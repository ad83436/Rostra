using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour {
	//singleton
	public static ExpManager instance;

	#region Initialization & Destruction

	private void Awake() {
		//initialize singleton
		if (instance == null) instance = this;
		else Debug.LogError("There is more than one instance of the ExpManager or the instance variable was not cleared on destroy");
	}

	private void OnDestroy() {
		//remove singleton
		if (instance == this) instance = null;
	}

	#endregion

	#region exp & leveling

	public void LevelUp(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}

		//level up
		PartyStats.chara[playerIndex].level++;
		//set current exp to 0
		PartyStats.chara[playerIndex].currentExperience = 0;

		//calls the corisponding player function to update the gained abilities
		switch (playerIndex) {
			case 0: CharaOneLevelUp(PartyStats.chara[playerIndex].level); break;
			case 1: CharaTwoLevelUp(PartyStats.chara[playerIndex].level); break;
			case 2: CharaThreeLevelUp(PartyStats.chara[playerIndex].level); break;
			case 3: CharaFourLevelUp(PartyStats.chara[playerIndex].level); break;
			default: Debug.LogError("If your reading this its too late (tell Domara)"); break;
		}

		//calculate the number of stat points gained
		///number of points increases by 1 each level
		///eg: level 30, points gained = 7;
		PartyStats.chara[playerIndex].statPoints += 1 + PartyStats.chara[playerIndex].level / 5; // WIP

		//changes the Exp needed to level up again
		PartyStats.chara[playerIndex].neededExperience = 500 + 250 * (PartyStats.chara[playerIndex].level - 1);
		// WIP
		///500 is the base exp needed
		///250 * (level - 1) adds 250 for each level gained
		///eg: level 30, exp needed is 7,750
	}

	public int ExpNeeded(int level) {
		//a calculation of how much exp is needed
		return 500 + 250 * (level - 1);
		// WIP
		///500 is the base exp needed
		///250 * (level - 1) adds 250 for each level gained
		///eg: level 30, exp needed is 7,750
	}

	/* TODO: Add functionality */
	#region functions that give each player their skills

	private void CharaOneLevelUp(int level) {
		switch (level) {
			default: /*Debug.LogError("Current level is not valid"); /**/ break;
		}
	}

	private void CharaTwoLevelUp(int level) {
		switch (level) {
			default: /*Debug.LogError("Current level is not valid"); /**/ break;
		}
	}

	private void CharaThreeLevelUp(int level) {
		switch (level) {
			default: /*Debug.LogError("Current level is not valid"); /**/ break;
		}
	}

	private void CharaFourLevelUp(int level) {
		switch (level) {
			default: /*Debug.LogError("Current level is not valid"); /**/ break;
		}
	}

	#endregion

	#endregion

	#region Using upgrade points

	public void UsePointOnAttack(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].attack++;
	}

	public void UsePointOnDefence(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].defence += 1f;
	}

	public void UsePointOnHealth(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].maxHealth += 1f;
		PartyStats.chara[playerIndex].hitpoints += 1f;
	}

	public void UsePointOnMana(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].maxMana += 1f;
		PartyStats.chara[playerIndex].magicpoints += 1f;
	}

	public void UsePointOnStrength(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].strength += 1f;
	}

	public void UsePointOnCritical(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].critical += 1f;
	}

	public void UsePointOnAgility(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].agility += 1f;
	}

	#endregion

	#region GUI

	private bool showGUI = false;
	private bool use = false;
	private bool up = false;
	private bool down = false;
	private int currentItem = 0;
	private int currentPlayer = 0;

	private void OnGUI() {
		if (!showGUI) {
			use = false;
			return;
		}

		if (up) currentItem--;
		else if (down) currentItem++;
		if (currentItem == -1) currentItem = 9;
		if (currentItem == 10) currentItem = 0;
		up = false;
		down = false;

		GUI.skin.label.fontSize = 40;

		Rect rect = new Rect(40, 50, 400, 80);

		GUI.Label(rect, "   skillPoints " + PartyStats.chara[currentPlayer].statPoints);

		rect.y += 80;
		if (currentItem == 0) GUI.Label(rect, "> Current Player " + currentPlayer);
		else GUI.Label(rect, "   Current Player " + currentPlayer);

		rect.y += 80;
		if (currentItem == 1) GUI.Label(rect, "> Level " + PartyStats.chara[currentPlayer].level);
		else GUI.Label(rect, "   Level " + PartyStats.chara[currentPlayer].level);

		rect.y += 80;
		if (currentItem == 2) GUI.Label(rect, "> Attack " + PartyStats.chara[currentPlayer].attack);
		else GUI.Label(rect, "   Attack " + PartyStats.chara[currentPlayer].attack);

		rect.y += 80;
		if (currentItem == 3) GUI.Label(rect, "> Defence " + PartyStats.chara[currentPlayer].defence);
		else GUI.Label(rect, "   Defence " + PartyStats.chara[currentPlayer].defence);

		rect.y += 80;
		if (currentItem == 4) GUI.Label(rect, "> Health " + PartyStats.chara[currentPlayer].hitpoints);
		else GUI.Label(rect, "   Health " + PartyStats.chara[currentPlayer].hitpoints);

		rect.y += 80;
		if (currentItem == 5) GUI.Label(rect, "> Mana " + PartyStats.chara[currentPlayer].magicpoints);
		else GUI.Label(rect, "   Mana " + PartyStats.chara[currentPlayer].magicpoints);

		rect.y += 80;
		if (currentItem == 6) GUI.Label(rect, "> Strength " + PartyStats.chara[currentPlayer].strength);
		else GUI.Label(rect, "   Strength " + PartyStats.chara[currentPlayer].strength);

		rect.y += 80;
		if (currentItem == 7) GUI.Label(rect, "> Critical " + PartyStats.chara[currentPlayer].critical);
		else GUI.Label(rect, "   Critical " + PartyStats.chara[currentPlayer].critical);

		rect.y += 80;
		if (currentItem == 8) GUI.Label(rect, "> Agility " + PartyStats.chara[currentPlayer].agility);
		else GUI.Label(rect, "   Agility " + PartyStats.chara[currentPlayer].agility);

		if (use) {
			switch (currentItem) {
				case 0:
					currentPlayer++;
					if (currentPlayer == 4) currentPlayer = 0;
					break;
				case 1: LevelUp(currentPlayer);				break;
				case 2: UsePointOnAttack(currentPlayer);	break;
				case 3: UsePointOnDefence(currentPlayer);	break;
				case 4: UsePointOnHealth(currentPlayer);	break;
				case 5: UsePointOnMana(currentPlayer);		break;
				case 6: UsePointOnStrength(currentPlayer);	break;
				case 7: UsePointOnCritical(currentPlayer);	break;
				case 8: UsePointOnAgility(currentPlayer);	break;
				default: break;
			}
		}
		use = false;
	}

	private void Update() {
		if (Input.GetButtonDown("expOpen")) showGUI = showGUI ? false : true;
		if (showGUI) use = Input.GetButtonDown("expConfirm");
		up = Input.GetButtonDown("expUp");
		down = Input.GetButtonDown("expDown");
	}

	#endregion
}
