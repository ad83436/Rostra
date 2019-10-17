#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct USkillItem {
	public int SkillID;
	public string name;
	public string description;
}

public class SkillsMenuController : SubMenu {

	//property to skillinventory
	private ref SkillsInventory skinv => ref SkillsInventory.invInstance;

	// UI Components
	public CanvasGroup EquipGroup;
	public CanvasGroup SkillGroup;
	public UnityEngine.UI.Text descriptionText;
	public UnityEngine.UI.Text[] equippedSkillsText;
	public UnityEngine.UI.Image[] equippedSkillsHighlightBox;
	public UnityEngine.UI.Text[] unSkillText;
	public UnityEngine.UI.Image[] unSkillBox;
	public UnityEngine.UI.Text[] loSkillText;
	public UnityEngine.UI.Image[] loSkillBox;

	// UI related
	public Color HighlightColor;
	public Color UnhighlightColor;

	//constants
	const int SKILLS_PER_PAGE = 6;

	//Lists
	private List<USkillItem> UnlockedSkillsList;
	private List<USkillItem> LockedSkillsList;

	//indices
	private int skillIndexA = 0;
	private int topOfListA = 0;
	private int RelIndexA {
		get => skillIndexA - topOfListA;
		set => skillIndexA = topOfListA + value;
	}

	private int skillIndexB = 0;
	private int topOfListB = 0;
	private int RelIndexB {
		get => skillIndexB - topOfListB;
		set => skillIndexB = topOfListB + value;
	}

	int playerIndex = 0;
	int equipIndex = 0;

	//// State Related
	// state
	private int state = 0;
	/// 0 = Equipment selection
	/// 1 = Unlocked skill list
	/// 2 = Locked skill list

	#region Initialization

	protected override void Awake() {
		base.Awake();
		EquipGroup.alpha = 1f;
		SkillGroup.alpha = 0f;
	}

	#endregion

	#region List Related Funtions

	private void FillLists(int playerindex) {
		//fill unlocked skills
		UnlockedSkillsList = new List<USkillItem>();
		for (int i = 0; i < PartySkills.skills[playerindex].learnedSkills.Length; i++) {
			if (PartySkills.skills[playerindex].learnedSkills[i] != (int)SKILLS.NO_SKILL)
				UnlockedSkillsList.Add(Generate(PartySkills.skills[playerindex].learnedSkills[i], false));
		}

		//fill locked skills
		LockedSkillsList = new List<USkillItem>();
		for (int i = 0; i < PartySkills.skills[playerindex].unlockableSkills.Length; i++) {
			if (PartySkills.skills[playerindex].unlockableSkills[i] != (int)SKILLS.NO_SKILL)
				LockedSkillsList.Add(Generate(PartySkills.skills[playerindex].unlockableSkills[i], true));
		}
	}

	private void ClearLists() {
		UnlockedSkillsList.Clear();
		LockedSkillsList.Clear();
	}

	private USkillItem Generate(int skillID, bool isLocked) {
		USkillItem skill = new USkillItem();

		skill.SkillID = skillID;
		skill.name = skinv.SkillName(skillID);
		if (!isLocked) skill.description = skinv.SkillDescription(skillID);
		else {
			string desc = skinv.SkillDescription(skillID);
			desc += "\n\n";

			int[] minimum = ExpManager.RequiredStats((SKILLS)skillID, playerIndex);

			if (minimum[0] > PartyStats.chara[playerIndex].attack) desc += "ATK: " + minimum[0] + "\n";
			if (minimum[1] > PartyStats.chara[playerIndex].defence) desc += "DEF: " + minimum[1] + "\n";
			if (minimum[2] > PartyStats.chara[playerIndex].maxHealth) desc += "HP:  " + minimum[2] + "\n";
			if (minimum[3] > PartyStats.chara[playerIndex].maxMana) desc += "MP:  " + minimum[3] + "\n";
			if (minimum[4] > PartyStats.chara[playerIndex].strength) desc += "STR: " + minimum[4] + "\n";
			if (minimum[5] > PartyStats.chara[playerIndex].agility) desc += "AGI: " + minimum[5] + "\n";
			if (minimum[6] > PartyStats.chara[playerIndex].speed) desc += "SPD: " + minimum[6] + "\n";

			skill.description = desc;
		}

		return skill;
	}

	#endregion

	#region UI Functions

	private void UpdateSelectedCharaUI(int character) {
		for (int i = 0; i < 4; i++) {
			equippedSkillsText[character * 4 + i].text = skinv.SkillName(PartySkills.skills[character].equippedSkills[i]);
		}
	}

	private void SetEquippedColor(int player, int setSkill, Color color) {
		equippedSkillsHighlightBox[player * 4 + setSkill].color = color;
	}

	private void UpdateEquipUI() {
		for (int player = 0; player < 4; player++) {
			for (int i = 0; i < 4; i++) {
				equippedSkillsText[player * 4 + i].text =
					skinv.SkillName(PartySkills.skills[player].equippedSkills[i]);
			}
		}
	}

	private void SetUnlColor(int relindex, Color color) {
		unSkillBox[relindex].color = color;
	}

	private void SetLoColor(int relindex, Color color) {
		loSkillBox[relindex].color = color;
	}

	private void UpdateSkillUI() {
		for (int i = 0; i < SKILLS_PER_PAGE; i++) {
			if (i < UnlockedSkillsList.Count) unSkillText[i].text = UnlockedSkillsList[i + topOfListA].name;
			else unSkillText[i].text = "";
			if (i < LockedSkillsList.Count) loSkillText[i].text = LockedSkillsList[i + topOfListB].name;
			else loSkillText[i].text = "";
		}
	}

	private void UpdateUnlockedUI() {
		for (int i = 0; i < SKILLS_PER_PAGE; i++) {
			if (i < UnlockedSkillsList.Count) unSkillText[i].text = UnlockedSkillsList[i + topOfListA].name;
			else unSkillText[i].text = "";
		}
	}

	private void UpdateLockedUI() {
		for (int i = 0; i < SKILLS_PER_PAGE; i++) {
			if (i < LockedSkillsList.Count) loSkillText[i].text = LockedSkillsList[i + topOfListB].name;
			else loSkillText[i].text = "";
		}
	}

	private void SetDescription(int relIndex, bool isunlockedlist) {
		if (isunlockedlist && UnlockedSkillsList.Count > 0)
			descriptionText.text = UnlockedSkillsList[relIndex].description;
		else if (LockedSkillsList.Count > 0)
			descriptionText.text = LockedSkillsList[relIndex].description;
		else
			ClearDescription();
	}

	private void ClearDescription() {
		descriptionText.text = "---";
	}

	#endregion

	#region Logic Functions

	private void MoveUpEquipped() {
		if (equipIndex == 0 && (playerIndex == 0 || playerIndex == 1)) return;
		if (equipIndex == 0) {
			if (playerIndex == 2) playerIndex = 0;
			if (playerIndex == 3) playerIndex = 1;
			equipIndex = 3;
		} else { --equipIndex; }
	}

	private void MoveDownEquipped() {
		if (equipIndex == 3 && (playerIndex == 2 || playerIndex == 3)) return;
		if (equipIndex == 3) {
			if (playerIndex == 0) playerIndex = 2;
			if (playerIndex == 1) playerIndex = 3;
			equipIndex = 0;
		} else { ++equipIndex; }
	}

	private void MoveLeftEquipped() {
		if (playerIndex == 0 || playerIndex == 2) return;
		if (playerIndex == 1) playerIndex = 0;
		if (playerIndex == 3) playerIndex = 2;
	}

	private void MoveRightEquipped() {
		if (playerIndex == 1 || playerIndex == 3) return;
		if (playerIndex == 0) playerIndex = 1;
		if (playerIndex == 2) playerIndex = 3;
	}

	private void MoveUpUnlocked() {
		if (skillIndexA == 0) return;
		if (RelIndexA == 0) --topOfListA;
		--skillIndexA;
	}

	private void MoveDownUnlocked() {
		if (skillIndexA == UnlockedSkillsList.Count - 1 || skillIndexA >= UnlockedSkillsList.Count) return;
		if (RelIndexA == SKILLS_PER_PAGE - 1) ++topOfListA;
		++skillIndexA;
	}

	private void MoveUpLocked() {
		if (skillIndexB == 0) return;
		if (RelIndexB == 0) --topOfListB;
		--skillIndexB;
	}

	private void MoveDownLocked() {
		if (skillIndexB == LockedSkillsList.Count - 1 || skillIndexB >= LockedSkillsList.Count) return;
		if (RelIndexB == SKILLS_PER_PAGE - 1) ++topOfListB;
		++skillIndexB;
	}

	#endregion

	#region Submenu Functions

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public override void MenuUpdate() {
		switch (state) {
			case 0: { /// equip menu
				if (Cancel) {
					SetEquippedColor(playerIndex, equipIndex, UnhighlightColor);
					ExitMenu();
					break;
				}

				SetEquippedColor(playerIndex, equipIndex, UnhighlightColor);
				if (Up) MoveUpEquipped();
				else if (Down) MoveDownEquipped();
				if (Left) MoveLeftEquipped();
				else if (Right) MoveRightEquipped();
				SetEquippedColor(playerIndex, equipIndex, HighlightColor);

				if (Confirm) {
					state = 1;
					FillLists(playerIndex);
					UpdateSkillUI();
					SkillGroup.alpha = 1f;
					SetDescription(0, true);
					break;
				}
				break;
			} /// equip menu
			case 1: { /// Unlocked skill list
				if (Cancel) {
					SetUnlColor(RelIndexA, UnhighlightColor);
					state = 0;
					SkillGroup.alpha = 0f;
					topOfListA = 0;
					skillIndexA = 0;
					topOfListB = 0;
					skillIndexB = 0;
					break;
				}
				if (Right) {
					SetUnlColor(RelIndexA, UnhighlightColor);
					if (RelIndexA < LockedSkillsList.Count - topOfListB) RelIndexB = RelIndexA;
					else RelIndexB = LockedSkillsList.Count - topOfListB - 1;
					if (RelIndexB < 0) RelIndexB = 0;
					state = 2;
					SetDescription(skillIndexB, false);
					break;
				}

				SetUnlColor(RelIndexA, UnhighlightColor);
				if (Up) MoveUpUnlocked();
				if (Down) MoveDownUnlocked();
				SetUnlColor(RelIndexA, HighlightColor);

				if (Up || Down) {
					UpdateUnlockedUI();
					SetDescription(skillIndexA, true);
				}
				if (UnlockedSkillsList.Count == 0) ClearDescription();

				if (Confirm && UnlockedSkillsList.Count > skillIndexA) {
					SetUnlColor(RelIndexA, UnhighlightColor);
					state = 0;
					SkillGroup.alpha = 0f;
					print(skillIndexA);
					skinv.EquipSkill(UnlockedSkillsList[skillIndexA].SkillID, equipIndex, playerIndex);
					UpdateSelectedCharaUI(playerIndex);
					ClearLists();
					topOfListA = 0;
					skillIndexA = 0;
					topOfListB = 0;
					skillIndexB = 0;
					break;
				}
				break;
			} /// Unlocked skill list
			case 2: { /// Locked skill list
				if (Cancel) {
					SetLoColor(RelIndexB, UnhighlightColor);
					state = 0;
					SkillGroup.alpha = 0f;
					topOfListA = 0;
					skillIndexA = 0;
					topOfListB = 0;
					skillIndexB = 0;
					break;
				}
				if (Left) {
					SetLoColor(RelIndexB, UnhighlightColor);
					if (RelIndexB < UnlockedSkillsList.Count - topOfListA) RelIndexA = RelIndexB;
					else RelIndexA = UnlockedSkillsList.Count - topOfListA - 1;
					if (RelIndexA < 0) RelIndexA = 0;
					state = 1;
					SetDescription(skillIndexA, true);
					break;
				}

				SetLoColor(RelIndexB, UnhighlightColor);
				if (Up) MoveUpLocked();
				if (Down) MoveDownLocked();
				SetLoColor(RelIndexB, HighlightColor);

				if (Up || Down) {
					UpdateLockedUI();
					SetDescription(skillIndexB, false);
				}
				if (LockedSkillsList.Count == 0) ClearDescription();
				break;
			} /// Locked skill list
			default: break;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	#region Events

	public override void OnVisible() {
		UpdateEquipUI();
	}

	public override void OnInvisible() {

	}

	public override void OnActive() {
		ClearDescription();
	}

	public override void OnInactive() {

	}

	#endregion

	#endregion

}
