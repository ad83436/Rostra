// Code Written By:     Christopher Brine
// Last Updated:        September 11th, 2019

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
};

enum ITEM_TYPE { // Minimum Value -- 0xFF00, Maximum Value -- 0xFF4F (64 possible item types)
	EQUIPABLE =         0xFF00, // Type for items that can be equipped and unequipped by certain characters (Armor/Weapons)
	CONSUMABLE =        0xFF01,	// Type for items that can be consumed by a character (Potions, Stat Boosts, etc.)
	KEY_ITEM =          0xFF02, // Type for items that are important in the game (Quest Items)
};

enum ARMOR_TYPE { // Minimum Value -- 0xFF50, Maximum Value -- 0xFFAF (80 possible armor types)
	LEATHER =           0xFF50, 
	CHAINMAIL =         0xFF51,		
	IRON =              0xFF52,			 
	COPPER =            0xFF53,		 
	STEEL =             0xFF54,			
	ROBE =              0xFF55,			 
	HIDE_CLOTHES =      0xFF56,  
	PLATED_STEEL =      0xFF57,  
};

enum WEAPON_TYPE { // Minimum Value -- 0xFFB0, Maximum Value -- 0xFFFF (80 possible weapon types)
	SWORD =             0xFFB0,			
	AXE =               0xFFB1,			
	MACE =              0xFFB2,			
	SPEAR =             0xFFB3,			
	STAFF =             0xFFB4,			
	DAGGER =            0xFFB5,		
	BOW =               0xFFB6,			
	JAVELIN =           0xFFB7,		
};