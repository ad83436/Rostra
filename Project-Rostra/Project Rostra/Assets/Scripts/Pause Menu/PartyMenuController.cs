using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	#endregion

	public override void MenuUpdate() {

	}

	public override void OnInactive() {
		
	}

	public override void OnActive() {
		
	}
}
