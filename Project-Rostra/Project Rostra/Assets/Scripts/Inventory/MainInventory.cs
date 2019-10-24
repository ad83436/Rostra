using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        October 10th, 2019

public class MainInventory : MonoBehaviour {
	public static MainInventory invInstance;    // Holds the current inventory instance in a single variable
	public static int totalMoney = 500000;           // The amount of money the player has
	public static int INVENTORY_SIZE = 60;      // The maximum size of the inventory
	public int[,] invItem = new int[INVENTORY_SIZE, 3];
	// NOTE -- Element 0 is the item's ID value that will point to its name, description, icon, etc.
	//         Element 1 is how many items currently occupy the slot in the inventory
	//         Element 2 is what character has this item equipped (Ex. armor and weapons)

	// A list to store the slots of all consumable items within the player's inventory (Used in battles only)
	public List<int> consumableInv = new List<int>();

	// The variables that are used for drawing the GUI to the screen
	public Font GuiSmall;
	public bool isVisible = false;

	// Variables for holding a to-be-swapped item in the inventory
	private int[] itemToSwap = new int[3];      // Holds data about the item being swapped in the inventory
	private int slotToSwapTo = -1;              // The slot the the selected item will send the other item to
	private bool swappingItems = false;         // If true, the inventory will be in an "Item Swap" state. Meaning, no items can be selected until the swap is declined or completed

	// Variables for selecting options and inventory items
	public int curOption = 0;                   // The current inventory item the player has their cursor over
	public int selectedOption = -1;             // The item that the player has selected in the inventory
	private int subCurOption = 0;               // The current option the player has their cursor over after selecting an item

	// Variables for selecting the player to use a consumable item
	private bool playerChooseWindow = false;    // If true, the inventory needs a player to be chosen (For using consumable items)
	private int curPlayerOption = 0;            // Which player is being chosen to use the given item
	private bool[] canPlayerEquip = { true, true, true, true };

	// Variables for drawing the inventory to the screen
	private int firstToDraw = 0;                // The first item from the inventory array to draw out of the full inventory
	private int numToDraw = 15;                 // How many inventory items that can be visible to the player at any given time

	//Amounts
	public int itemAddAmount = 0;

    public Sprite[] itemIcons;

	// Set the main inventory instance to this one if no inventory is active, delete otherwise
	public void Awake() {
		if (invInstance == null) {
			invInstance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
		// Set all 3rd elements in the array to -1 (Equipped by nobody) 
		for (int i = 0; i < INVENTORY_SIZE; i++) { invItem[i, 2] = -1; }
	}

	// FOR TESTING
	private void Start() {
		invItem[0, 0] = (int)ITEM_ID.TEST_WEAPON1;
		invItem[0, 1] = 1;

		invItem[1, 0] = (int)ITEM_ID.HP_POTION;
		invItem[1, 1] = ItemStackLimit((int)ITEM_ID.HP_POTION);
		consumableInv.Add(1);

		invItem[2, 0] = (int)ITEM_ID.MP_ELIXER;
		invItem[2, 1] = ItemStackLimit((int)ITEM_ID.MP_ELIXER);
		consumableInv.Add(2);

        invItem[3, 0] = (int)ITEM_ID.HOPE_POTION;
        invItem[3, 1] = ItemStackLimit((int)ITEM_ID.HOPE_POTION);
        consumableInv.Add(3);

        invItem[4, 0] = (int)ITEM_ID.TEST_ARMOR1;
		invItem[4, 1] = 1;

	}

	// Handling keyboard functionality
	private void Update() {
		// Getting Keyboard Input
		bool keyOpen, keySelect, keyReturn, keyUp, keyDown;
		keyOpen = Input.GetKeyDown(KeyCode.O);
		keySelect = Input.GetKeyDown(KeyCode.Z);
		keyReturn = Input.GetKeyDown(KeyCode.X);
		keyUp = Input.GetKeyDown(KeyCode.UpArrow);
		keyDown = Input.GetKeyDown(KeyCode.DownArrow);

		// Opening and Closing the Inventory Window
		if (keyOpen) {
			isVisible = !isVisible;
			return;
		}

		// Don't allow any input functionality when the inventory is not open
		if (!isVisible) {
			return;
		}

		if (selectedOption == -1) { // Input functionality for when the player has no item currently selected
									// Shifting up and down through the inventory screen
			if (keyUp) {
				curOption--;
				// Shifting the inventory's view up
				if (curOption < firstToDraw + (numToDraw / 2) - 1 && firstToDraw > 0) {
					firstToDraw--;
				}
				// Looping to the end of the inventory if the player presses up while on the first item
				if (curOption < 0) {
					curOption = INVENTORY_SIZE - 1;
					firstToDraw = curOption - numToDraw;
				}
			} else if (keyDown) {
				curOption++;
				// Shifting the inventory's view down
				if (curOption > firstToDraw + (numToDraw / 2) + 1 && firstToDraw < INVENTORY_SIZE - 1 - numToDraw) {
					firstToDraw++;
				}
				// Looping to the start of the inventory if the player presses down while on the last item
				if (curOption > INVENTORY_SIZE - 1) {
					curOption = 0;
					firstToDraw = 0;
				}
			}

			if (keySelect) {
				if (swappingItems) { // Swapping items when the player selects after enabling swapping
					if (slotToSwapTo != curOption) { // Make sure the player isn't swapping an item into the same spot it was in before
						SwapItems(slotToSwapTo, curOption);
						// Remove the current item from the swapping buffer, and reset all the variables to their original values
						swappingItems = false;
						slotToSwapTo = -1;
						var length = itemToSwap.Length;
						for (int i = 0; i < length; i++) {
							itemToSwap[i] = 0;
						}
					}
				} else {  // Selecting an item when the player doesn't have swapping enabled
					if (invItem[curOption, 0] > 0) {
						selectedOption = curOption;
					}
				}
			}

			if (keyReturn) {
				if (swappingItems) { // Disabling item swapping
					swappingItems = false;
					slotToSwapTo = -1;
					var length = itemToSwap.Length;
					for (int i = 0; i < length; i++) {
						itemToSwap[i] = 0;
					}
				} else {
					isVisible = false;
				}
			}
		} else { // Input functionality for when the player has selected an item (The option menu)
			if (!playerChooseWindow) {
				int menuLength = ItemOptions(invItem[curOption, 0], curOption).Count;
				// Shifting up and down through the sub-menu options
				if (keyUp) {
					subCurOption--;
					if (subCurOption < 0) {
						subCurOption = menuLength - 1;
					}
				} else if (keyDown) {
					subCurOption++;
					if (subCurOption > menuLength - 1) {
						subCurOption = 0;
					}
				}

				// Selecting one of the given options
				if (keySelect) {
					List<string> options = ItemOptions(invItem[curOption, 0], curOption);
					string[] option = options.ToArray();
					ItemOptionsFunction(invItem[curOption, 0], option[subCurOption]);
				}

				// Unselecting the current item, returning the player back to the main inventory window
				if (keyReturn) {
					selectedOption = -1;
				}
			} else { // Choosing which player will use the consumable
				if (keyUp) { // Moving up in the list of players
					curPlayerOption--;
					if (curPlayerOption < 0) {
						curPlayerOption = 3;
					}
				}

				if (keyDown) { // Moving down in the list of players
					curPlayerOption++;
					if (curPlayerOption > 3) {
						curPlayerOption = 0;
					}
				}

				// Using an item on the player that is currently highlighted
				if (keySelect) {
					// Equip the item only if it can be equipped
					if (canPlayerEquip[curPlayerOption]) {
						ItemUseFunction(invItem[curOption, 0], curOption, curPlayerOption);
						playerChooseWindow = false;
						curPlayerOption = 0;
						subCurOption = 0;
						selectedOption = -1;
					}
				}

				// Returning to the item's sub-menu, exiting out of the player selection menu
				if (keyReturn) {
					playerChooseWindow = false;
					curPlayerOption = 0;
				}
			}
		}
	}

	// Drawing the inventory to the screen
	private void OnGUI() {
		// Don't allow the inventory to be drawn when it isn't open
		if (!isVisible) {
			return;
		}

		// Creating the Font(s) for the Inventory
		GUIStyle style = new GUIStyle(GUI.skin.label) {
			font = GuiSmall,
			fontSize = 30,
		};
		var fontHeight = style.lineHeight;

		// Drawing the inventory items
		for (int i = firstToDraw; i <= firstToDraw + numToDraw; i++) {
			GUI.Label(new Rect(45.0f, 15.0f + (fontHeight * (i - firstToDraw)), 200.0f, 50.0f), ItemName(invItem[i, 0]), style);
			// Only show number of items beside items that can stack
			if (ItemStackLimit(invItem[i, 0]) > 1) { GUI.Label(new Rect(550.0f, 15.0f + (fontHeight * (i - firstToDraw)), 200.0f, fontHeight), "x" + invItem[i, 1], style); }
			// Drawing a cursor that points to the item the player has highlighted
			GUI.Label(new Rect(25.0f, 15.0f + (fontHeight * (curOption - firstToDraw)), 50.0f, 50.0f), ">", style);
			// Let the player know this item is equipped if it is
			if (invItem[i, 2] != -1) { GUI.Label(new Rect(510.0f, 15.0f + (fontHeight * (i - firstToDraw)), 50.0f, 50.0f), "(E)", style); }
		}

		// Drawing the item's description
		if (ItemType(invItem[curOption, 0]) == (int)ITEM_TYPE.EQUIPABLE) {
			string playerName = "N/A";
			if (invItem[curOption, 2] != -1) { playerName = "Player" + (invItem[curOption, 2] + 1); }
			GUI.Label(new Rect(25.0f, 15.0f + (fontHeight * numToDraw) + 35.0f, 600.0f, 300.0f), ItemDescription(invItem[curOption, 0]) + "\n\nEquipped By -- " + playerName, style);
		} else {
			GUI.Label(new Rect(25.0f, 15.0f + (fontHeight * numToDraw) + 35.0f, 600.0f, 300.0f), ItemDescription(invItem[curOption, 0]), style);
		}

		// Drawing the selected item's options to the screen
		if (selectedOption != -1) {
			List<string> options = ItemOptions(invItem[curOption, 0], curOption);
			string[] option = options.ToArray();
			var length = options.Count;
			for (int i = 0; i < length; i++) {
				GUI.Label(new Rect(620.0f, 15.0f + (fontHeight * i), 200.0f, 50.0f), option[i], style);
				if (subCurOption == i) { GUI.Label(new Rect(600.0f, 15.0f + (fontHeight * i), 50.0f, 50.0f), ">", style); }
			}
		}

		// Drawing the item that is currently being swapped
		if (swappingItems) {
			GUI.Label(new Rect(620.0f, 180.0f, 150.0f, 150.0f), "Item Held:\n" + ItemName(itemToSwap[0]) + "\nx" + itemToSwap[1], style);
		}

		// Drawing the player selection window options
		if (playerChooseWindow) {
			for (int i = 0; i < 4; i++) {
				GUI.Label(new Rect(770.0f, 15.0f + (fontHeight * i), 150.0f, 50.0f), "Player" + (i + 1), style);
				if (curPlayerOption == i) { GUI.Label(new Rect(750.0f, 15.0f + (fontHeight * i), 50.0f, 50.0f), ">", style); }
			}
		}
	}

	#region Item/Inventory Manipulation Scripts

	// Swaps two items within the inventory
	public void SwapItems(int slot1, int slot2) {
		if (slot1 != slot2) {
			int[] tempItem = { invItem[slot1, 0], invItem[slot1, 1], invItem[slot1, 2] };
			// Move the second item into the first item's slot
			invItem[slot1, 0] = invItem[slot2, 0];
			invItem[slot1, 1] = invItem[slot2, 1];
			invItem[slot1, 2] = invItem[slot2, 2];
			// Move the first item into the second item's slot
			invItem[slot2, 0] = tempItem[0];
			invItem[slot2, 1] = tempItem[1];
			invItem[slot2, 2] = tempItem[2];
		}
	}

	// Adds an item into an empty slot in the inventory. Can also add more items to a currently occupied slot if the item can do so
	public bool AddItem(int itemID, int numToAdd = 1) {
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			// If the slot is empty, add the item to the inventory
			// Also, if the item already has a stack, add these items to the stack as well
			if ((invItem[i, 0] == 0 || invItem[i, 0] == itemID) && numToAdd > 0) {
				int stackSize = ItemStackLimit(itemID);
				if (invItem[i, 1] + numToAdd <= stackSize) { // There is enough space in the stack for the newly found item
					invItem[i, 0] = itemID;
					invItem[i, 1] += numToAdd;
					numToAdd = 0;
				} else { // No more space in a stack, try to find an empty spot
					int remainder = stackSize - invItem[i, 1];
					invItem[i, 1] = stackSize;
					numToAdd -= remainder;
					// NOTE -- If the inventory cannot find a spot for the remaining items it will just discard them.
					// When this happens a message should be displayed for the player to let them know those items couldn't be picked up.
				}
				if (numToAdd == 0) { return true; }
			}
		}
		// Return false if the item could not be added, allowing for a prompt telling the user the item cannot be added to the inventory
		return false;
	}

	// Finds if an item exists in the inventory and returns the slot that the item is in
	// A side effect of this method of search is that it will find the first instance of the
	// item relative to the top of the inventory. So, the closest to the top will be prioritized
	public int FindItem(int itemID) {
		int slot = -1;

		// Search the entire inventory for the item
		for (int i = 0; i < INVENTORY_SIZE; i++) {
			// The item is found
			if (invItem[i, 0] == itemID) {
				slot = i;
				break;
			}
		}

		// Return either -1 (Default Value) or the item's slot
		return slot;
	}

	// Removes an item from the specified slot in the inventory, if 0 items remain after word, empty the slot completely
	public void RemoveItem(int slot, int numToRemove = 1) {
		invItem[slot, 1] -= numToRemove;
		Debug.Log("Number of Items: " + invItem[slot, 1].ToString() + ", Slot Value: " + slot.ToString());
		// Completely remove the item if all in the stack have been used
		if (invItem[slot, 1] <= 0) {
			invItem[slot, 0] = (int)ITEM_ID.NO_ITEM;
			// Remove the item from a player object if one had the dropped item equipped
			if (invItem[slot, 2] != -1) {
				UpdatePlayerStats(invItem[slot, 2], invItem[slot, 0], true);
			}
		}
	}

	#endregion

	#region Item Names

	// Returns the name of the specified item based on its ID
	public string ItemName(int itemID) {
		string name = "---";

		// Find the item's name based on its ID value
		switch (itemID) {
			case (int)ITEM_ID.TEST_POTION_HP:
				name = "Test Potion (HP)";
				break;
			case (int)ITEM_ID.TEST_POTION_MP:
				name = "Test Potion (MP)";
				break;
			case (int)ITEM_ID.TEST_QUEST_ITEM:
				name = "Test Quest Item";
				break;
			case (int)ITEM_ID.TEST_ARMOR1:
				name = "Test Leather Armor";
				break;
			case (int)ITEM_ID.TEST_WEAPON1:
				name = "Test Iron Sword";
				break;
            case (int)ITEM_ID.HP_POTION:
                name = "Potion";
                break;
            case (int)ITEM_ID.MP_ELIXER:
                name = "Elixer";
                break;
            case (int)ITEM_ID.HOPE_POTION:
                name = "Hope";
                break;

        }
		return name;
	}

	#endregion

	#region Item Descriptions

	// Returns the item's description based of the itemID specified
	public string ItemDescription(int itemID) {
		string description = "";

		// Find an item's description based on its ID
		switch (itemID) {
			case (int)ITEM_ID.TEST_POTION_HP:
				description = "There's like a 50% chance this will restore the player's HP.";
				break;
			case (int)ITEM_ID.TEST_POTION_MP:
				description = "I think it restores MP, but it might not.";
				break;
			case (int)ITEM_ID.TEST_QUEST_ITEM:
				description = "Some piece of junk. Go give it to someone.";
				break;
			case (int)ITEM_ID.TEST_ARMOR1:
				description = "A piece of leather armor to test the inventory with.";
				break;
			case (int)ITEM_ID.TEST_WEAPON1:
				description = "An Iron Sword used for testing the game's inventory.";
				break;
            case (int)ITEM_ID.HP_POTION:
                description = "A potion that restores 50 hit points for one ally.";
                break;
            case (int)ITEM_ID.MP_ELIXER:
                description = "An elixer that restores 50 mana points for one ally.";
                break;
            case (int)ITEM_ID.HOPE_POTION:
                description = "Revives an ally with and restores a 100 hit points";
                break;
        }

		return description;
	}

	#endregion

	#region Item Prices for Purchasing

	public int ItemPrice(int itemID) {
		int price = 0;

		// Find out the item's price based on the ID provided
		switch (itemID) {
			case (int)ITEM_ID.TEST_POTION_HP:
				price = 69;
				break;
			case (int)ITEM_ID.TEST_POTION_MP:
				price = 420;
				break;
			case (int)ITEM_ID.TEST_QUEST_ITEM:
				price = 666;
				break;
			case (int)ITEM_ID.TEST_ARMOR1:
				price = 6969;
				break;
			case (int)ITEM_ID.TEST_WEAPON1:
				price = 42069;
				break;
		}

		return price;
	}

	#endregion

	#region Item Classes for Merchants

	public int ItemClass(int itemID) {
		int itemClass = 0;

		// Find the item's class based on its ID
		switch (itemID) {
			case (int)ITEM_ID.TEST_POTION_HP:
			case (int)ITEM_ID.TEST_POTION_MP:
				itemClass = (int)ITEM_CLASS.POTIONS;
				break;
			case (int)ITEM_ID.TEST_QUEST_ITEM:
				itemClass = (int)ITEM_CLASS.UNSELLABLE;
				break;
			case (int)ITEM_ID.TEST_ARMOR1:
				itemClass = (int)ITEM_CLASS.ARMOR;
				break;
			case (int)ITEM_ID.TEST_WEAPON1:
				itemClass = (int)ITEM_CLASS.WEAPON;
				break;
		}

		return itemClass;
	}

	#endregion

	#region Item Options and Their Functionality

	// Returns a full list of options that an item can have based on its type
	public List<string> ItemOptions(int itemID, int slotID) {
		List<string> options = new List<string>();
		int itemType = ItemType(itemID);

		// Find out the options avaiable to a player bassed on the selected item's ID
		switch (itemType) {
			case (int)ITEM_TYPE.CONSUMABLE:
				options.Add("Use");
				options.Add("Move");
				options.Add("Drop");
				break;
			case (int)ITEM_TYPE.KEY_ITEM:
				options.Add("Use");
				options.Add("Move");
				break;
			case (int)ITEM_TYPE.EQUIPABLE:
				if (invItem[slotID, 2] == -1) { // Shot "Equip" if nobody has the item equipped
					options.Add("Equip");
				} else { // Show "Unequip" if the item is equipped by a player already
					options.Add("Unequip");
					///print(curOption + " : " + (ITEM_ID)itemID + " Equipped by: " + invItem[curOption, 2]);
				}
				options.Add("Move");
				options.Add("Drop");
				break;
		}

		return options;
	}

	// Executes code based upon what option was selected by the user. These include options like equipping, unequipping, switching, dropping, etc.
	public void ItemOptionsFunction(int itemID, string option) {
		int itemType = ItemType(itemID);

		// Starting up the item swapping
		if (option.Equals("Switch")) {
			if (!swappingItems) {
				swappingItems = true;
				slotToSwapTo = curOption;
				var length = itemToSwap.Length;
				for (int i = 0; i < length; i++) {
					itemToSwap[i] = invItem[curOption, i];
				}
			}
			subCurOption = 0;
			selectedOption = -1;
			return;
		}

		// Removing an item and/or its entire stack from the inventory
		if (option.Equals("Drop")) {
			RemoveItem(curOption, invItem[curOption, 1]);
			subCurOption = 0;
			selectedOption = -1;
			return;
		}

		// Using a consumable item or equipping a piece of armor or weapon
		if (option.Equals("Use") || option.Equals("Equip")) {
			if (ItemType(invItem[curOption, 0]) != (int)ITEM_TYPE.KEY_ITEM) {
				// Determine which players can equip the current item
				if (ItemType(invItem[curOption, 0]) == (int)ITEM_TYPE.EQUIPABLE) {
					float[] itemStats = ItemStats(invItem[curOption, 0]);
					// Loop through the items that the selected player can equip
					for (int i = 0; i < 4; i++) {
						var length = PartyStats.chara[i].validEquipables.Length;
						for (int ii = 0; ii < length; ii++) {
							if (PartyStats.chara[i].validEquipables[ii] == itemStats[7]) { // Item is valid, selected player can equip it
								canPlayerEquip[i] = true;
								ii = length; // Exit the loop
							} else { // Item is not valid, selected player cannot equip it
								canPlayerEquip[i] = false;
							}
						}
					}
				}
				// Open up the player choose window
				playerChooseWindow = true;
			} else {
				// TODO -- Add code for turning in quest items
			}
			return;
		}

		// Unequipping a piece of armor or a weapon from a player
		if (option.Equals("Unequip")) {
			UpdatePlayerStats(invItem[curOption, 2], itemID, true);
			selectedOption = -1;
			return;
		}
	}

	// A method that holds all the functionality for every item in the game. If the item slot is empty after use, delete the item.
	// This method will not equip or unequip any item, but it will provide functionality for CONSUMABLE items.
	public void ItemUseFunction(int itemID, int slotID, int playerID) {
		int itemType = ItemType(itemID);
		bool isEquipped = false;
		// Only check if an item is equipped or not if the item CAN BE EQUIPPED (Ex. Weapons and Armor)
		if (itemType == (int)ITEM_TYPE.EQUIPABLE) {
			if (invItem[slotID, 2] == playerID) {
				isEquipped = true;
			}
		}

		// Check which functionality to use based on the itemID provided
		switch (itemID) {
			case (int)ITEM_ID.HP_POTION:
				itemAddAmount = 50;
				UpdatePlayerHitpoints(itemAddAmount, playerID);
				break;
			case (int)ITEM_ID.MP_ELIXER:
				itemAddAmount = 50;
				UpdatePlayerMagicpoints(itemAddAmount, playerID);
				break;
            case (int)ITEM_ID.HOPE_POTION:
                itemAddAmount = 100;
                UpdatePlayerHitpoints(itemAddAmount, playerID);
                break;
            case (int)ITEM_ID.TEST_ARMOR1:
			case (int)ITEM_ID.TEST_WEAPON1:
				UpdatePlayerStats(playerID, itemID, isEquipped);
				break;
		}

		// Remove the item (Or one from the stack) if it was consumed by the player
		if (itemType == (int)ITEM_TYPE.CONSUMABLE) {
			RemoveItem(slotID);
		}
	}

	#endregion

	#region An item's "Type", which determines funcaitonality

	// Returns the "Type" of the item based on the itemID. This is used to determined what options the player can use in tandem with the item
	public int ItemType(int itemID) {
		int itemType = (int)ITEM_TYPE.CONSUMABLE;

		// Search for the item's type based on its ID
		switch (itemID) {
			case (int)ITEM_ID.TEST_POTION_HP:
			case (int)ITEM_ID.TEST_POTION_MP:
            case (int)ITEM_ID.HP_POTION:
            case (int)ITEM_ID.MP_ELIXER:
            case (int)ITEM_ID.HOPE_POTION:
                itemType = (int)ITEM_TYPE.CONSUMABLE;
				break;
			case (int)ITEM_ID.TEST_QUEST_ITEM:
				itemType = (int)ITEM_TYPE.KEY_ITEM;
				break;
			case (int)ITEM_ID.TEST_WEAPON1:
			case (int)ITEM_ID.TEST_ARMOR1:
				itemType = (int)ITEM_TYPE.EQUIPABLE;
				break;
            default:
                itemType = (int)ITEM_TYPE.EQUIPABLE;
                break;
		}

		return itemType;
	}

    #endregion

    #region An item's "Icon"
    public Sprite ItemIcon(int itemID)
    {
        Sprite icon = itemIcons[0]; //No Item is at 0

        // Search for the item's type based on its ID
        switch (itemID)
        {
            case (int)ITEM_ID.TEST_POTION_HP:
            case (int)ITEM_ID.HP_POTION:
                icon = itemIcons[1];//Potion is 1
                break;
            case (int)ITEM_ID.TEST_POTION_MP:
            case (int)ITEM_ID.MP_ELIXER:
                icon = itemIcons[2]; //Elixer is 2
                break;
            case (int)ITEM_ID.HOPE_POTION:
                icon = itemIcons[3]; //Hope is 3
                break;
            case (int)ITEM_ID.TEST_ARMOR1:
                icon = itemIcons[4];
                break;
            case (int)ITEM_ID.TEST_WEAPON1:
                icon = itemIcons[5];
                break;
            default:
                icon = itemIcons[0];
                break;
        }

        return icon;
    }

    #endregion
    #region Stack Limit of Items in a Single Inventory Space

    // Returns the maximum stack limit for an item given the itemID
    public int ItemStackLimit(int itemID) {
		int stackSize = 1; // Default stack limit is 1

		// Find out an item's max stack size based on its ID
		switch (itemID) {
			case (int)ITEM_ID.TEST_POTION_HP:
            case (int)ITEM_ID.HP_POTION:
				stackSize = 20;
				break;
			case (int)ITEM_ID.TEST_POTION_MP:
            case (int)ITEM_ID.MP_ELIXER:
            case (int)ITEM_ID.HOPE_POTION:
                stackSize = 10;
				break;
        }

		return stackSize;
	}

	#endregion

	#region Weapon, Armor, and Accessory Statistics 

	// Holds stats for every single weapon and piece of armor in the game
	// If no values have been set, the item's stats will be defaulted to 0
	public float[] ItemStats(int itemID) {
		float[] stat = { 0, 0, 0, 0, 0, 0, 0, 0 };
		// NOTE -- Element 0 is the item's attack-buff property
		//         Element 1 is the item's defense-buff property
		//         Element 2 is the item's strength-buff property
		//         Element 3 is the item's agility-buff property
		//         Element 4 is the item's critical-buff property
		//         Element 5 is the item's hitpoint-buff property
		//         Element 6 is the item's magicpoint-buff property
		//		   Element 7 is the item's class property

		// Find out an item's stats based on its ID
		switch (itemID) {
			case (int)ITEM_ID.TEST_ARMOR1:
				stat[1] = 5;
				stat[5] = 10;
				stat[6] = 2;
				stat[7] = (float)ARMOR_TYPE.LEATHER;
				break;
			case (int)ITEM_ID.TEST_WEAPON1:
				stat[0] = 8;
				stat[4] = 2;
				stat[7] = (float)WEAPON_TYPE.SWORD;
				break;
		}

		return stat;
	}

	#endregion

	#region Methods for Updating Player Statistics

	// Updates the player's HP when a health potion or similar is used in the inventory
	private void UpdatePlayerHitpoints(int amount, int playerID) {
		PartyStats.chara[playerID].hitpoints += amount;
		PartyStats.chara[playerID].rage -= amount * 2.0f;
		// Lower the player's rage value based on their health recovered doubled
		if (PartyStats.chara[playerID].rage < 0.0f) {
			PartyStats.chara[playerID].rage = 0.0f;
		}
		// Make sure the hitpoints don't go below zero or above the player's maximum HP
		if (PartyStats.chara[playerID].hitpoints > PartyStats.chara[playerID].TotalMaxHealth) {
			PartyStats.chara[playerID].hitpoints = PartyStats.chara[playerID].TotalMaxHealth;
			PartyStats.chara[playerID].rage = 0.0f; // If health is full, then rage is zero
		} else if (PartyStats.chara[playerID].hitpoints < 0) {
			PartyStats.chara[playerID].hitpoints = 0;
		}
		Debug.Log("\nHealth: " + PartyStats.chara[playerID].hitpoints + "/" + PartyStats.chara[playerID].TotalMaxHealth);
	}

	// Updates the player's MP when a mana potion or similar is used in the inventory
	private void UpdatePlayerMagicpoints(int amount, int playerID) {
		PartyStats.chara[playerID].magicpoints += amount;
		// Make sure the magicpoints don't go below zero or above the player's maximum MP
		if (PartyStats.chara[playerID].magicpoints > PartyStats.chara[playerID].TotalMaxMana) {
			PartyStats.chara[playerID].magicpoints = PartyStats.chara[playerID].TotalMaxMana;
		} else if (PartyStats.chara[playerID].magicpoints < 0) {
			PartyStats.chara[playerID].magicpoints = 0;
		}
		Debug.Log("\nMana: " + PartyStats.chara[playerID].magicpoints + "/" + PartyStats.chara[playerID].TotalMaxMana);
	}

	// Updates player stats whenever an item like a piece of armor or weapon is equipped or unequipped
	private void UpdatePlayerStats(int playerID, int itemID, bool isEquipped) {
		// This array will ignore the final element in the array (The weapon's class)
		float[] itemStats = ItemStats(itemID);
		if (!isEquipped) { // Equipping the item onto the specified player
						   // Add the item to the current player
			PartyStats.chara[playerID].attackMod += itemStats[0];
			PartyStats.chara[playerID].defenceMod += itemStats[1];
			PartyStats.chara[playerID].strengthMod += itemStats[2];
			PartyStats.chara[playerID].agilityMod += itemStats[3];
			PartyStats.chara[playerID].criticalMod += itemStats[4];
			PartyStats.chara[playerID].maxHealthMod += itemStats[5];
			PartyStats.chara[playerID].maxManaMod += itemStats[6];
			// Remove this item from another player if it is equipped to them
			if (invItem[curOption, 2] != -1) {
				int oPlayerID = invItem[curOption, 2];
				PartyStats.chara[oPlayerID].attackMod -= itemStats[0];
				PartyStats.chara[oPlayerID].defenceMod -= itemStats[1];
				PartyStats.chara[oPlayerID].strengthMod -= itemStats[2];
				PartyStats.chara[oPlayerID].agilityMod -= itemStats[3];
				PartyStats.chara[oPlayerID].criticalMod -= itemStats[4];
				PartyStats.chara[oPlayerID].maxHealthMod -= itemStats[5];
				PartyStats.chara[oPlayerID].maxManaMod -= itemStats[6];
			}
			// Set the inventory item's "Who's Equipped This" element to the current player's ID
			invItem[curOption, 2] = playerID;
		} else { // Unequipping the item from the specified player
			PartyStats.chara[playerID].attackMod -= itemStats[0];
			PartyStats.chara[playerID].defenceMod -= itemStats[1];
			PartyStats.chara[playerID].strengthMod -= itemStats[2];
			PartyStats.chara[playerID].agilityMod -= itemStats[3];
			PartyStats.chara[playerID].criticalMod -= itemStats[4];
			PartyStats.chara[playerID].maxHealthMod -= itemStats[5];
			PartyStats.chara[playerID].maxManaMod -= itemStats[6];
			// Tell the inventory that the item isn't equipped by anybody anymore
			invItem[curOption, 2] = -1;
		}
		Debug.Log("\nAttack: " + PartyStats.chara[playerID].TotalAttack + " Defence: " + PartyStats.chara[playerID].TotalDefence + " Strength: " + PartyStats.chara[playerID].TotalStrength + " Agility: " + PartyStats.chara[playerID].TotalAgility + " Critical: " + PartyStats.chara[playerID].TotalCritical + " Maximum Health: " + PartyStats.chara[playerID].TotalMaxHealth + " Maximum Mana: " + PartyStats.chara[playerID].TotalMaxMana);
	}

	#endregion
}