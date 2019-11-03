#pragma warning disable CS0649

using UnityEngine;
using System.Collections.Generic;
using Wrapper;

public class ItemShopUI : MonoBehaviour {
	// Singleton and static properties
	public static ItemShopUI Singleton { get; private set; }
	public static bool IsOpen { get; private set; }
    public MerchantTrigger activeMerchant;

	// UI Color variables
	[SerializeField] private Color Color_Highlight;
	[SerializeField] private Color Color_Unhighlight;

	// UI text variables
	[SerializeField] private UnityEngine.UI.Text Text_GoldCounter;
	[SerializeField] private UnityEngine.UI.Text Text_Description;
	[SerializeField] private UnityEngine.UI.Text[] TextArr_ListNames;
	[SerializeField] private UnityEngine.UI.Text[] TextArr_ListPrices;
	[SerializeField] private UnityEngine.UI.Text[] TextArr_ListOwned;

	// UI image variables
	[SerializeField] private UnityEngine.UI.Image[] ImageArr_Selection;
	[SerializeField] private UnityEngine.UI.Image[] ImageArr_List;

	// Canvas Groups
	[SerializeField] private CanvasGroup Group_Main;
	[SerializeField] private CanvasGroup Group_Selection;
	[SerializeField] private CanvasGroup Group_List;
	[SerializeField] private CanvasGroup Popup_Confirm;

	// UI state variables
	private int Index_Selection = 0;
	private int Index_TopOfList = 0;
	private int Index_List = 0;
	private int RelativeIndex => Index_List - Index_TopOfList;

	private int State_UI = 0;
	/// 0 = Nonactive state
	/// 1 = Selection state
	/// 2 = Buy state
	/// 3 = Sell state
	/// 4 = Buy confirm state
	/// 5 = Sell confirm state

	// Item List Deque
	private struct ShopItem {
		public string Name;
		public string Description;
		public int Stacked;
		public int Price;
		public int ItemID;
	}
	private Deque<ShopItem> Deque_ShopItems;

	// Useful Properties
	private ref List<int> ItemsInShop => ref ItemShop.singleton.shopItems;
	private MainInventory MainInv => MainInventory.invInstance;
	private ItemShop Shop => ItemShop.singleton;

	// Input
	private bool In_Confirm => Input.GetButtonDown("Confirm");
	private bool In_Cancel => Input.GetButtonDown("Cancel");
	private bool In_Up => Input.GetButtonDown("Up");
	private bool In_Down => Input.GetButtonDown("Down");
	//private bool In_Left => Input.GetButtonDown("Left");
	//private bool In_Right => Input.GetButtonDown("Right");

	#region UI update functions

	// checks the entire inventory for how many of a certain item you have
	private int ItemCount(int ItemID) {
		int count = 0;
		for (int index = 0; index < MainInventory.INVENTORY_SIZE; index++) {
			// increase the count for each stack found
			if (MainInv.invItem[index, 0] == ItemID) count += MainInv.invItem[index, 1];
		}
		return count;
	}

	private void UpdateGoldCounter() {
		Text_GoldCounter.text = "Gold: " + MainInventory.totalMoney;
	}

	private void ClearDeque() {
		Deque_ShopItems.Clear();
	}

	#region Buyable list

	private void UpdateBuyable(int relindex) {
		if (relindex < 0 || relindex > 5) {
			Debug.LogError("index was outside but thats ok because I cought it");
			return;
		}

		// Create updated item
		ShopItem item = new ShopItem();
		item.ItemID = ItemsInShop[relindex + Index_TopOfList];
		item.Name = MainInv.ItemName(item.ItemID);
		item.Price = MainInv.ItemPrice(item.ItemID);
		item.Description = item.Name + "\n\n" + MainInv.ItemDescription(item.ItemID);
		item.Stacked = ItemCount(item.ItemID);

		// Change to updated item
		Deque_ShopItems[relindex] = item;
	}

	private void FillDequeWithBuyables() {
		ShopItem item = new ShopItem();
		int i = 0;
		// fill each slot in list
		for (; i < 5; i++) {
			if (i >= ItemsInShop.Count) break;
			item.ItemID = ItemsInShop[i];
			item.Name = MainInv.ItemName(item.ItemID);
			item.Price = MainInv.ItemPrice(item.ItemID);
			item.Description = item.Name + "\n\n" + MainInv.ItemDescription(item.ItemID);
			item.Stacked = ItemCount(item.ItemID);
			Deque_ShopItems.push_end(item);
		}

		// fill the rest if they are empty
		if (i < 5) {
			item.ItemID = 0;
			item.Name = "---";
			item.Price = 0;
			item.Description = "---";
			item.Stacked = 0;
			for (; i < 5; i++)
				Deque_ShopItems.push_end(item);
		}
	}

	private void TopWithBuyable() {
		if (Index_TopOfList < 0) {
			Debug.LogError("You cant do that");
			return;
		}
		// get rid of last item
		Deque_ShopItems.pop_end();

		// create item
		ShopItem item = new ShopItem();
		item.ItemID = ItemsInShop[Index_TopOfList];
		item.Name = MainInv.ItemName(item.ItemID);
		item.Price = MainInv.ItemPrice(item.ItemID);
		item.Description = item.Name + "\n\n" + MainInv.ItemDescription(item.ItemID);
		item.Stacked = ItemCount(item.ItemID);

		// add new item
		Deque_ShopItems.push_start(item);
	}

	private void BottomWithBuyable() {
		if (Index_TopOfList + 5 >= ItemsInShop.Count) {
			Debug.LogError("You cant do that");
			return;
		}
		// get rid of last item
		Deque_ShopItems.pop_start();

		// create item
		ShopItem item = new ShopItem();
		item.ItemID = ItemsInShop[Index_TopOfList + 5];
		item.Name = MainInv.ItemName(item.ItemID);
		item.Price = MainInv.ItemPrice(item.ItemID);
		item.Description = item.Name + "\n\n" + MainInv.ItemDescription(item.ItemID);
		item.Stacked = ItemCount(item.ItemID);

		// add new item
		Deque_ShopItems.push_end(item);
	}

	#endregion

	#region Sellable list

	private void UpdateSellable(int relindex) {
		if (relindex < 0 || relindex > 5) {
			Debug.LogError("index was outside but thats ok because I cought it");
			return;
		}

		// Create updated item
		ShopItem item = new ShopItem();
		item.ItemID = MainInv.invItem[relindex + Index_TopOfList, 0];
		item.Name = MainInv.ItemName(item.ItemID);
		item.Price = MainInv.ItemPrice(item.ItemID);
		item.Description = item.Name + "\n\n" + MainInv.ItemDescription(item.ItemID);
		item.Stacked = MainInv.invItem[relindex + Index_TopOfList, 1];

		// Change to updated item
		Deque_ShopItems[relindex] = item;
	}

	private void FillDequeWithSellables() {
		ShopItem item = new ShopItem();
		int i = 0;
		// fill each slot in list
		for (; i < 5; i++) {
			item.ItemID = MainInv.invItem[i, 0];
			item.Name = MainInv.ItemName(item.ItemID);
			item.Price = MainInv.ItemPrice(item.ItemID);
			item.Description = item.Name + "\n\n" + MainInv.ItemDescription(item.ItemID);
			item.Stacked = MainInv.invItem[i, 1];
			Deque_ShopItems.push_end(item);
		}
	}

	private void TopWithSellable() {
		if (Index_TopOfList < 0) {
			Debug.LogError("You cant do that");
			return;
		}
		// get rid of last item
		Deque_ShopItems.pop_end();

		// create item
		ShopItem item = new ShopItem();
		item.ItemID = MainInv.invItem[Index_TopOfList, 0];
		item.Name = MainInv.ItemName(item.ItemID);
		item.Price = MainInv.ItemPrice(item.ItemID);
		item.Description = item.Name + "\n\n" + MainInv.ItemDescription(item.ItemID);
		item.Stacked = MainInv.invItem[Index_TopOfList, 1];

		// add new item
		Deque_ShopItems.push_start(item);
	}

	private void BottomWithSellable() {
		if (Index_TopOfList + 5 >= MainInventory.INVENTORY_SIZE) {
			Debug.LogError("You cant do that");
			return;
		}
		// get rid of last item
		Deque_ShopItems.pop_start();

		// create item
		ShopItem item = new ShopItem();
		item.ItemID = MainInv.invItem[Index_TopOfList + 5, 0];
		item.Name = MainInv.ItemName(item.ItemID);
		item.Price = MainInv.ItemPrice(item.ItemID);
		item.Description = item.Name + "\n\n" + MainInv.ItemDescription(item.ItemID);
		item.Stacked = MainInv.invItem[Index_TopOfList + 5, 1];

		// add new item
		Deque_ShopItems.push_end(item);
	}

	#endregion

	private void UpdateListUI() {
		if (Deque_ShopItems.Count != 5) {
			Debug.LogError("There are " + Deque_ShopItems.Count + " in the deque");
		}

		for (int i = 0; i < 5; i++) {
			TextArr_ListNames[i].text = Deque_ShopItems[i].Name;
			if (Deque_ShopItems[i].Price > 0) TextArr_ListPrices[i].text = (State_UI == 2 ? "Buy Price: " : "Sell Price: ") +
										 (Deque_ShopItems[i].Price == 0 ? "---" : "" + Deque_ShopItems[i].Price); // weird thing
			else TextArr_ListPrices[i].text = "---";
			if (Deque_ShopItems[i].Stacked > 1) TextArr_ListOwned[i].text = (State_UI == 3 ? "Stack: " : "Own: ") +
										(Deque_ShopItems[i].Stacked == 0 ? "---" : "" + Deque_ShopItems[i].Stacked); // weird thing 2
			else TextArr_ListOwned[i].text = "---";
		}
	}

	private void UpdateDescription(string text) {
		Text_Description.text = text;
	}
	
	private void UpdateDescription(int relindex) {
		Text_Description.text = Deque_ShopItems[relindex].Description;
	}

	#endregion

	#region Unity Events

	#region Initialization & Destruction

	private void Awake() {
		// Singleton
		if (Singleton == null) Singleton = this;
		else Debug.LogError("There are multiple itemshop UIs loaded");

		// Static
		IsOpen = false;

		// Canvas groups
		Group_Main.alpha = 0f;
		Group_Selection.alpha = 1f;
		Group_List.alpha = 0f;
		Popup_Confirm.alpha = 0f;

		// Deque
		Deque_ShopItems = new Deque<ShopItem>();
	}

	private void OnDestroy() {
		if (Singleton = this) Singleton = null;
	}

	#endregion

	private void Update() {
		if (Input.GetKeyDown(KeyCode.T)) OpenItemShop();

		switch (State_UI) {
			case 0: { /// Nonactive state
				break;
			} /// Nonactive state
			case 1: { /// Selection state
				// check for exit
				if (In_Cancel) {
					ImageArr_Selection[Index_Selection].color = Color_Unhighlight; // unselect
					Index_Selection = 0; // reset index
					State_UI = 0; // change to inactive state
					Group_Main.alpha = 0f; // hide UI
                    IsOpen = false;
                    activeMerchant.currentState = MerchantTrigger.merchantState.idle; //Make sure the conversation with the merchant does not start again if the player chooses Exit
                    activeMerchant.canTalkAgain = false;
                    activeMerchant = null;
                        return;
				}

				// move index
				ImageArr_Selection[Index_Selection].color = Color_Unhighlight; // Unhighlight
				if (In_Up && Index_Selection > 0) --Index_Selection; // move up
				else if (In_Up) Index_Selection = 2; // also move up
				if (In_Down && Index_Selection < 2) ++Index_Selection; // move down
				else if (In_Down) Index_Selection = 0; // also move down
				ImageArr_Selection[Index_Selection].color = Color_Highlight; // highlight

				if (In_Confirm) {
					switch (Index_Selection) {
						case 0:
							FillDequeWithBuyables(); // fill the deque
							State_UI = 2; // change to buy state
							Group_Selection.alpha = 0f; // hide selection UI
							Group_List.alpha = 1f; // show list UI
							break;
						case 1:
							FillDequeWithSellables(); // fill the deque
							State_UI = 3; // change to sell state
							Group_Selection.alpha = 0f; // hide selection UI
							Group_List.alpha = 1f; // show list UI
							break;
						case 2:
							ImageArr_Selection[Index_Selection].color = Color_Unhighlight; // unselect
							Index_Selection = 0; // reset index
							State_UI = 0; // change to inactive state
							Group_Main.alpha = 0f; // hide UI
                            IsOpen = false;
                            activeMerchant.currentState = MerchantTrigger.merchantState.idle; //Make sure the conversation with the merchant does not start again if the player chooses Exit
                            activeMerchant.canTalkAgain = false;
                            activeMerchant = null;

                                return;
						default: break;
					}

					break;
				}

				break;
			} /// Selection state
			case 2: { /// Buy state
				if (In_Cancel) {
					ImageArr_List[RelativeIndex].color = Color_Unhighlight; // Unhighlight
					ClearDeque();
					Index_TopOfList = 0;
					Index_List = 0;
					State_UI = 1;
					Group_List.alpha = 0f;
					Group_Selection.alpha = 1f;
					UpdateDescription("---");
					break;
				}

				ImageArr_List[RelativeIndex].color = Color_Unhighlight; // Unhighlight
				if (In_Up && Index_List != 0) { // move up
					if (RelativeIndex == 0) {
						--Index_TopOfList;
						TopWithBuyable();
					}
					--Index_List;
				}
				if (In_Down && Index_List != ItemsInShop.Count) { // move down
					if (RelativeIndex == 4) {
						++Index_TopOfList;
						BottomWithBuyable();
					}
					++Index_List;
				}
				ImageArr_List[RelativeIndex].color = Color_Highlight; // Highlight

				// Update UI
				UpdateListUI();
				UpdateDescription(RelativeIndex);

				if (In_Confirm && Deque_ShopItems[Index_List].Price > 0) {
					State_UI = 4;
					Popup_Confirm.alpha = 1f;
					break;
				}
				break;
			} /// Buy state
			case 3: { /// Sell state
				if (In_Cancel) {
					ImageArr_List[RelativeIndex].color = Color_Unhighlight; // Unhighlight
					ClearDeque();
					Index_TopOfList = 0;
					Index_List = 0;
					State_UI = 1;
					Group_List.alpha = 0f;
					Group_Selection.alpha = 1f;
					UpdateDescription("---");
					break;
				}

				ImageArr_List[RelativeIndex].color = Color_Unhighlight; // Unhighlight
				if (In_Up && Index_List != 0) { // move up
					if (RelativeIndex == 0) {
						--Index_TopOfList;
						TopWithSellable();
					}
					--Index_List;
				}
				if (In_Down && Index_List != MainInventory.INVENTORY_SIZE) { // move down
					if (RelativeIndex == 4) {
						++Index_TopOfList;
						BottomWithSellable();
					}
					++Index_List;
				}
				ImageArr_List[RelativeIndex].color = Color_Highlight; // Highlight

				// Update UI
				UpdateListUI();
				UpdateDescription(RelativeIndex);

				if (In_Confirm && Deque_ShopItems[Index_List].Stacked > 0) {
					State_UI = 5;
					Popup_Confirm.alpha = 1f;
					break;
				}
				break;
			} /// Sell state
			case 4: { /// Buy confirm
				if (In_Cancel) {
					State_UI = 2; // return to buy state
					Popup_Confirm.alpha = 0f; // close popup
					break;
				}

				if (In_Confirm) {
					State_UI = 2; // return to buy state
					Popup_Confirm.alpha = 0f; // close popup
					Shop.BuyItem(Deque_ShopItems[Index_List].ItemID); // buy item
					UpdateBuyable(RelativeIndex); // update the listed item
					UpdateGoldCounter(); // update gold
					break;
				}
				break;
			} /// Buy confirm
			case 5: { /// Sell confirm
				if (In_Cancel) {
					State_UI = 3; // return to sell state
					Popup_Confirm.alpha = 0f; // close popup
					break;
				}

				if (In_Confirm) {
					State_UI = 3; // return to sell state
					Popup_Confirm.alpha = 0f; // close popup
					Shop.SellItem(Deque_ShopItems[Index_List].ItemID, Index_List); // Sell item
					UpdateSellable(RelativeIndex); // update the listed item
					UpdateGoldCounter(); // update gold
					break;
				}
				break;
			} /// Sell confirm
			default: Debug.LogError("Non-existing state: " + State_UI); break;
		}

	}

	#endregion

	#region Most Important Function

	public static void OpenItemShop() {
		Singleton.State_UI = 1;
		Singleton.Group_Main.alpha = 1f;
		IsOpen = true;
	}

	#endregion

}
