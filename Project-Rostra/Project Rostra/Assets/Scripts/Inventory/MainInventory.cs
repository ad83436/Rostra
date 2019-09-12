using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        September 11th, 2019

public class MainInventory : MonoBehaviour {
    public static MainInventory invInstance;    // Holds the current inventory instance in a single variable
    public static int INVENTORY_SIZE = 60;      // The maximum size of the inventory
    public int[,] invItem = new int[INVENTORY_SIZE, 3];
    // NOTE -- Element 0 is the item's ID value that will point to its name, description, icon, etc.
    //         Element 1 is how many items currently occupy the slot in the inventory
    //         Element 2 is what character has this item equipped (Ex. armor and weapons)

    // The fonts that are used for drawing the Inventory's GUI
    public Font invGUIsmall;

    // Variables for holding a to-be-swapped item in the inventory
    private int[] itemToSwap = new int[3];      // Holds data about the item being swapped in the inventory
    private int slotToSwapTo = -1;              // The slot the the selected item will send the other item to
    private bool swappingItems = false;         // If true, the inventory will be in an "Item Swap" state. Meaning, no items can be selected until the swap is declined or completed

    // Variables for selecting options and inventory items
    private int curOption = 0;                  // The current inventory item the player has their cursor over
    private int selectedOption = -1;            // The item that the player has selected in the inventory
    private int subCurOption = 0;               // The current option the player has their cursor over after selecting an item

    // Variables for selecting the player to use a consumable item
    private bool playerChooseWindow = false;    // If true, the inventory needs a player to be chosen (For using consumable items)
    private int curPlayerOption = 0;            // Which player is being chosen to use the given item

    // Variables for drawing the inventory to the screen
    private int firstToDraw = 0;                // The first item from the inventory array to draw out of the full inventory
    private int numToDraw = 15;                 // How many inventory items that can be visible to the player at any given time

    // Set the main inventory instance to this one if no inventory is active, delete otherwise
    public void Awake() {
        if (invInstance == null) {
            invInstance = this;
        } else {
            Destroy(gameObject);
        }
        // Set all 3rd elements in the array to -1 (Equipped by nobody) 
        for (int i = 0; i < INVENTORY_SIZE; i++) {
            invItem[i, 2] = -1;
        }
    }

    // FOR TESTING
    private void Start() {
        invItem[0, 0] = (int)ITEM_ID.TEST_QUEST_ITEM;
        invItem[0, 1] = ItemStackLimit((int)ITEM_ID.TEST_QUEST_ITEM);

        invItem[1, 0] = (int)ITEM_ID.TEST_POTION_HP;
        invItem[1, 1] = ItemStackLimit((int)ITEM_ID.TEST_POTION_HP);

        invItem[2, 0] = (int)ITEM_ID.TEST_POTION_MP;
        invItem[2, 1] = ItemStackLimit((int)ITEM_ID.TEST_POTION_MP);
    }

    // Handling keyboard functionality
    private void Update() {
        // Getting Keyboard Input
        bool keySelect, keyReturn, keyUp, keyDown;
        keySelect = Input.GetKeyDown(KeyCode.Z);
        keyReturn = Input.GetKeyDown(KeyCode.X);
        keyUp = Input.GetKeyDown(KeyCode.UpArrow);
        keyDown = Input.GetKeyDown(KeyCode.DownArrow);

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
            }
            if (keyDown) {
                curOption++;
                // Shifting the inventory's view up
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
                    // TODO -- Make this block of code exit out of the inventory
                }
            }
        } else { // Input functionality for when the player has selected an item (The option menu)
            if (!playerChooseWindow) {
                int menuLength = ItemOptions(invItem[curOption, 0]).Count;
                // Shifting up and down through the sub-menu options
                if (keyUp) {
                    subCurOption--;
                    if (subCurOption < 0) {
                        subCurOption = menuLength - 1;
                    }
                }
                if (keyDown) {
                    subCurOption++;
                    if (subCurOption > menuLength - 1) {
                        subCurOption = 0;
                    }
                }

                // Selecting one of the given options
                if (keySelect) {
                    List<string> options = ItemOptions(invItem[curOption, 0]);
                    string[] option = options.ToArray();
                    ItemOptionsFunction(invItem[curOption, 0], option[subCurOption]);
                    subCurOption = 0;
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
                    ItemUseFunction(invItem[curOption, 0], curPlayerOption);
                    playerChooseWindow = false;
                    selectedOption = -1;
                }

                // Returning to the item's sub-menu, exiting out of the player selection menu
                if (keyReturn) {
                    playerChooseWindow = false;
                }
            }
        }
    }

    // Drawing the inventory to the screen
    private void OnGUI() {
        GUIStyle style = new GUIStyle(GUI.skin.label) {
            font = invGUIsmall,
            fontSize = 20,
        };

        // Drawing the inventory items
        for (int i = firstToDraw; i <= firstToDraw + numToDraw; i++) {
            GUI.Label(new Rect(25.0f, 5.0f + (15.0f * (i - firstToDraw)), 200.0f, 25.0f), ItemName(invItem[i, 0]), style);
            GUI.Label(new Rect(200.0f, 5.0f + (15.0f * (i - firstToDraw)), 200.0f, 25.0f), "x" + invItem[i, 1], style);
            // Drawing a cursor that points to the item the player has highlighted
            GUI.Label(new Rect(5.0f, 5.0f + (15.0f * (curOption - firstToDraw)), 25.0f, 25.0f), ">", style);
        }
        // Drawing the item's description
        GUI.Label(new Rect(5.0f, 260.0f, 300.0f, 150.0f), ItemDescription(invItem[curOption, 0]), style);

        // Drawing the selected item's options to the screen
        if (selectedOption != -1) {
            List<string> options = ItemOptions(invItem[curOption, 0]);
            string[] option = options.ToArray();
            var length = options.Count;
            for (int i = 0; i < length; i++) {
                GUI.Label(new Rect(250.0f, 5.0f + (15.0f * i), 200.0f, 25.0f), option[i], style);
                if (subCurOption == i) { GUI.Label(new Rect(230.0f, 5.0f + (15.0f * i), 25.0f, 25.0f), ">", style); }
            }
        }

        // Drawing the item that is currently being swapped
        if (swappingItems) {
            GUI.Label(new Rect(250.0f, 90.0f, 150.0f, 150.0f), "Item Held:\n" + ItemName(itemToSwap[0]) + "\nx" + itemToSwap[1], style);
        }

        // Drawing the player selection window options
        if (playerChooseWindow) {
            // TEMPORARY CODE
            GUI.Label(new Rect(320.0f, 5.0f, 150.0f, 150.0f), "Player1\nPlayer2\nPlayer3\nPlayer4", style);
            for (int i = 0; i < 4; i++) {
                if (curPlayerOption == i) { GUI.Label(new Rect(300.0f, 5.0f + (15.0f * i), 25.0f, 25.0f), ">", style); }
            }
        }
    }

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
       // Completely remove the item if all in the stack have been used
       if (invItem[slot, 1] <= 0) {
            invItem[slot, 0] = (int)ITEM_ID.NO_ITEM;
            // Remove the item from a player object if one had the dropped item equipped
            if (invItem[slot, 2] != -1) {
                UpdatePlayerStats(invItem[slot, 2], invItem[slot, 0], true);
            }
        }
    }

    // All Methods Below Hold Important Information About Every Item (Name, Description, Function, etc.) //////////////////////////////////

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
        }
        return name;
    }

    // Returns the item's description based of the itemID specified
    public string ItemDescription(int itemID) {
        string description = "---";

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
        }

        return description;
    }

    // Returns a full list of options that an item can have based on its type
    public List<string> ItemOptions(int itemID) {
        List<string> options = new List<string>();
        int itemType = ItemType(itemID);

        // Find out the options avaiable to a player bassed on the selected item's ID
        switch (itemType) {
            case (int)ITEM_TYPE.CONSUMABLE:
                options.Add("Use");
                options.Add("Switch");
                options.Add("Drop");
                break;
            case (int)ITEM_TYPE.KEY_ITEM:
                options.Add("Use");
                options.Add("Switch");
                break;
            case (int)ITEM_TYPE.EQUIPABLE:
                if (invItem[curOption, 2] == -1) { // Shot "Equip" if nobody has the item equipped
                    options.Add("Equip");
                } else { // Show "Unequip" if the item is equipped by a player already
                    options.Add("Unequip");
                }
                options.Add("Switch");
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
            return;
        }

        // Removing an item and/or its entire stack from the inventory
        if (option.Equals("Drop")) {
            RemoveItem(curOption, invItem[curOption, 1]);
            return;
        }

        if (itemType == (int)ITEM_TYPE.EQUIPABLE) {
            if (option.Equals("Equip") || option.Equals("Unequip")) {

                return;
            }
        }

        // Using a CONSUMABLE item (Not the same as equipping/unequipping a weapon or piece of armor)
        if (itemType == (int)ITEM_TYPE.CONSUMABLE) {
            if (option.Equals("Use")) {
                // Open prompt for choosing which player to use the consumable on
                playerChooseWindow = true;
                return;
            }
        }
    }

    // A method that holds all the functionality for every item in the game. If the item slot is empty after use, delete the item.
    // This method will not equip or unequip any item, but it will provide functionality for CONSUMABLE items.
    public void ItemUseFunction(int itemID, int playerID) {
        int itemType = ItemType(itemID);
        bool isEquipped = false;
        // Only check if an item is equipped or not if the item CAN BE EQUIPPED (Ex. Weapons and Armor)
        if (itemType == (int)ITEM_TYPE.EQUIPABLE) {
            if (invItem[curOption, 2] == playerID) {
                isEquipped = true;
            }
        }

        // Check which functionality to use based on the itemID provided
        switch (itemID) {
            case (int)ITEM_ID.TEST_POTION_HP:
                UpdatePlayerHitpoints(10, playerID);
                break;
            case (int)ITEM_ID.TEST_POTION_MP:
                UpdatePlayerMagicpoints(20, playerID);
                break;
            case (int)ITEM_ID.TEST_ARMOR1:
            case (int)ITEM_ID.TEST_WEAPON1:
                UpdatePlayerStats(playerID, itemID, isEquipped);
                break;
        }

        // Remove the item (Or one from the stack) if it was consumed by the player
        if (itemType == (int)ITEM_TYPE.CONSUMABLE) {
            RemoveItem(invItem[curOption, 0]);
        }
    }

    // Returns the "Type" of the item based on the itemID. This is used to determined what options the player can use in tandem with the item
    public int ItemType(int itemID) {
        int itemType = (int)ITEM_TYPE.CONSUMABLE;

        // Search for the item's type based on its ID
        switch (itemID) {
            case (int)ITEM_ID.TEST_POTION_HP:
            case (int)ITEM_ID.TEST_POTION_MP:
                itemType = (int)ITEM_TYPE.CONSUMABLE;
                break;
            case (int)ITEM_ID.TEST_QUEST_ITEM:
                itemType = (int)ITEM_TYPE.KEY_ITEM;
                break;
            case (int)ITEM_ID.TEST_WEAPON1:
            case (int)ITEM_ID.TEST_ARMOR1:
                itemType = (int)ITEM_TYPE.EQUIPABLE;
                break;
        }

        return itemType;
    }

    // Returns the maximum stack limit for an item given the itemID
    public int ItemStackLimit(int itemID) {
        int stackSize = 1; // Default stack limit is 1

        // Find out an item's max stack size based on its ID
        switch (itemID) {
            case (int)ITEM_ID.TEST_POTION_HP:
                stackSize = 20;
                break;
            case (int)ITEM_ID.TEST_POTION_MP:
                stackSize = 10;
                break;
        }

        return stackSize;
    }

    // Holds stats for every single weapon and piece of armor in the game
    // If no values have been set, the item's stats will be defaulted to 0
    public int[] ItemStats(int itemID) {
        int[] stat = { 0, 0, 0, 0, 0, 0, 0 };
        // NOTE -- Element 0 is the item's attack-buff property
        //         Element 1 is the item's defense-buff property
        //         Element 2 is the item's strength-buff property
        //         Element 3 is the item's agility-buff property
        //         Element 4 is the item's critical-buff property
        //         Element 5 is the item's hitpoint-buff property
        //         Element 6 is the item's magicpoint-buff property

        // Find out an item's stats based on its ID
        switch (itemID) {
            case (int)ITEM_ID.TEST_ARMOR1:
                stat[1] = 5;
                stat[5] = 10;
                stat[6] = 2;
                break;
            case (int)ITEM_ID.TEST_WEAPON1:
                stat[0] = 8;
                stat[4] = 2;
                break;
        }

        return stat;
    }

    // Updates the player's HP when a health potion or similar is used in the inventory
    private void UpdatePlayerHitpoints(int amount, int playerID) {
        CharacterStats player = PartyStats.chara[playerID];
        player.hitpoints += amount;
        // Make sure the hitpoints don't go below zero or above the player's maximum HP
        if (player.hitpoints > player.TotalMaxHealth) {
            player.hitpoints = player.TotalMaxHealth;
        } else if (player.hitpoints < 0) {
            player.hitpoints = 0;
        }
    } 

    // Updates the player's MP when a mana potion or similar is used in the inventory
    private void UpdatePlayerMagicpoints(int amount, int playerID) {
        CharacterStats player = PartyStats.chara[playerID];
        player.magicpoints += amount;
        // Make sure the magicpoints don't go below zero or above the player's maximum MP
        if (player.magicpoints > player.TotalMaxMana) {
            player.magicpoints = player.TotalMaxMana;
        } else if (player.magicpoints < 0) {
            player.magicpoints = 0;
        }
    }

    // Updates player stats whenever an item like a piece of armor or weapon is equipped or unequipped
    private void UpdatePlayerStats(int playerID, int itemID, bool isEquipped) {
        CharacterStats player = PartyStats.chara[playerID];
        int[] itemStats = ItemStats(itemID);

        if (!isEquipped) { // Equipping the item onto the specified player
            // Add the item to the current player
            player.attackMod += itemStats[0];
            player.defenceMod += itemStats[1];
            player.strengthMod += itemStats[2];
            player.agilityMod += itemStats[3];
            player.criticalMod += itemStats[4];
            player.maxHealthMod += itemStats[5];
            player.maxManaMod += itemStats[6];
            // Remove this item from another player if it is equipped to them
            if (invItem[curOption, 2] != -1) {
                CharacterStats oPlayer = PartyStats.chara[invItem[curOption, 2]];
                oPlayer.attackMod -= itemStats[0];
                oPlayer.defenceMod -= itemStats[1];
                oPlayer.strengthMod -= itemStats[2];
                oPlayer.agilityMod -= itemStats[3];
                oPlayer.criticalMod -= itemStats[4];
                oPlayer.maxHealthMod -= itemStats[5];
                oPlayer.maxManaMod -= itemStats[6];
            }
            // Set the inventory item's "Who's Equipped This" elements to the current player's ID
            invItem[curOption, 2] = playerID;
        } else { // Unequipping the item from the specified player
            player.attackMod -= itemStats[0];
            player.defenceMod -= itemStats[1];
            player.strengthMod -= itemStats[2];
            player.agilityMod -= itemStats[3];
            player.criticalMod -= itemStats[4];
            player.maxHealthMod -= itemStats[5];
            player.maxManaMod -= itemStats[6];
            // Tell the inventory that the item isn't equipped by anybody anymore
            invItem[curOption, 2] = -1;
        }
    }
}