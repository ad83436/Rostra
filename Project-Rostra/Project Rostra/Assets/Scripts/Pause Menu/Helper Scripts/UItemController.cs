using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UItemController : MonoBehaviour {

	//panel
	public Image panel;

	//parts
	public Image itemIcon;
	public Text itemName;
	public Text eqippedBy;
	public Text count;
	
	public void HighlightItem(Color color) {
		panel.color = color;
	}

	public void UnHighlightItem(Color color) {
		panel.color = color;
	}

	public void SetNormalItem(Sprite itemIcon_, string itemName_, string eqippedBy_, string count_) {
		itemIcon.sprite = itemIcon_;
		itemName.text = itemName_;
		eqippedBy.text = eqippedBy_;
		count.text = count_;
	}

	public void SetSmallItem(string itemName_) {
		itemName.text = itemName_;
	}

}
