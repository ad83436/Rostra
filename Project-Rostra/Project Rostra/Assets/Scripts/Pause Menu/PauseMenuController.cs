using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649

public class PauseMenuController : MonoBehaviour {

	//singleton
	public static PauseMenuController instance;

	public static bool isPaused = false;
	private bool activeMenu = true;
	public bool ActiveMenu {
		get => activeMenu;
		set {
			activeMenu = value;
			listItems[currentListItem].color = Color.yellow;
		}
	}
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

	private const float REP_LENGTH = 0.12f;
	private const float REP_DELAY = 0.35f;
	private float UpTimer = 0f;
	private float DownTimer = 0f;
	private float LeftTimer = 0f;
	private float RightTimer = 0f;

	private bool pause = false;

	#endregion

	#region Initialization

	private void Awake() {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
		group = GetComponent<CanvasGroup>();
		group.alpha = 0f;

		for (int i = 0; i < allSubMenus.Length; i++) {
			allSubMenus[i].IsActive = false;
			allSubMenus[i].Visible = false;
		}
	}

	private void OnDestroy() {
		if (instance) instance = null;
	}

	#endregion

	#region Logic

	private void Update() {
		//update inputs
		pause = Input.GetButtonDown("Pause") && !BattleManager.battleInProgress && !DialogueManager.instance.isActive && !CutsceneManager.instance.isActive; //Player should not be able to open the menu insdie the battle
		Cancel = Input.GetButtonDown("Cancel");

		if ((pause && activeMenu) || (isPaused && activeMenu && Cancel)) {
			isPaused = !isPaused;
			if (isPaused) {
				//onPause logic
				currentListItem = 0;
				listItems[currentListItem].color = Color.yellow;
				group.alpha = 1f;
				allSubMenus[currentListItem].Visible = true;
				allSubMenus[currentListItem].OnActive();
			} else {
				//onUnPause logic
				listItems[currentListItem].color = Color.white;
				group.alpha = 0f;
				allSubMenus[currentListItem].Visible = false;
				allSubMenus[currentListItem].OnInactive();
				currentListItem = 0;
			}
		}

		HandleInDir();

		if (!isPaused) return;

		Confirm = Input.GetButtonDown("Confirm");
		
			DoAudioBlips();

		if (activeMenu) {

			//do main list
			if (Down) currentListItem++;
			else if (Up) currentListItem--;
			if (currentListItem > listItems.Length - 1) currentListItem = 0;
			else if (currentListItem < 0) currentListItem = listItems.Length - 1;

			if (Up || Down) {
				for (int i = 0; i < listItems.Length; i++) {
					if (currentListItem == i) listItems[i].color = Color.yellow;
					else listItems[i].color = Color.white;
				}
				for (int i = 0; i < allSubMenus.Length; i++) {
					if (currentListItem == i) allSubMenus[i].Visible = true;
					else allSubMenus[i].Visible = false;
				}
			}

			if (Confirm && allSubMenus[currentListItem].canOpen) {
				allSubMenus[currentListItem].IsActive = true;
				activeMenu = false;
				listItems[currentListItem].color = Color.white;
			}

		} else { // currently in a submenu or submenu is visible
			foreach (SubMenu item in allSubMenus) {
				if (item.Visible) item.MenuUpdate();
			}
		}

	}

	#endregion

	private void HandleInDir() {
		
		Up = Input.GetButtonDown("Up");
		if (Up) UpTimer = -REP_DELAY;

		if (!Up && Input.GetButton("Up")) {
			UpTimer += Time.deltaTime;
			if (UpTimer > REP_LENGTH) {
				UpTimer -= REP_LENGTH;
				Up = true;
			} else {
				Up = false;
			}
		}
		
		Down = Input.GetButtonDown("Down");
		if (Down) DownTimer = -REP_DELAY;

		if (!Down && Input.GetButton("Down")) {
			DownTimer += Time.deltaTime;
			if (DownTimer > REP_LENGTH) {
				DownTimer -= REP_LENGTH;
				Down = true;
			} else {
				Down = false;
			}
		}
		
		Left = Input.GetButtonDown("Left");
		if (Left) LeftTimer = -REP_DELAY;

		if (!Left && Input.GetButton("Left")) {
			LeftTimer += Time.deltaTime;
			if (LeftTimer > REP_LENGTH) {
				LeftTimer -= REP_LENGTH;
				Left = true;
			} else {
				Left = false;
			}
		}
		
		Right = Input.GetButtonDown("Right");
		if (Right) RightTimer = -REP_DELAY;

		if (!Right && Input.GetButton("Right")) {
			RightTimer += Time.deltaTime;
			if (RightTimer > REP_LENGTH) {
				RightTimer -= REP_LENGTH;
				Right = true;
			} else {
				Right = false;
			}
		}


	}

	private void DoAudioBlips() {
		if (Confirm != Cancel) {
			if (Confirm) AudioManager.instance.playThisEffect("Boop");
			if (Cancel) AudioManager.instance.playThisEffect("Poob");
		} else if (Up || Down || Left || Right) {
			AudioManager.instance.playThisEffect("Bleep");
		}		
	}
}
