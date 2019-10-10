using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649

public class PartyMenuController : SubMenu {

	#region Meters

	[SerializeField] private MeterController chara0Health;
	[SerializeField] private MeterController chara0Mana;
	[SerializeField] private MeterController chara1Health;
	[SerializeField] private MeterController chara1Mana;
	[SerializeField] private MeterController chara2Health;
	[SerializeField] private MeterController chara2Mana;
	[SerializeField] private MeterController chara3Health;
	[SerializeField] private MeterController chara3Mana;

	private void UpdateMeters() {
		chara0Health.SetMeterLevel(PartyStats.CharaOne.hitpoints / PartyStats.CharaOne.TotalMaxHealth);
		chara0Mana.SetMeterLevel(PartyStats.CharaOne.magicpoints / PartyStats.CharaOne.TotalMaxMana);
		chara1Health.SetMeterLevel(PartyStats.CharaTwo.hitpoints / PartyStats.CharaTwo.TotalMaxHealth);
		chara1Mana.SetMeterLevel(PartyStats.CharaTwo.magicpoints / PartyStats.CharaTwo.TotalMaxMana);
		chara2Health.SetMeterLevel(PartyStats.CharaThree.hitpoints / PartyStats.CharaThree.TotalMaxHealth);
		chara2Mana.SetMeterLevel(PartyStats.CharaThree.magicpoints / PartyStats.CharaThree.TotalMaxMana);
		chara3Health.SetMeterLevel(PartyStats.CharaFour.hitpoints / PartyStats.CharaFour.TotalMaxHealth);
		chara3Mana.SetMeterLevel(PartyStats.CharaFour.magicpoints / PartyStats.CharaFour.TotalMaxMana);
	}

	#endregion

	#region Stats

	[SerializeField] private UnityEngine.UI.Text attackStatText;
	[SerializeField] private UnityEngine.UI.Text defenceStatText;
	[SerializeField] private UnityEngine.UI.Text maxHealthStatText;
	[SerializeField] private UnityEngine.UI.Text maxManaStatText;
	[SerializeField] private UnityEngine.UI.Text strengthStatText;
	[SerializeField] private UnityEngine.UI.Text agilityStatText;
	[SerializeField] private UnityEngine.UI.Text speedStatText;
	[SerializeField] private UnityEngine.UI.Text CriticalStatText;

	private int playerIndex = 0;

	private void UpdateStats() {
		attackStatText.text = "" + PartyStats.chara[playerIndex].TotalAttack;
		defenceStatText.text = "" + PartyStats.chara[playerIndex].TotalDefence;
		maxHealthStatText.text = "" + PartyStats.chara[playerIndex].TotalMaxHealth;
		maxManaStatText.text = "" + PartyStats.chara[playerIndex].TotalMaxMana;
		strengthStatText.text = "" + PartyStats.chara[playerIndex].TotalStrength;
		agilityStatText.text = "" + PartyStats.chara[playerIndex].TotalAgility;
		speedStatText.text = "" + PartyStats.chara[playerIndex].TotalSpeed;
		CriticalStatText.text = "" + PartyStats.chara[playerIndex].TotalCritical;
	}

	#endregion

	#region Other Text

	[SerializeField] private UnityEngine.UI.Text[] charanames;
	[SerializeField] private UnityEngine.UI.Text statsName;

	public ItemMenu statsMenu;

	#endregion

	#region Character Images

	[SerializeField] private UnityEngine.UI.Image[] charaImages;

	#endregion
	
	public override void OnVisible() { }

	protected override void Awake() {
		base.Awake();
		statsMenu.pressedDelegate = IncreaseStat;
		statsMenu.exitDelegate = CloseStatsMenu;
	}

	private bool IsInStatsMenu {
		get => statsGroup.alpha > 0.5f && partyGroup.alpha < 0.5f;
		set {
			statsGroup.alpha = value ? 1f : 0f;
			partyGroup.alpha = value ? 0f : 1f;
		}
	}

	[SerializeField] private CanvasGroup partyGroup;
	[SerializeField] private CanvasGroup statsGroup;

	public override void MenuUpdate() {
		if (!IsInStatsMenu) {
			if (Cancel) {
				ExitMenu();
				return;
			}
			switch (playerIndex) {
				case 0:
					if (Confirm) { OpenStatsMenu(); break; }
					charanames[playerIndex].color = Color.white;
					if (Down) playerIndex = 2;
					else if (Right) playerIndex = 1;
					charanames[playerIndex].color = Color.yellow;
					break;
				case 1:
					if (Confirm) { OpenStatsMenu(); break; }
					charanames[playerIndex].color = Color.white;
					if (Down) playerIndex = 3;
					else if (Left) playerIndex = 0;
					charanames[playerIndex].color = Color.yellow;
					break;
				case 2:
					if (Confirm) { OpenStatsMenu(); break; }
					charanames[playerIndex].color = Color.white;
					if (Up) playerIndex = 0;
					else if (Right) playerIndex = 3;
					charanames[playerIndex].color = Color.yellow;
					break;
				case 3:
					if (Confirm) { OpenStatsMenu(); break; }
					charanames[playerIndex].color = Color.white;
					if (Up) playerIndex = 1;
					else if (Left) playerIndex = 2;
					charanames[playerIndex].color = Color.yellow;
					break;
				default: Debug.LogError("partymenu character index broke at" + playerIndex); break;
			}
		} else {
			if (Cancel) { CloseStatsMenu(); return; }
			statsMenu.UpdateItemSelected(Down, Up, Left, Right, Confirm, Cancel);

		}
	}

	private void OpenStatsMenu() {
		statsMenu.ResetSelected();
		IsInStatsMenu = true;
		UpdateStats();

		for (int i = 0; i < 4; i++) {
			if (i == playerIndex) {
				charaImages[i].enabled = true;
				switch (playerIndex) {
					case 0: statsName.text = "Fargas"; break;
					case 1: statsName.text = "Oberon"; break;
					case 2: statsName.text = "Frea"; break;
					case 3: statsName.text = "Arcelus"; break;
					default: Debug.LogError("WOWSERS"); break;
				}

			} else charaImages[i].enabled = false;
		}
	}

	public void CloseStatsMenu() {
		statsMenu.ResetSelected();
		IsInStatsMenu = false;
	}

	public override void OnInactive() {
		IsInStatsMenu = false;
		charanames[playerIndex].color = Color.white;
		playerIndex = 0;
	}

	public override void OnActive() {
		if (IsInStatsMenu) {
			UpdateStats();
			playerIndex = 0;
			charanames[playerIndex].color = Color.yellow;
		} else {
			UpdateMeters();
		}
	}

	public void IncreaseStat(ButtonController button) {
		switch (button.buttonName) {
			case "Attack": ExpManager.instance.UsePointOnAttack(playerIndex); break;
			case "Defence": ExpManager.instance.UsePointOnDefence(playerIndex); break;
			case "Health": ExpManager.instance.UsePointOnHealth(playerIndex); break;
			case "Mana": ExpManager.instance.UsePointOnMana(playerIndex); break;
			case "Strength": ExpManager.instance.UsePointOnStrength(playerIndex); break;
			case "Agility": ExpManager.instance.UsePointOnAgility(playerIndex); break;
			case "Speed": ExpManager.instance.UsePointOnSpeed(playerIndex); break;
			case "Critical": break;
			default: Debug.LogError("Stat button name did not match"); break;
		}
		print("reached end");
		UpdateStats();
	}
}
