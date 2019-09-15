using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubMenu : MonoBehaviour {
	
	public CanvasGroup group;

	private bool activeMenu = false;
	public bool ActiveMenu {
		get => activeMenu;
		set {
			if (activeMenu && !value) {
				activeMenu = value;
				OnInactive();
			} else if (!activeMenu && value) {
				activeMenu = value;
				OnActive();
			}
		}
	}

	public bool Visible {
		get => group.alpha == 1f;
		set => group.alpha = value ? 1f : 0f;
	}

	protected bool HeldUp => PauseMenuController.instance.HeldUp;
	protected bool HeldDown => PauseMenuController.instance.HeldDown;
	protected bool HeldLeft => PauseMenuController.instance.HeldLeft;
	protected bool HeldRight => PauseMenuController.instance.HeldUp;
	protected bool Up => PauseMenuController.instance.Up;
	protected bool Down => PauseMenuController.instance.Down;
	protected bool Left => PauseMenuController.instance.Left;
	protected bool Right => PauseMenuController.instance.Right;
	protected bool Confirm => PauseMenuController.instance.Confirm;
	protected bool Cancel => PauseMenuController.instance.Cancel;

	public abstract void MenuUpdate();
	public abstract void OnActive();
	public abstract void OnInactive();

	protected void ExitMenu() {
		ActiveMenu = false;
		PauseMenuController.instance.activeMenu = true;
	}

}
