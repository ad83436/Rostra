using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonController {
	public string buttonName;
	public UnityEngine.UI.Text buttonText;

	public void SetSelected(Color selectedColor) {
		buttonText.color = selectedColor;
	}

	public void SetUnselected(Color unselectedColor) {
		buttonText.color = unselectedColor;
	}
}

[System.Serializable]
public class ButtonHelper {
	public ButtonController[] buttons;

	public ButtonHelper(int size) {
		buttons = new ButtonController[size];
	}

	public ButtonHelper() { }

	public ref ButtonController this[int index]{
		get => ref buttons[index];
	}

	public int Length => buttons.Length;
}

[System.Serializable]
public class ItemMenu {
	public Color unselectedColor = Color.white;
	public Color selectedColor = Color.yellow;
	public ButtonHelper[] buttonHelper;
	public int xIndex { get; private set; }
	public int yIndex { get; private set; }

	//delegates
	public delegate void PressedItemDel(ButtonController button);
	public delegate void ExitPressedDel();
	public PressedItemDel pressedDelegate;
	public ExitPressedDel exitDelegate;

	public ItemMenu() {
		xIndex = 0; yIndex = 0;
	}

	public ItemMenu(int width, int height) {
		buttonHelper = new ButtonHelper[width];
		for (int i = 0; i < buttonHelper.Length; i++) {
			buttonHelper[i] = new ButtonHelper(height);
		}
		xIndex = 0; yIndex = 0;
	}

	public ItemMenu(ButtonHelper[] buttons_) {
		buttonHelper = buttons_;
	}

	public void UpdateItemSelected(bool IncreaseY, bool DecreaseY, bool DecreaseX, bool IncreaseX, bool Confirm, bool Cancel) {
		if (Cancel) {
			exitDelegate();
			return;
		}

		if (IncreaseX || IncreaseY || DecreaseX || DecreaseY) {
			buttonHelper[yIndex][xIndex].SetUnselected(unselectedColor);

			if (IncreaseY) yIndex++;
			else if (DecreaseY) yIndex--;
			//Debug.Log(buttonHelper.Length);
			ClampYValue(buttonHelper.Length - 1);

			if (IncreaseX) xIndex++;
			else if (DecreaseX) xIndex--;
			//Debug.Log(buttonHelper[0].Length);
			ClampXValue(buttonHelper[0].Length - 1);

			buttonHelper[yIndex][xIndex].SetSelected(selectedColor);
		}

		if (Confirm) {
			pressedDelegate(buttonHelper[yIndex][xIndex]);
		}
	}

	void ClampXValue(int max) {
		if (xIndex > max) xIndex = max; 
		if (xIndex < 0) xIndex = 0; 
	}

	void ClampYValue(int max) {
		if (yIndex > max) yIndex = max; 
		if (yIndex < 0) yIndex = 0;
	}

	public void ResetSelected() {
		buttonHelper[yIndex][xIndex].SetUnselected(unselectedColor);
		xIndex = 0;
		yIndex = 0;
		buttonHelper[xIndex][yIndex].SetSelected(selectedColor);
	}
}
