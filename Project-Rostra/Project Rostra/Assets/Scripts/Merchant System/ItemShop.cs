using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        October 16th, 2019

public class ItemShop : MonoBehaviour {
    // The list of items that the shopkeeper has on them
    public List<int> shopItems = new List<int>();

    public int curOption = 0;              // The current item that the user has selected from the shop's available stock
    public int selectedOption = -1;        // Gets set to the player's last curOption whenever they press the select button on an item
    public int numToPurchase = 1;          // How many of that item the player wishes to purchase at a single time
    public int firstToDraw = 0;            // The first item that will be drawn in the visible portion of the shop for the player
    public int numToDraw = 8;              // The total number of shop items visible to the player on a single page

    private bool isSelling = false;         // If true, the player will be selling items from their inventory to the shopkeeper
    private bool isBuying = false;          // If true, the player is purchasing items from the merchant

    // The current merchant that owns this store
    private Merchant merchant;                     

    public Font GuiSmall;

    public ItemShop(Merchant merchant) {
        this.merchant = merchant;
    }

    private void Awake() {
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

        if (!isBuying && !isSelling) { // If the player has just opened the shop, but hasn't selected
            // Moving between the buy, sell, and exit options
            if (keyUp) {
                curOption--;
                if (curOption < 0) {
                    curOption = 2;
                }
            } else if (keyDown) {
                curOption++;
                if (curOption > 2) {
                    curOption = 0;
                }
            }

            if (keySelect) {
                switch (curOption) {
                    case 0: // Purchasing items from the merchant
                        isBuying = true;
                        isSelling = false;
                        break;
                    case 1: // Selling items to the merchant
                        isSelling = true;
                        isBuying = false;
                        break;
                    case 2: // Exiting the shop
                        Destroy(gameObject);
                        break;
                }
                curOption = 0;
            }

            // Exit the shop
            if (keyReturn) {
                Destroy(gameObject);
            }
            return;
        }

        // Menu code for when the player is purchasing items from the 
        if (isBuying) {
            if (selectedOption == -1) {
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

                // Selecting an option in the shop list
                if (keySelect) {
                    int[] items = shopItems.ToArray();
                    if (!BuyItem(items[curOption])) {
                        Debug.Log("Cannot Purchase -- insufficent funds");
                    }
                }
            }
        } else if (isSelling) {
            if (selectedOption == -1) {
                if (keyUp) {
                    curOption--;
                    // Shifting the inventory's view up
                    if (curOption < firstToDraw + (numToDraw / 2) - 1 && firstToDraw > 0) {
                        firstToDraw--;
                    }
                    // Looping to the end of the inventory if the player presses up while on the first item
                    if (curOption < 0) {
                        curOption = MainInventory.INVENTORY_SIZE - 1;
                        firstToDraw = curOption - numToDraw;
                    }
                } else if (keyDown) {
                    curOption++;
                    // Shifting the inventory's view down
                    if (curOption > firstToDraw + (numToDraw / 2) + 1 && firstToDraw < MainInventory.INVENTORY_SIZE - 1 - numToDraw) {
                        firstToDraw++;
                    }
                    // Looping to the start of the inventory if the player presses down while on the last item
                    if (curOption > MainInventory.INVENTORY_SIZE - 1) {
                        curOption = 0;
                        firstToDraw = 0;
                    }
                }

                // Selecting an item to sell to the merchant
                if (keySelect) {
                    if (MainInventory.invInstance.invItem[curOption, 0] != 0) {
                        if (!SellItem(MainInventory.invInstance.invItem[curOption, 0], curOption)) {
                            Debug.Log("Cannot Purchase -- Merchant doesn't sell this item");
                        }
                    }
                }
            }
        }

        // Returning to the buy, sell, and exit menu
        if (keyReturn) {
            isBuying = false;
            isSelling = false;
            curOption = 0;
            selectedOption = -1;
        }
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

        // Drawing the buy, sell, and exit GUI stuff
        if (!isBuying && !isSelling) {
            // Drawing the menu elements
            GUI.Label(new Rect(1205.0f, 15.0f, 200.0f, 50.0f), "Buy Items", style);
            GUI.Label(new Rect(1205.0f, 15.0f + fontHeight, 200.0f, 50.0f), "Sell Items", style);
            GUI.Label(new Rect(1205.0f, 15.0f + (fontHeight * 2), 200.0f, 50.0f), "Exit", style);
            // Drawing the cursor
            GUI.Label(new Rect(1185.0f, 15.0f + (fontHeight * curOption), 50.0f, 50.0f), ">", style);
            return;
        }

        // Drawing the purchase GUI stuff
        if (isBuying) {
            // Drawing the inventory items
            for (int i = 0; i <= (shopItems.Count - 1); i++) {
                int[] items = shopItems.ToArray();
                GUI.Label(new Rect(1205.0f, 15.0f + (fontHeight * (i)), 200.0f, 50.0f), MainInventory.invInstance.ItemName(items[i]), style);
                // Drawing a cursor that points to the item the player has highlighted
                if (i == curOption) { GUI.Label(new Rect(1185.0f, 15.0f + (fontHeight * curOption), 50.0f, 50.0f), ">", style); }
            }
        }

        // Drawing the inventory for selling items
        if (isSelling) {
            // Drawing the inventory items
            for (int i = firstToDraw; i <= firstToDraw + numToDraw; i++) {
                GUI.Label(new Rect(45.0f, 15.0f + (fontHeight * (i - firstToDraw)), 200.0f, 50.0f), MainInventory.invInstance.ItemName(MainInventory.invInstance.invItem[i, 0]), style);
                // Only show number of items beside items that can stack
                if (MainInventory.invInstance.ItemStackLimit(MainInventory.invInstance.invItem[i, 0]) > 1) { GUI.Label(new Rect(550.0f, 15.0f + (fontHeight * (i - firstToDraw)), 200.0f, fontHeight), "x" + MainInventory.invInstance.invItem[i, 1], style); }
                // Drawing a cursor that points to the item the player has highlighted
                if (i == curOption) { GUI.Label(new Rect(25.0f, 15.0f + (fontHeight * (curOption - firstToDraw)), 50.0f, 50.0f), ">", style); }
                // Let the player know this item is equipped if it is
                if (MainInventory.invInstance.invItem[i, 2] != -1) { GUI.Label(new Rect(510.0f, 15.0f + (fontHeight * (i - firstToDraw)), 50.0f, 50.0f), "(E)", style); }
            }
        }

        GUI.Label(new Rect(1205.0f, 200.0f, 200.0f, 50.0f), "$" + MainInventory.totalMoney.ToString(), style);
    }

    // Buys the current item from the merchants inventory and attempts to place it into the player's inventory
    // If the player has insufficient funds or not enough space in their inventory, the function will return false,
    // telling the user that they cannot purchase the item at the current time
    public bool BuyItem(int itemID, int itemNum = 1) {
        int itemPrice = MainInventory.invInstance.ItemPrice(itemID);
        bool canPurchase = false;

        // The player doesn't have enough money to purchase the items they want, return false
        if (MainInventory.totalMoney < itemPrice * itemNum) {
            return canPurchase;
        }

        // Add to the last open list in the inventory/the nearest stack of the item from the start
        int curItemNum = itemNum;
        for (int i = 0; i < MainInventory.INVENTORY_SIZE && curItemNum > 0; i++) {
            int stackLimit = MainInventory.invInstance.ItemStackLimit(itemID);
            int slotItem = MainInventory.invInstance.invItem[i, 0];
            if (stackLimit > 1) { // Purchasing stackable items form a merchant
                if (slotItem == itemID && MainInventory.invInstance.invItem[i, 1] < stackLimit) {
                    while(MainInventory.invInstance.invItem[i, 1] < stackLimit && curItemNum >= 1) { // There is a slot with items that can still be added to it
                        MainInventory.invInstance.invItem[i, 1]++;
                        MainInventory.totalMoney -= itemPrice;
                        curItemNum--;
                        canPurchase = true;
                    }
                } else if (slotItem == 0) { // The slot is empty
                    while(MainInventory.invInstance.invItem[i, 1] < stackLimit && curItemNum >= 1) {
                        MainInventory.invInstance.invItem[i, 0] = itemID;
                        MainInventory.invInstance.invItem[i, 1]++;
                        MainInventory.totalMoney -= itemPrice;
                        curItemNum--;
                        canPurchase = true;
                    }
                }
            } else { // Purchasing non-stackable items from a merchant
                if (slotItem == 0) { // The slot is empty
                    MainInventory.invInstance.invItem[i, 0] = itemID;
                    MainInventory.invInstance.invItem[i, 1] = 1;
                    MainInventory.totalMoney -= itemPrice;
                    curItemNum--;
                    canPurchase = true;
                }
            }
        }

        return canPurchase;
    }

    // Sells the current item selected within the player's inventory to the current merchant
    // If a merchant cannot buy the current item, the function will return false and prompt the user that
    // the merchant won't buy this type of item
    public bool SellItem(int itemID, int slotID, int itemNum = 1) {
        int itemPrice = MainInventory.invInstance.ItemPrice(itemID);
        int itemClass = MainInventory.invInstance.ItemClass(itemID);
        bool saleBonus = false;
        bool canSell = true;

        // Search through the types of items that the merchant can purchase. If the item being sold can be bought, purchase the item
        //var length = merchant.itemsToPurchase.Length;
        var length = 0;
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
        // Block of code above is unused for now until I can fully implement the merchant classes

        // Sell the item
        if (canSell) {
            MainInventory.invInstance.RemoveItem(slotID, itemNum);
            if (saleBonus) {
                MainInventory.totalMoney += (int)((MainInventory.invInstance.ItemPrice(itemID) * itemNum) * 0.9);
            } else {
                MainInventory.totalMoney += (int) ((MainInventory.invInstance.ItemPrice(itemID) * itemNum) * 0.67);
            }
        }

        return canSell;
    }
}
