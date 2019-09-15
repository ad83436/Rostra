using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour {

	//singleton
	public static PauseMenuController instance;

	public static bool isPaused = false;
	public bool activeMenu = true;

	[SerializeField]
	private CanvasGroup PauseMenu;

	[SerializeField] private UnityEngine.UI.Text[] listItems;
	[SerializeField] private SubMenu[] allSubMenus;

	#region Main list variables

	private int currentListItem = 0;

	#endregion

	#region InputVariables

	public bool HeldUp { get; private set; }
	public bool HeldDown { get; private set; }
	public bool HeldLeft { get; private set; }
	public bool HeldRight { get; private set; }

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
	}

	private void OnDestroy() {
		if (instance) instance = null;
	}

	#endregion

	#region Logic

	private void Update() {
		//update inputs
		pause = Input.GetButtonDown("pause");

		if (pause) {
			isPaused = !isPaused;
			if (isPaused) {
				//onPause logic
			} else {
				//onUnPause logic
			}
		}

		if (!isPaused) return;

		Up = !HeldUp && Input.GetAxis("Vertical") > 0f;			HeldUp = Input.GetAxis("Vertical") > 0f;
		Down = !HeldDown && Input.GetAxis("Vertical") < 0f;		HeldDown = Input.GetAxis("Vertical") < 0f;
		Left = !HeldLeft && Input.GetAxis("Horizontal") < 0f;	HeldLeft = Input.GetAxis("Horizontal") < 0f;
		Right = !HeldRight && Input.GetAxis("Horizontal") > 0f; HeldRight = Input.GetAxis("Horizontal") > 0f;

		if (activeMenu) {
			//do main list
			if (Up) currentListItem++;
			else if (Down) currentListItem--;
			if (currentListItem > 5) currentListItem = 0;
			else if (currentListItem < 0) currentListItem = 5;



			if (Confirm) {
				switch (currentListItem) {
					case 0:

						break;
					case 1: break;
					case 2: break;
					case 3: break;
					case 4: break;
					default: Debug.LogError("Something broke in the PauseMenuController tell dom"); break;
				}
			}


		} else { // currently in a submenu
			foreach (SubMenu item in allSubMenus) {
				item.MenuUpdate;
			}
		}

	}
	
	#endregion

}
