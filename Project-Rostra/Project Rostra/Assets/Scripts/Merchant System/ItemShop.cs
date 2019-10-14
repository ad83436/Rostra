using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        October 10th, 2019

public class ItemShop : MonoBehaviour {
    public List<int> shopItems = new List<int>();   // The list of items that the shopkeeper has on them
    public bool isSelling = false;                  // If true, the user will be selling items from their inventory

    private Merchant merchant;                      // The current merchant that owns this store

    public ItemShop(Merchant merchant) {
        this.merchant = merchant;
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
        for (int i = 0; i < MainInventory.INVENTORY_SIZE; i++) {
            int stackLimit = MainInventory.invInstance.ItemStackLimit(itemID);
            int slotItem = MainInventory.invInstance.invItem[i, 0];
            if (stackLimit > 1) { // Purchasing stackable items form a merchant
                if (slotItem == itemID && MainInventory.invInstance.invItem[i, 1] < stackLimit) {
                    while(MainInventory.invInstance.invItem[i, 1] < stackLimit && curItemNum > 1) { // There is a slot with items that can still be added to it
                        MainInventory.invInstance.invItem[i, 1]++;
                        MainInventory.totalMoney -= itemPrice;
                        curItemNum--;
                        canPurchase = true;
                    }
                } else if (slotItem == 0) { // The slot is empty
                    while(MainInventory.invInstance.invItem[i, 1] < stackLimit && curItemNum > 1) {
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
