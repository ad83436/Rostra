using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubMenu : MonoBehaviour {
	[SerializeField]
	public CanvasGroup group;

	private bool activeM = false;
	public bool ActiveM {
		get => false;
		set {
			activeM = value;
			group.alpha = value ? 1f : 0f;
		}
	}

	public abstract void CanvasUpdate();
	public abstract void OnOpenMenu();
	public abstract void OnCloseMenu();

}
