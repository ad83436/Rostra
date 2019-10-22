using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMenuController : SubMenu {

	#region Submenu Functions

	public override void MenuUpdate() {
		if (Cancel) {
			ExitMenu();
		}
	}

	#region Menu Events

	public override void OnActive() { }
	public override void OnInactive() { }
	public override void OnInvisible() { }
	public override void OnVisible() { }

	#endregion

	#endregion
}
