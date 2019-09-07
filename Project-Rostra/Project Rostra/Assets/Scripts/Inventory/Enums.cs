// Code Written By:     Christopher Brine
// Last Updated:        September 6th, 2019

// NOTE -- The minimum and maximum values exists as a guideline to prevent any values from overlapping.

enum ITEM_ID { // Minimum Value -- 0x0000, Maximum Value -- 0xFEFF (65280 possible items)
    NO_ITEM =           0x0000,     
    TEST_POTION_HP =    0x0001,     // Test Item
    TEST_POTION_MP =    0x0002,     // Test Item
    TEST_QUEST_ITEM =   0x0003,     // Test Item
    TEST_ARMOR1 =       0x0004,     // Test Item        (Leather Armor)
    TEST_WEAPON1 =      0x0005,     // Test Item        (Iron Sword)
};  

enum ITEM_TYPE { // Minimum Value -- 0xFF00, Maximum Value -- 0xFF4F (64 possible item types)
    EQUIPABLE =         0xFF00,     // Type for items that can be equipped and unequipped by certain characters (Armor/Weapons)
    CONSUMABLE =        0xFF01,     // Type for items that can be consumed by a character (Potions, Stat Boosts, etc.)
    KEY_ITEM =          0xFF02,     // Type for items that are important in the game (Quest Items)
};

enum ARMOR_TYPE { // Minimum Value -- 0xFF50, Maximum Value -- 0xFFAF (80 possible armor types)
    LEATHER =           0xFF50,     // Usable by Damager
    CHAINMAIL =         0xFF51,     // Usable by Damager
    IRON =              0xFF52,     // Usable by Tank
    COPPER =            0xFF53,     // Usable by Tank
    STEEL =             0xFF54,     // Usable by Tank
    ROBE =              0xFF55,     // Usable by Support
    HIDE_CLOTHES =      0xFF56,     // Usable by Support/Tank/Damager   (Possible Starting Armor?)
    PLATED_STEEL =      0xFF57,     // Usable by Tank
};

enum WEAPON_TYPE { // Minimum Value -- 0xFFB0, Maximum Value -- 0xFFFF (80 possible weapon types)
    SWORD =             0xFFB0,     // Usable by Damager/Tank
    AXE =               0xFFB1,     // Usable by Tank/Damager
    MACE =              0xFFB2,     // Usable by Tank
    SPEAR =             0xFFB3,     // Usable by Damager/Support
    STAFF =             0xFFB4,     // Usable by Support
    DAGGER =            0xFFB5,     // Usable by Support/Damager
    BOW =               0xFFB6,     // Usable by Damager
    JAVELIN =           0xFFB7,     // Usable by Tank/Support
};