#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrapper;

public class UInvItem {
	public UInvItem() {
		itemID = -1;
		itemtype = -1;
		name = "";
		count = -1;
		subtext = "";
		equippedBy = -1;
		description = "";
		isequipable = 0;
		isconsumeable = false;
		isdroppable = false;
	}
	//item id
	public int itemID;
	//info
	public int itemtype;
	public string name;
	public int count;
	public string subtext;
	public int equippedBy;
	public string description;
	public string stats;
	//uses
	public int isequipable; /* 0 is not equipable; 1 is equipable; 2 is unequipable; */
	public bool isconsumeable;
	public bool isdroppable;
	public bool isswappable;
}

public class ItemsMenuController : SubMenu {

	//directly UI related variables
	public Color HighlightColor;
	public Color UnhighlightColor;
	public Color AvailOptionColor;
	public Color UnavailOptionColor;
	public Color SwappingColor;
	public CanvasGroup mainInventoryGroup;
	public CanvasGroup equipPanelGroup;
	public CanvasGroup usePanelGroup;
	public CanvasGroup peekPanelGroup;
	public UnityEngine.UI.Image[] ItemImages;
	public UItemController[] mainUItemsList;
	public UItemController[] equipUItemsList;
	public UnityEngine.UI.Image[] Options;
	public UnityEngine.UI.Text[] OptionsText;
	public UnityEngine.UI.Image[] EquipSelectionImages;
	public UnityEngine.UI.Image[] UseOptionsImages;
	public UnityEngine.UI.Text description;
	public UItemController[] peekItems;
	public UnityEngine.UI.Text GoldText;

	//reference to invetory
	private ref MainInventory invinst {
		get => ref MainInventory.invInstance;
	}

	//items list variabels
	private Deque<UInvItem> itemDeque = new Deque<UInvItem>();
	private const int ITEMS_PER_PAGE = 6;
	private int topofListIndex = 0;
	private int itemindex = 0;
	private int RelativeIndex {
		get => itemindex - topofListIndex;
	}

	//options list variables
	private int optionsIndex = 0;
	private bool[] availOptions;

	//equip list variables
	private int equipIndexX = 0;
	private int equipIndexY = 0;

	//use list variables
	private int useOptionsIndex = 0;

	//swap variables
	private int selectedIndex = 0;

	//menu state
	private int state = 0;
	/// 0 = main list
	/// 1 = item options
	/// 2 = equip menu
	/// 3 = consume menu
	/// 4 = main list swap mode
	/// 5 = unequip confim
	/// 6 = drop confirm

	#region Initialization

	private void Awake() {
		availOptions = new bool[5];
		for (int i = 0; i < availOptions.Length; i++) {
			availOptions[i] = false;
		}
		mainInventoryGroup.alpha = 0f;
		equipPanelGroup.alpha = 0f;
		usePanelGroup.alpha = 0f;
		peekPanelGroup.alpha = 0f;
	}

	private void Start() {
		invinst.invItem[0, 2] = 3;
	}

	#endregion

	#region Submenu Functions

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//

	public override void MenuUpdate() {
		//if this breaks i throw an error
		if (itemDeque.Count != ITEMS_PER_PAGE)
			Debug.LogError("The deque broke: deque.Count = " + itemDeque.Count + " ITEMS_PER_PAGE = " + ITEMS_PER_PAGE);

		switch (state) {
			case 0: { /// main list state
				if (Cancel) {
					UpdatePeekUI();
					mainUItemsList[RelativeIndex].UnHighlightItem(UnhighlightColor);
					ExitMenu();
					break;
				}
				mainUItemsList[RelativeIndex].UnHighlightItem(UnhighlightColor);
				ScrollUpMain();
				ScrollDownMain();
				mainUItemsList[RelativeIndex].HighlightItem(HighlightColor);
				HandleOptionsList();
				if (Confirm) {
					state = 1;
					for (int i = 0; i < availOptions.Length; i++) {
						if (availOptions[i]) {
							optionsIndex = i; // set to first available item in options list
							break;
						}
					}
					Options[optionsIndex].color = HighlightColor;
				}
				break;
			} /// main list state
			case 1: {/// options state
				if (Cancel) {
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					break;
				}

				Options[optionsIndex].color = UnhighlightColor;
				ScrollDownOptions();
				ScrollUpOptions();
				Options[optionsIndex].color = HighlightColor;

				if (Confirm && availOptions[optionsIndex]) {
					Options[optionsIndex].color = UnhighlightColor;
					if (optionsIndex == 0) { /// Equip
						state = 2;
						equipPanelGroup.alpha = 1f;
						UpdateEquipmentUI();
						break;
					}
					if (optionsIndex == 2) { /// Use
						state = 3;
						usePanelGroup.alpha = 1f;
						break;
					}
					if (optionsIndex == 3) { /// Swap
						state = 4;
						selectedIndex = itemindex;
						Options[optionsIndex].color = HighlightColor;
						break;
					}
					if (optionsIndex == 1) { /// Unequip
						state = 5;
						Options[optionsIndex].color = HighlightColor;
						OptionsText[1].text = "Confirm?";
						break;
					}
					if (optionsIndex == 4) { /// Drop
						state = 6;
						Options[optionsIndex].color = HighlightColor;
						OptionsText[4].text = "Confirm?";
						break;
					}
				}
				break;
			} /// options state
			case 2: { /// eqipmen state
				if (Cancel) {
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					equipPanelGroup.alpha = 0f;
					EquipSelectionImages[equipIndexY * 2 + equipIndexX].color = UnhighlightColor;
					equipIndexX = 0;
					equipIndexY = 0;
					break;
				}
				EquipSelectionImages[equipIndexY * 2 + equipIndexX].color = UnhighlightColor;
				if (equipIndexX == 0 && Right) equipIndexX = 1;
				else if (equipIndexX == 1 && Left) equipIndexX = 0;
				if (equipIndexY == 0 && Down) equipIndexY = 1;
				else if (equipIndexY == 1 && Up) equipIndexY = 0;
				EquipSelectionImages[equipIndexY * 2 + equipIndexX].color = HighlightColor;
				if (Confirm) {
					invinst.curOption = itemindex;
					invinst.ItemUseFunction(itemDeque[RelativeIndex].itemID, itemindex, equipIndexY * 2 + equipIndexX);
					itemDeque[RelativeIndex] = GenerateItem(itemindex);
					UpdateUItem(RelativeIndex);
					HandleOptionsList();
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					equipPanelGroup.alpha = 0f;
					EquipSelectionImages[equipIndexY * 2 + equipIndexX].color = UnhighlightColor;
					equipIndexX = 0;
					equipIndexY = 0;
					break;
				}
				break;
			} /// eqipmen state
			case 3: { /// consume state 
				if (Cancel) {
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					usePanelGroup.alpha = 0f;
					useOptionsIndex = 0;
					break;
				}

				UseOptionsImages[useOptionsIndex].color = UnhighlightColor;
				if (Up && useOptionsIndex != 0) useOptionsIndex--;
				else if (Down && useOptionsIndex != 3) useOptionsIndex++;
				UseOptionsImages[useOptionsIndex].color = HighlightColor;

				if (Confirm) {
					UseOptionsImages[useOptionsIndex].color = UnhighlightColor;
					usePanelGroup.alpha = 0f;

					if (useOptionsIndex < 4 && useOptionsIndex > -1)
						invinst.ItemUseFunction(itemDeque[itemindex].itemID, itemindex, useOptionsIndex);
					else
						Debug.LogError("Chara " + useOptionsIndex + " does not exist");

					itemDeque[itemindex] = GenerateItem(itemindex);
					UpdateUItem(RelativeIndex);
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					useOptionsIndex = 0;
					break;
				}
				break;
			} /// consume state 
			case 4: { /// main list swap state
				if (Cancel) {
					state = 0;
					if (selectedIndex - topofListIndex > -1 && selectedIndex - topofListIndex < ITEMS_PER_PAGE)
						mainUItemsList[selectedIndex - topofListIndex].HighlightItem(UnhighlightColor);
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					break;
				}

				if (selectedIndex - topofListIndex > -1 && selectedIndex - topofListIndex < ITEMS_PER_PAGE)
					mainUItemsList[selectedIndex - topofListIndex].HighlightItem(UnhighlightColor);
				mainUItemsList[RelativeIndex].UnHighlightItem(UnhighlightColor);
				ScrollUpMain();
				ScrollDownMain();
				mainUItemsList[RelativeIndex].HighlightItem(HighlightColor);
				if (selectedIndex - topofListIndex > -1 && selectedIndex - topofListIndex < ITEMS_PER_PAGE)
					mainUItemsList[selectedIndex - topofListIndex].HighlightItem(SwappingColor);

				if (Confirm) {
					invinst.SwapItems(selectedIndex, itemindex); // swap in the inventory
					itemDeque[RelativeIndex] = GenerateItem(itemindex);
					itemDeque[selectedIndex - topofListIndex] = GenerateItem(selectedIndex);
					UpdateListUI();
					state = 0;
					if (selectedIndex - topofListIndex > -1 && selectedIndex - topofListIndex < ITEMS_PER_PAGE)
						mainUItemsList[selectedIndex - topofListIndex].HighlightItem(UnhighlightColor);
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					break;
				}
				break;
			} /// main list swap state
			case 5: { /// unequip confirm
				if (Cancel) {
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					OptionsText[1].text = "Unequip";
					break;
				}

				if (Confirm) {
					invinst.curOption = itemindex;
					OptionsText[1].text = "Unequip";
					invinst.ItemUseFunction(itemDeque[RelativeIndex].itemID, itemindex, itemDeque[RelativeIndex].equippedBy);
					itemDeque[RelativeIndex] = GenerateItem(itemindex);
					UpdateUItem(RelativeIndex);
					HandleOptionsList();
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					break;
				}
				break;
			} /// unequip confirm
			case 6: { /// drop confirm
				if (Cancel) {
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					OptionsText[4].text = "Drop";
					break;
				}
				if (Confirm) {
					HandleOptionsList();
					OptionsText[4].text = "Drop";
					invinst.RemoveItem(itemindex);
					itemDeque[RelativeIndex] = GenerateItem(itemindex);
					UpdateUItem(RelativeIndex);
					HandleOptionsList();
					state = 0;
					Options[optionsIndex].color = UnhighlightColor;
					optionsIndex = 0;
					break;
				}
				break;
			} /// drop confirm
			default: break;
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//

	public override void OnVisible() {
		peekPanelGroup.alpha = 1f;
		//update the view
		UpdatePeekUI();
	}

	public override void OnInvisible() {
		peekPanelGroup.alpha = 0f;
	}

	public override void OnActive() {
		UpdateGold();
		itemindex = 0;
		topofListIndex = 0;
		InitializeDeque();
		UpdateListUI();
		HandleOptionsList();
		mainInventoryGroup.alpha = 1;
		UpdateDescription();
	}

	public override void OnInactive() {
		ClearDeque();
		mainInventoryGroup.alpha = 0;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//

	#endregion

	#region UI Updating

	public void UpdateGold() {
		GoldText.text = "Gold: " + MainInventory.totalMoney;
	}

	private void UpdateUItem(int relativeindex) {
		string charaname = "";
		switch (itemDeque[relativeindex].equippedBy) {
			case 0:
				charaname += "Fargas";
				break;
			case 1:
				charaname += "Oberon";
				break;
			case 2:
				charaname += "Frea";
				break;
			case 3:
				charaname += "Arcelus";
				break;
			default: break;
		}
		mainUItemsList[relativeindex].SetNormalItem(invinst.ItemIcon(itemDeque[relativeindex].itemID), 
			itemDeque[relativeindex].name, charaname, itemDeque[relativeindex].count > 1 ? "" + itemDeque[relativeindex].count : "");
	}

	private void UpdateListUI() {
		for (int i = 0; i < mainUItemsList.Length; i++) {
			string charaname = "";
			if (itemDeque[i].isequipable != 0) {
				switch (itemDeque[i].equippedBy) {
					case 0:
						charaname += "Fargas";
						break;
					case 1:
						charaname += "Oberon";
						break;
					case 2:
						charaname += "Frea";
						break;
					case 3:
						charaname += "Arcelus";
						break;
					default: break;
				}
			}
			mainUItemsList[i].SetNormalItem(invinst.ItemIcon(itemDeque[i].itemID), itemDeque[i].name, charaname, itemDeque[i].count > 1 ? "" + itemDeque[i].count : "");
		}
	}

	private void ScrollDownMain() {
		if (Up && itemindex > 0) --itemindex;
		else return;
		if (itemindex < topofListIndex) {
			itemDeque.push_start(GenerateItem(itemindex));
			itemDeque.pop_end();
			--topofListIndex;
			UpdateListUI();
		}
		UpdateDescription();
	}

	private void ScrollUpMain() {
		if (Down && itemindex < MainInventory.INVENTORY_SIZE - 1) ++itemindex;
		else return;
		if (itemindex > topofListIndex + ITEMS_PER_PAGE - 1) {
			itemDeque.push_end(GenerateItem(itemindex));
			itemDeque.pop_start();
			++topofListIndex;
			UpdateListUI();
		}
		UpdateDescription();
	}

	private void ScrollDownOptions() {
		if (Up && optionsIndex > 0) --optionsIndex;
		else return;
	}

	private void ScrollUpOptions() {
		if (Down && optionsIndex < MainInventory.INVENTORY_SIZE - 1) ++optionsIndex;
		else return;
	}

	private void HandleOptionsList() {
		for (int i = 0; i < availOptions.Length; i++) {
			availOptions[i] = false;
		}

		if (itemDeque[RelativeIndex].isequipable == 1) availOptions[0] = true;
		if (itemDeque[RelativeIndex].isequipable == 2) availOptions[1] = true;
		if (itemDeque[RelativeIndex].isconsumeable) availOptions[2] = true;
		if (itemDeque[RelativeIndex].isswappable) availOptions[3] = true;
		if (itemDeque[RelativeIndex].isdroppable) availOptions[4] = true;

		for (int i = 0; i < availOptions.Length; i++) {
			if (availOptions[i]) {
				OptionsText[i].color = AvailOptionColor;
			} else {
				OptionsText[i].color = UnavailOptionColor;
			}
		}
	}

	private void UpdateEquipmentUI() {
		for (int i = 0; i < 12; i++) {
			// clear all to "---"
			equipUItemsList[i].SetSmallItem(invinst.ItemName((int)ITEM_ID.NO_ITEM));
		}

		// new code
		int eqc0 = 0;
		int eqc1 = 0;
		int eqc2 = 0;
		int eqc3 = 0;
		int listItem = 0;
		for (int i = 0; i < MainInventory.INVENTORY_SIZE; i++) {
			if (invinst.ItemType(invinst.invItem[i, 0]) == (int)ITEM_TYPE.EQUIPABLE) {
				switch (invinst.invItem[i, 2]) {
					case -1:
						// skip this one because its not equipped
						break;
					case 0:
						if (eqc0 > 2) {
							Debug.LogError("eqc was 3 or more");
							break;
						}
						equipUItemsList[eqc0].SetSmallItem(invinst.ItemName(invinst.invItem[i, 0]));
						// iterate
						++eqc0; ++listItem;
						break;
					case 1:
						if (eqc1 > 3) {
							Debug.LogError("eqc was 3 or more");
							break;
						}
						equipUItemsList[eqc1 + 3].SetSmallItem(invinst.ItemName(invinst.invItem[i, 0]));
						// iterate
						++eqc1; ++listItem;
						break;
					case 2:
						if (eqc2 > 3) {
							Debug.LogError("eqc was 3 or more");
							break;
						}
						equipUItemsList[eqc2 + 6].SetSmallItem(invinst.ItemName(invinst.invItem[i, 0]));
						// iterate
						++eqc2; ++listItem;
						break;
					case 3:
						if (eqc3 > 3) {
							Debug.LogError("eqc was 3 or more");
							break;
						}
						equipUItemsList[eqc3 + 9].SetSmallItem(invinst.ItemName(invinst.invItem[i, 0]));
						// iterate
						++eqc3;
						++listItem;
						break;
					default: Debug.LogError("Weird but the player was: " + invinst.invItem[i, 2]); break;
				}

				if (listItem > 11) {
					break;
				}
			}
		}
	}

	private void UpdateDescription() {
		description.text = "<b>" + itemDeque[RelativeIndex].name + "</b>\n<i>"
			+ itemDeque[RelativeIndex].description + "</i>\n\n";
		if (itemDeque[RelativeIndex].subtext.Length > 0)
			description.text = description.text + "<b>Buffs</b>\n<i>" + itemDeque[RelativeIndex].subtext + "</i>";
	}

	private void UpdatePeekUI() {
		for (int i = 0; i < 12; i++) {
			// clear all to "---"
			peekItems[i].SetSmallItem(invinst.ItemName((int)ITEM_ID.NO_ITEM));
		}

		// new code
		int eqc0 = 0;
		int eqc1 = 0;
		int eqc2 = 0;
		int eqc3 = 0;
		int listItem = 0;
		for (int i = 0; i < MainInventory.INVENTORY_SIZE; i++) {
			if (invinst.ItemType(invinst.invItem[i, 0]) == (int)ITEM_TYPE.EQUIPABLE) {
				switch (invinst.invItem[i, 2]) {
					case -1:
						// skip this one because its not equipped
						break;
					case 0:
						if (eqc0 > 2) {
							Debug.LogError("eqc was 3 or more");
							break;
						}
						peekItems[eqc0].SetSmallItem(invinst.ItemName(invinst.invItem[i, 0]));
						// iterate
						++eqc0; ++listItem;
						break;
					case 1:
						if (eqc1 > 3) {
							Debug.LogError("eqc was 3 or more");
							break;
						}
						peekItems[eqc1 + 3].SetSmallItem(invinst.ItemName(invinst.invItem[i, 0]));
						// iterate
						++eqc1; ++listItem;
						break;
					case 2:
						if (eqc2 > 3) {
							Debug.LogError("eqc was 3 or more");
							break;
						}
						peekItems[eqc2 + 6].SetSmallItem(invinst.ItemName(invinst.invItem[i, 0]));
						// iterate
						++eqc2; ++listItem;
						break;
					case 3:
						if (eqc3 > 3) {
							Debug.LogError("eqc was 3 or more");
							break;
						}
						peekItems[eqc3 + 9].SetSmallItem(invinst.ItemName(invinst.invItem[i, 0]));
						// iterate
						++eqc3;
						++listItem;
						break;
					default: Debug.LogError("Weird but the player was: " + invinst.invItem[i, 2]); break;
				}

				if (listItem > 11) {
					break;
				}
			}
		}
	}

	#endregion

	#region Handle Deque

	private void InitializeDeque() {
		if (itemDeque.Count != 0) {
			Debug.LogError("Deque wasnt cleared correctly!");
			ClearDeque();
		}

		for (int i = 0; i < ITEMS_PER_PAGE; i++) {
			itemDeque.push_end(GenerateItem(i));
			//print(itemDeque[i].equippedBy + " for item " + (ITEM_ID)itemDeque[i].itemID);
		}
	}

	private void ClearDeque() {
		itemDeque.Clear();
	}

	private UInvItem GenerateItem(int index) {
		UInvItem item = new UInvItem();

		item.itemID = invinst.invItem[index, 0];
		item.itemtype = invinst.ItemType(item.itemID);
		item.name = invinst.ItemName(item.itemID);
		item.count = invinst.invItem[index, 1];
		item.equippedBy = invinst.invItem[index, 2];

		// generate decription
		item.description = invinst.ItemDescription(item.itemID);
		item.subtext = "";

		float[] stats = invinst.ItemStats(item.itemID);

		if (stats[0] > 0f) item.subtext += "Attack: " + stats[0] + "\n";
		if (stats[1] > 0f) item.subtext += "Defence: " + stats[1] + "\n";
		if (stats[2] > 0f) item.subtext += "Strength: " + stats[2] + "\n";
		if (stats[3] > 0f) item.subtext += "Agility: " + stats[3] + "\n";
		if (stats[4] > 0f) item.subtext += "Critical: " + stats[4] + "\n";
		if (stats[5] > 0f) item.subtext += "Health: " + stats[5] + "\n";
		if (stats[6] > 0f) item.subtext += "Magic: " + stats[6] + "\n";

		invinst.curOption = index;
		string[] itemOptions = invinst.ItemOptions(item.itemID, index).ToArray();
		foreach (string option in itemOptions) {
			switch (option) {
				case "Use":
					item.isconsumeable = true;
					break;
				case "Move":
					item.isswappable = true;
					break;
				case "Drop":
					item.isdroppable = true;
					break;
				case "Equip":
					item.isequipable = 1;
					break;
				case "Unequip":
					item.isequipable = 2;
					break;
				default: Debug.Log("Item option was not found: \"" + option + "\""); break;
			}
		}

		return item;
	}

	private void UpdateItem(int index) {
		/* TODO: add functionality */
	}

	private void UpdateDeque() {
		/* TODO: add functionality */
	}

	#endregion

}
