using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649

public abstract class SubMenu : MonoBehaviour {

	[SerializeField] private CanvasGroup group;

	public bool canOpen = true;
	private bool active = false;
	public bool IsActive {
		get => active;
		set {
			if (active && !value) {
				active = value;
				OnInactive();
			} else if (!active && value) {
				active = value;
				OnActive();
			}
		}
	}

	public bool Visible {
		get => group.alpha == 1f;
		set {
			if (value == true && group.alpha != 1f) OnVisible();
			else if (value == false && group.alpha != 0) OnInvisible();
			group.alpha = value ? 1f : 0f;
		}
	}

	protected bool Up => PauseMenuController.instance.Up;
	protected bool Down => PauseMenuController.instance.Down;
	protected bool Left => PauseMenuController.instance.Left;
	protected bool Right => PauseMenuController.instance.Right;
	protected bool Confirm => PauseMenuController.instance.Confirm;
	protected bool Cancel => PauseMenuController.instance.Cancel;

	public abstract void MenuUpdate();
	public abstract void OnVisible();
	public abstract void OnInvisible();
	public abstract void OnActive();
	public abstract void OnInactive();

	protected void ExitMenu() {
		IsActive = false;
		PauseMenuController.instance.ActiveMenu = true;
	}

}
