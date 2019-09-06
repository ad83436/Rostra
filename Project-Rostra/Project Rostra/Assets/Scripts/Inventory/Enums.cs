// Code Written By:     Christopher Brine
// Last Updated:        September 6th, 2019

enum ITEM_ID {
    NO_ITEM = 0x0000,
    TEST1 = 0x0001,
    TEST2 = 0x0002,
    TEST3 = 0x0003,
};

enum ITEM_TYPE {
    EQUIPABLE = 0x0000,     // Type for items that can be equipped and unequipped by certain characters (Armor/Weapons)
    CONSUMABLE = 0x0001,    // Type for items that can be consumed by a character (Potions, Stat Boosts, etc.)
    KEY_ITEM = 0x0002,      // Type for items that are important in the game (Quest Items)
};