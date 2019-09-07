using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        September 6th, 2019

public class MainInventory : MonoBehaviour {
    public static MainInventory invInstance;    // Holds the current inventory instance in a single variable
    public static int INVENTORY_SIZE = 60;      // The maximum size of the inventory
    public int[,] invItem = new int[INVENTORY_SIZE, 3];
    // NOTE -- Element 1 is the item's ID value that will point to its name, description, icon, etc.
    //         Element 2 is how many items currently occupy the slot in the inventory
    //         Element 3 is what character has this item equipped (Ex. armor and weapons)

    // Set the main inventory instance to this one if no inventory is active, delete otherwise
    public void Awake() {
        if (invInstance == null) {
            invInstance = this;
        } else {
            Destroy(gameObject);
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
                if (numToAdd <= stackSize) { // There is enough space in the stack for the newly found item
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

    // Removes an item from the specified slot in the inventory, if 0 items remain after word, empty the slot completely
    public void RemoveItem(int slot, int numToRemove = 1) {
        if (numToRemove >= invItem[slot, 1]) {
            invItem[slot, 1] -= numToRemove;
            // Completely remove the item if all in the stack have been used
            if (invItem[slot, 1] <= 0) {
                invItem[slot, 0] = 0;
                // Chekc if the item needs to be unequipped from a character
                if (invItem[slot, 2] != 0) {
                    invItem[slot, 2] = 0;
                    // TODO -- Add a call to remove the item off a character if the item is equipped by one of them
                }
            }
        }
    }

    // All Methods Below Hold Important Information About Every Item (Name, Description, Function, etc.) //////////////////////////////////

    // Returns the name of the specified item based on its ID
    public string ItemName(int itemID) {
        string name = "";

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
                name = "Test Armor";
                break;
            case (int)ITEM_ID.TEST_WEAPON1:
                name = "Test Weapon";
                break;
        }
        return name;
    }

    // Returns the item's description based of the itemID specified
    public string ItemDescription(int itemID) {
        string description = "";

        switch (itemID) {
            case (int)ITEM_ID.TEST1:
                description = "Test_Description_1";
                break;
            case (int)ITEM_ID.TEST2:
                description = "Test_Description_2";
                break;
            case (int)ITEM_ID.TEST3:
                description = "Test_Description_3";
                break;
        }

        return description;
    }

    // Returns a full list of options that an item can have based on its type
    public List<string> ItemOptions(int itemID) {
        List<string> options = new List<string>();
        int itemType = ItemType(itemID);

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
        }

        return options;
    }

    // Executes code based upon what option was selected by the user. These include options like equipping, unequipping, switching, dropping, etc.
    public void ItemOptionsFunction(string option) {
        // Provide use cases for every item based on their item stats
    }

    // Returns the "Type" of the item based on the itemID. This is used to determined what options the player can use in tandem with the item
    public int ItemType(int itemID) {
        int itemType = 0;

        switch (itemID) {
            case (int)ITEM_ID.TEST1:
            case (int)ITEM_ID.TEST2:
                itemType = (int)ITEM_TYPE.CONSUMABLE;
                break;
            case (int)ITEM_ID.TEST3:
                itemType = (int)ITEM_TYPE.KEY_ITEM;
                break;
        }

        return itemType;
    }

    // Returns the maximum stack limit for an item given the itemID
    public int ItemStackLimit(int itemID) {
        int stackSize = 1; // Default stack limit is 1

        switch (itemID) {
            case (int)ITEM_ID.TEST1:
                stackSize = 20;
                break;
            case (int)ITEM_ID.TEST2:
                stackSize = 10;
                break;
        }

        return stackSize;
    }
}