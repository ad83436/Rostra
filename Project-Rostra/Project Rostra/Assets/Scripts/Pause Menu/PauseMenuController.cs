using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour {

	//singleton
	public static PauseMenuController instance;

	public static bool isPaused = false;
	public bool activeMenu = true;
	private CanvasGroup group;
	
	[SerializeField] private UnityEngine.UI.Text[] listItems;
	[SerializeField] private SubMenu[] allSubMenus;

	#region Main list variables

	private int currentListItem = 0;

	#endregion

	#region InputVariables
	
	public bool Up { get; private set; }
	public bool Down { get; private set; }
	public bool Left { get; private set; }
	public bool Right { get; private set; }
	public bool Confirm { get; private set; }
	public bool Cancel { get; private set; }

	private bool pause = false;

	#endregion

	#region Initialization

	private void Awake() {
		if (instance == null) instance = this;
		else Debug.LogError("instance was not null in the PauseMenuController");
		group = GetComponent<CanvasGroup>();
		group.alpha = 0f;
	}

	private void OnDestroy() {
		if (instance) instance = null;
	}

	#endregion

	#region Logic

	private void Update() {
		//update inputs
		pause = Input.GetButtonDown("Pause");

		if (pause) {
			isPaused = !isPaused;
			if (isPaused) {
				//onPause logic
				currentListItem = 0;
				listItems[currentListItem].color = Color.blue;
				group.alpha = 1f;
			} else {
				//onUnPause logic
				listItems[currentListItem].color = Color.white;
				group.alpha = 0f;
			}
		}

		if (!isPaused) return;

		Up = Input.GetButtonDown("Up");
		Down = Input.GetButtonDown("Down");
		Left = Input.GetButtonDown("Left");
		Right = Input.GetButtonDown("Right");

		Confirm = Input.GetButtonDown("Confirm");
		Cancel = Input.GetButtonDown("Cancel");
		
		if (activeMenu) {
			//do main list
			if (Up) currentListItem++;
			else if (Down) currentListItem--;
			if (currentListItem > listItems.Length - 1) currentListItem = 0;
			else if (currentListItem < 0) currentListItem = listItems.Length - 1;
			
			if (Up || Down) {
				for (int i = 0; i < listItems.Length; i++) {
					if (currentListItem == i) listItems[i].color = Color.blue;
					else listItems[i].color = Color.white;
				}
				for (int i = 0; i < allSubMenus.Length; i++) {
					if (currentListItem == i) allSubMenus[i].Visible = true;
					else allSubMenus[i].Visible = false;
				}
			}

			if (Confirm) allSubMenus[currentListItem].IsActive = true;

		} else { // currently in a submenu or submenu is visible
			foreach (SubMenu item in allSubMenus) {
				if (item.Visible) item.MenuUpdate();
			}
		}

	}
	
	#endregion

}
