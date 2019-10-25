// Code Written By:     Christopher Brine
// Last Updated:        October 10th, 2019

// IMPORTANT -- Make sure you put (int) before referencing the enum values in this file.
//                      Example:    (int)ITEM_ID.NO_ITEM;

// NOTE -- The minimum and maximum values exists as a guideline to prevent any values from overlapping.

enum ITEM_ID { // Minimum Value -- 0x0000, Maximum Value -- 0xFEFF (65280 possible items)
	NO_ITEM =           0x0000,
	TEST_POTION_HP =    0x0001, // Test Item
	TEST_POTION_MP =    0x0002, // Test Item
	TEST_QUEST_ITEM =   0x0003, // Test Item
	TEST_ARMOR1 =       0x0004, // Test Item        (Leather Armor)
	TEST_WEAPON1 =      0x0005, // Test Item        (Iron Sword)
    HP_POTION =         0x0006,
    MP_ELIXER =         0x0007,
    HOPE_POTION =       0x0008,
};

enum ITEM_TYPE { // Minimum Value -- 0xFF00, Maximum Value -- 0xFF4F (64 possible item types)
	EQUIPABLE =         0xFF00, // Type for items that can be equipped and unequipped by certain characters (Armor/Weapons)
	CONSUMABLE =        0xFF01,	// Type for items that can be consumed by a character (Potions, Stat Boosts, etc.)
	KEY_ITEM =          0xFF02, // Type for items that are important in the game (Quest Items)
};

enum ARMOR_TYPE { // Minimum Value -- 0xFF50, Maximum Value -- 0xFF0F
	LEATHER =           0xFF50, 
	CHAINMAIL =         0xFF51,		
	IRON =              0xFF52,			 
	COPPER =            0xFF53,		 
	STEEL =             0xFF54,			
	ROBE =              0xFF55,			 
	HIDE_CLOTHES =      0xFF56,  
	PLATED_STEEL =      0xFF57,  
};

enum WEAPON_TYPE { // Minimum Value -- 0xFFA0, Maximum Value -- 0xFFEF
	SWORD =             0xFFB0,			
	AXE =               0xFFB1,			
	MACE =              0xFFB2,			
	SPEAR =             0xFFB3,			
	STAFF =             0xFFB4,			
	DAGGER =            0xFFB5,		
	BOW =               0xFFB6,			
	JAVELIN =           0xFFB7,		
};

// The item's class is how the merchant decides whether they can purchase an item from the player or not
enum ITEM_CLASS {// Minimum Value -- 0xFFF0, Maximum Value -- 0xFFFF
    UNSELLABLE =        0xFFF0,     // Items that fall under this class are unsellable to any merchant
    GENERIC =           0xFFF1,     // Generic items like random sellable loot and stuff
    WEAPON =            0xFFF2,     // Any type of weaponry
    ARMOR =             0xFFF3,     // Any type of armor
    POTIONS =           0xFFF4,     // Any type of consumable potion
    STAT_BOOSTS =       0xFFF5,     // Any type of permanent stat boosting item
}   