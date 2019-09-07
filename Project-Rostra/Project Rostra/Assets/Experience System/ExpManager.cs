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

		switch (playerIndex) {
			case 0: PlayerOneLevelUp(PartyStats.chara[playerIndex].level);   break;
			case 1: PlayerTwoLevelUp(PartyStats.chara[playerIndex].level);   break;
			case 2: PlayerThreeLevelUp(PartyStats.chara[playerIndex].level); break;
			case 3: PlayerFourLevelUp(PartyStats.chara[playerIndex].level);  break;
			default: Debug.LogError("If your reading this its too late (tell Domara)"); break;
		}
		
		//calculate the number of skill points gained
		///number of points increases by 1 each level
		///eg: level 30, points gained = 7;
		PartyStats.chara[playerIndex].skillPoints += 1 + PartyStats.chara[playerIndex].level / 5; // WIP

		//changes the Exp needed to level up again
		PartyStats.chara[playerIndex].neededExperience = ExpNeeded(PartyStats.chara[playerIndex].level);
	}

	private int ExpNeeded(int level) {
		//a calculation of how much exp is needed
		return 500 + 250 * (level - 1); // WIP
		///500 is the base exp needed
		///250 * (level - 1) adds 250 for each level gained
		///eg: level 30, exp needed is 7,750
	}
	
	/* TODO: Add functionality */
	#region functions that give each player their abilities, upgrade points, etc

	private void PlayerOneLevelUp(int level) {
		switch (level) {
			default:
				Debug.LogError("Current level is not valid");
				break;
		}
	}

	private void PlayerTwoLevelUp(int level) {
		switch (level) {
			default:
				Debug.LogError("Current level is not valid");
				break;
		}
	}

	private void PlayerThreeLevelUp(int level) {
		switch (level) {
			default:
				Debug.LogError("Current level is not valid");
				break;
		}
	}

	private void PlayerFourLevelUp(int level) {
		switch (level) {
			default:
				Debug.LogError("Current level is not valid");
				break;
		}
	}

	#endregion

	#endregion
}
