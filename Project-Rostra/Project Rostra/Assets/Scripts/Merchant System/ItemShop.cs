using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        October 10th, 2019

public class ItemShop : MonoBehaviour {
    public List<int> shopItems = new List<int>();   // The list of items that the shopkeeper has on them
    public bool isSelling = false;                  // If true, the user will be selling items from their inventory

    // The variables that are used for drawing the GUI to the screen
    public Font GuiSmall;

    private Merchant merchant;                      // The current merchant that owns this store

    private int curOption = 0;                      // The current shop item that is being looked at by the player
    private int selectedOption = -1;                // The option that the user selects to purchase
    private int firstToDraw = 0;                    // The top-most element that is being draw in the list of visible items
    private int numToDraw = 8;                      // The number of shop items that are visible to the player at any given time

    public ItemShop(Merchant merchant) {
        this.merchant = merchant;
    }

    private void Start() {
        shopItems.Add((int)ITEM_ID.TEST_POTION_HP);
        shopItems.Add((int)ITEM_ID.TEST_POTION_MP);
        shopItems.Add((int)ITEM_ID.TEST_ARMOR1);
        shopItems.Add((int)ITEM_ID.TEST_WEAPON1);
    }

    // Handles input and functionality for the menu (For testing purposes)
    private void Update() {
        if (MainInventory.invInstance.isVisible) {
            return;
        }

        // Getting Keyboard Input
        bool keyDown, keyUp, keySelect, keyReturn;
        keyDown = Input.GetKeyDown(KeyCode.DownArrow);
        keyUp = Input.GetKeyDown(KeyCode.UpArrow);
        keySelect = Input.GetKeyDown(KeyCode.Z);
        keyReturn = Input.GetKeyDown(KeyCode.X);

        //if (selectedOption == -1) {
            // Shifting up and down through the item shop screen
            if (keyUp) {
                curOption--;
                // Shifting the item shop's view up
                if (curOption < firstToDraw + (numToDraw / 2) - 1 && firstToDraw > 0) {
                    firstToDraw--;
                }
                // Looping to the end of the item shop list if the player presses up while on the first item
                if (curOption < 0) {
                    curOption = shopItems.Count - 1;
                    firstToDraw = curOption - numToDraw;
                }
            } else if (keyDown) {
                curOption++;
                // Shifting the item shop's view down
                if (curOption > firstToDraw + (numToDraw / 2) + 1 && firstToDraw < shopItems.Count - 1 - numToDraw) {
                    firstToDraw++;
                }
                // Looping to the start of the item shop list if the player presses down while on the last item
                if (curOption > shopItems.Count - 1) {
                    curOption = 0;
                    firstToDraw = 0;
                }
            }

            // Seleccting an option in the shop list
            if (keySelect) {
                int[] items = shopItems.ToArray();
                if (!BuyItem(items[curOption])) {
                    Debug.Log("Cannot Purchase -- insufficent funds");
                } else {
                    Debug.Log("Item Purchased -- " + items[curOption]);
                }
            }
        //}
    }

    // Drawing the Item Shop in code for testing
    private void OnGUI() {
        if (MainInventory.invInstance.isVisible) {
            return;
        }

        // Creating the Font(s) for the Inventory
        GUIStyle style = new GUIStyle(GUI.skin.label) {
            font = GuiSmall,
            fontSize = 30,
        };
        var fontHeight = style.lineHeight;

        // Drawing the inventory items
        for (int i = 0; i <= (shopItems.Count - 1); i++) {
            int[] items = shopItems.ToArray();
            GUI.Label(new Rect(1205.0f, 15.0f + (fontHeight * (i)), 200.0f, 50.0f), MainInventory.invInstance.ItemName(items[i]), style);
            // Drawing a cursor that points to the item the player has highlighted
            GUI.Label(new Rect(1185.0f, 15.0f + (fontHeight * (curOption)), 50.0f, 50.0f), ">", style);
        }

        GUI.Label(new Rect(1205.0f, 200.0f, 200.0f, 50.0f), "$" + MainInventory.totalMoney.ToString(), style);
    }

    // Buys the current item from the merchants inventory and attempts to place it into the player's inventory
    // If the player has insufficient funds or not enough space in their inventory, the function will return false,
    // telling the user that they cannot purchase the item at the current time
    public bool BuyItem(int itemID, int itemNum = 1) {
        int itemPrice = MainInventory.invInstance.ItemPrice(itemID);
        bool canPurchase = false;

        Debug.Log(itemPrice + ", " + MainInventory.totalMoney);

        // The player doesn't have enough money to purchase the items they want, return false
        if (MainInventory.totalMoney < itemPrice * itemNum) {
            return canPurchase;
        }

        // Add to the last open list in the inventory/the nearest stack of the item from the start
        int curItemNum = itemNum;
        for (int i = 0; i < MainInventory.INVENTORY_SIZE; i++) {
            int stackLimit = MainInventory.invInstance.ItemStackLimit(itemID);
            Debug.Log(curItemNum);
            if (stackLimit > 1) { // Purchasing stackable items form a merchant
                if (MainInventory.invInstance.invItem[i, 0] == itemID && MainInventory.invInstance.invItem[i, 1] < stackLimit) {
                    while(MainInventory.invInstance.invItem[i, 1] < stackLimit && curItemNum >= 1) { // There is a slot with items that can still be added to it
                        MainInventory.invInstance.invItem[i, 1]++;
                        MainInventory.totalMoney -= itemPrice;
                        curItemNum--;
                        Debug.Log("Item added to already existing stack");
                    }
                    canPurchase = true;
                } else if (MainInventory.invInstance.invItem[i, 0] == (int)ITEM_ID.NO_ITEM) { // The slot is empty
                    while(MainInventory.invInstance.invItem[i, 1] < stackLimit && curItemNum >= 1) {
                        MainInventory.invInstance.invItem[i, 0] = itemID;
                        MainInventory.invInstance.invItem[i, 1]++;
                        MainInventory.totalMoney -= itemPrice;
                        Debug.Log("Item added to new stack");
                        curItemNum--;
                    }
                    canPurchase = true;
                }
            } else { // Purchasing non-stackable items from a merchant
                if (MainInventory.invInstance.invItem[i, 0] == (int)ITEM_ID.NO_ITEM) { // The slot is empty
                    MainInventory.invInstance.invItem[i, 0] = itemID;
                    MainInventory.invInstance.invItem[i, 1] = 1;
                    MainInventory.totalMoney -= itemPrice;
                    curItemNum--;
                    canPurchase = true;
                    Debug.Log("Item added");
                }
            }
            // Exit the loop
            if (curItemNum == 0) { break; }
        }

        return canPurchase;
    }

    // Sells the current item selected within the player's inventory to the current merchant
    // If a merchant cannot buy the current item, the function will return false and prompt the user that
    // the merchant won't buy this type of item
    public bool SellItem(int itemID, int slotID, int itemNum) {
        int itemPrice = MainInventory.invInstance.ItemPrice(itemID);
        int itemClass = MainInventory.invInstance.ItemClass(itemID);
        bool saleBonus = false;
        bool canSell = false;

        // Search through the types of items that the merchant can purchase. If the item being sold can be bought, purchase the item
        var length = merchant.itemsToPurchase.Length;
        for (int i = 0; i < length; i++) {
            if (itemClass == merchant.itemsToPurchase[i]) {
                canSell = true;
                // Check if a bonus selling price should be applied
                if (merchant.merchantType != (int)MERCHANT.GENERAL) {
                    if (itemClass != (int)ITEM_CLASS.GENERIC) {
                        saleBonus = true;
                    }
                }
            }
        }

        // Sell the item
        if (canSell) {
            MainInventory.invInstance.RemoveItem(slotID, itemNum);
            if (saleBonus) { /* Give the player slightly more money (~90% of the actual price) */ }
            else { /* Give the player the default amount (~67% of the actual price) */ }
        }

        return canSell;
    }
}
