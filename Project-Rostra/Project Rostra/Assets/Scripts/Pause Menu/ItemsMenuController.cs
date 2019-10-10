#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrapper;

namespace Wrapper {
	//I hope this doesnt break or else ill brek someone
	public class Deque<T> : List<T> {

		public Deque() { }
		public Deque(int capacity) : base(capacity) { }
		public Deque(IEnumerable<T> collection) : base(collection) { }

		public void push_end(T item) {
			Insert(Count, item);
		}
		public void push_start(T item) {
			Insert(0, item);
		}

		public void pop_end() {
			RemoveAt(Count - 1);
		}
		public void pop_start() {
			RemoveAt(0);
		}
	}
}

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
		isunequipable = false;
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
	//uses
	public int isequipable; /* 0 is not equipable; 1 is equipable; 2 is unequipable; */
	public bool isunequipable;
	public bool isconsumeable;
	public bool isdroppable;
}

public class ItemsMenuController : SubMenu {

	//directly UI related variables
	public Color HighlightColor;
	public Color UnhighlightColor;
	public Color AvailOptionColor;
	public Color UnavailOptionColor;
	public CanvasGroup mainInventoryUI;
	public UnityEngine.UI.Image[] ItemImages;
	public UItemController[] mainList;
	public UnityEngine.UI.Image[] Options;
	public UnityEngine.UI.Text[] OptionsText;

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
	private bool[] availOptions = new bool[5];

	//menu state
	private int state = 0;
	/// 0 = main list
	/// 1 = item options
	/// 2 = equip menu
	/// 3 = consume menu

	#region Initialization

	protected override void Awake() {
		base.Awake();
		for (int i = 0; i < availOptions.Length; i++) {
			availOptions[i] = false;
		}
	}

	private void Start() {
	}

	#endregion

	#region Submenu Functions

	public void Update() {
	}

	public override void MenuUpdate() {
		//if this breaks i throw an error
		if (itemDeque.Count != ITEMS_PER_PAGE)
			Debug.LogError("The deque broke: deque.Count = " + itemDeque.Count + " ITEMS_PER_PAGE = " + ITEMS_PER_PAGE);

		switch (state) {
			case 0: // main list state
				mainList[RelativeIndex].UnHighlightItem(UnhighlightColor);
				ScrollUpMain();
				ScrollDownMain();
				mainList[RelativeIndex].HighlightItem(HighlightColor);
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
					goto case 1; // goto options state
				}
				break;
			case 1: // options state
				if (Cancel) {
					state = 0;
					optionsIndex = 0;
					Options[optionsIndex].color = UnhighlightColor;
					goto case 0;
				}
				Options[optionsIndex].color = UnhighlightColor;
				ScrollDownOptions();
				ScrollUpOptions();

				Options[optionsIndex].color = HighlightColor;
				if (Confirm && availOptions[optionsIndex]) {
					if (optionsIndex == 0) state = 2;
					if (optionsIndex == 1) state = 2; /* TODO: Change to unequip */
				}
				break;
			case 2: // eqipmen state

				break;
			case 3: // consume state 

				break;
			default: break;
		}
	}

	public override void OnVisible() {
		itemindex = 0;
		topofListIndex = 0;
		InitializeDeque();
		UpdateListUI();
		HandleOptionsList();
	}

	public override void OnActive() {

	}

	public override void OnInactive() {
		ClearDeque();
	}

	#endregion

	#region UI Updating

	private void ScrollDownMain() {
		if (Up && itemindex > 0) --itemindex;
		else return;
		if (itemindex < topofListIndex) {
			itemDeque.push_start(GenerateItem(itemindex));
			itemDeque.pop_end();
			--topofListIndex;
			UpdateListUI();
		}
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
	}

	private void UpdateListUI() {
		for (int i = 0; i < mainList.Length; i++) {
			string charaname = "";
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
			mainList[i].SetNormalItem(null, itemDeque[i].name, charaname, itemDeque[i].count > 1 ? "" + itemDeque[i].count : "");
		}
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
		if (itemDeque[itemindex].isequipable == 1)	availOptions[0] = true;
		if (itemDeque[itemindex].isequipable == 2)	availOptions[1] = true;
		if (itemDeque[itemindex].isconsumeable)		availOptions[2] = true;
													availOptions[3] = true; /// this should always be true
		if (itemDeque[itemindex].isdroppable)		availOptions[4] = true;

		for (int i = 0; i < availOptions.Length; i++) {
			if (availOptions[i]) {
				OptionsText[i].color = AvailOptionColor;
			} else {
				OptionsText[i].color = UnavailOptionColor;
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
		item.subtext = invinst.ItemDescription(item.itemID);
		item.equippedBy = invinst.invItem[index, 2];
		item.description = ""; /* TODO: add proper description */

		string[] itemOptions = invinst.ItemOptions(item.itemID).ToArray();
		foreach (string option in itemOptions) {
			switch (option) {
				case "Use":
					item.isconsumeable = true;
					break;
				case "Move":
					Debug.Log("Why is the item moveable?");
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
