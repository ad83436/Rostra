// Code Written By:     Christopher Brine and Domara Shlimon
// Last Updated:        September 26th, 2019

// IMPORTANT -- Make sure you put (int) before referencing the enum values in this file.
//                      Example:    (int)SKILLS.NO_SKILL;

public enum SKILLS { // Minimum Value -- 0x0000, Maximum Value -- 0xFFEF (65519 possible skills)
	NO_SKILL =              0x0000,
    TEST_Fargas =           0x0001,
    TEST_Frea =             0x0002,
    TEST_Oberon =           0x0003,
    TEST_Arcelus =          0x0004,
    TEST_Fargas2 =          0x0005,
    TEST_Frea2 =            0x0006,
    TEST_Oberon2 =          0x0007,
    TEST_Arcelus2 =         0x0008,
    Fa_SwiftStrike =        0x0009,
    Fa_SwordOfFury =        0x000A,
    Fr_PiercingShot =       0x000B,
    Fr_ArrowRain =          0x000C,
    Fr_DoubleShot =         0x000D,
    Fr_PhantomShot =        0x000E,
    Fr_IDontMiss =          0x000F,
    Fr_NeverAgain =         0x0010,
    Ar_HealingAura =        0x0011,
    Ar_DrainEye =           0x0012,
    Ob_ShieldAlly =         0x0013,
    Ob_ShieldAllAllies =    0x0014,
};

public enum SKILL_TYPE{ // Minimum Value -- 0xFFF0, Maximum Value -- 0xFFFF (16 possible skill types)
    SINGLE_TARGET_ATK =     0xFFF0,     // Skills of this type will target a single enemy
    FULL_ROW_ATK =          0xFFF1,     // SKills of this type will target a full row of enemies
    ALL_TARGETS_ATK =       0XFFF2,     // Skills of this type will target all enemies on the field
    SINGLE_PLAYER_HEAL =    0xFFF3,     // Skills of this type will heal a single player character
    ALL_PLAYER_HEAL =       0xFFF4,     // Skills of this type will heal all player characters
    SINGLE_PLAYER_BUFF =    0xFFF5,     // Skills of this type will buff a single player character's stats
    ALL_PLAYER_BUFF =       0XFFF6,     // Skills of this type will buff alll player character's stats
    SINGLE_TARGET_DEBUFF =  0xFFF7,     // Skills of this type will debuff a single enemy's stats
    FULL_ROW_DEBUFF =       0xFFF8,     // Skills of this type will debuff an entire row of enemies
    ALL_TARGETS_DEBUFF =    0xFFF9,     // Skills of this type will debuff every enemy on the field
};