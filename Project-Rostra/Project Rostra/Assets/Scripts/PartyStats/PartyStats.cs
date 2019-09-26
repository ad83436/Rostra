// Code Written By:     Christopher Brine and Domara Shlimon
// Last Updated:        September 17th, 2019

public static class PartyStats {
    public static CharacterStats[] chara ={
        new CharacterStats(
            new int[]{ 
                // Equipable Weapons for Fargas
                (int)WEAPON_TYPE.AXE,
                (int)WEAPON_TYPE.SWORD,
                (int)WEAPON_TYPE.MACE,
                // Equipable Armor for Fargas
                (int)ARMOR_TYPE.LEATHER,
                (int)ARMOR_TYPE.HIDE_CLOTHES,
                (int)ARMOR_TYPE.CHAINMAIL,
                (int)ARMOR_TYPE.COPPER
            }),
		new CharacterStats(
            new int[]{ 
                // Equipable Weapons for Oberon
                (int)WEAPON_TYPE.SPEAR,
                (int)WEAPON_TYPE.JAVELIN,
                (int)WEAPON_TYPE.STAFF,
                // Equipable Armor for Oberon
                (int)ARMOR_TYPE.HIDE_CLOTHES,
                (int)ARMOR_TYPE.LEATHER,
                (int)ARMOR_TYPE.PLATED_STEEL,
                (int)ARMOR_TYPE.CHAINMAIL,
                (int)ARMOR_TYPE.COPPER,
                (int)ARMOR_TYPE.IRON,
                (int)ARMOR_TYPE.STEEL
            }),
		new CharacterStats(
            new int[]{ 
                // Equipable Weapons for Frea
                (int)WEAPON_TYPE.DAGGER,
                (int)WEAPON_TYPE.BOW,
                // Equipable Weapons for Frea
                (int)ARMOR_TYPE.HIDE_CLOTHES,
                (int)ARMOR_TYPE.LEATHER,
                (int)ARMOR_TYPE.IRON,
                (int)ARMOR_TYPE.CHAINMAIL
            }),
        new CharacterStats(
            new int[]{ 
                // Equipable Weapons for Arcelus
                (int)WEAPON_TYPE.STAFF,
                (int)WEAPON_TYPE.SPEAR,
                (int)WEAPON_TYPE.SWORD,
                // Equipable Armor for Arcelus
                (int)ARMOR_TYPE.HIDE_CLOTHES,
                (int)ARMOR_TYPE.LEATHER,
                (int)ARMOR_TYPE.ROBE,
                (int)ARMOR_TYPE.CHAINMAIL
            }),
    };

	//returns a reference to the corisponding character
	public static ref CharacterStats CharaOne => ref chara[0];              // Fargas  == 0
	public static ref CharacterStats CharaTwo => ref chara[1];              // Oberon  == 1
	public static ref CharacterStats CharaThree => ref chara[2];            // Frea    == 2
	public static ref CharacterStats CharaFour => ref chara[3];             // Arcelus == 3
}

public struct CharacterStats {
	public CharacterStats(int[] vEquipables) {
		attack = 0.0f;
		attackMod = 0.0f;

		defence = 0.0f;
		defenceMod = 0.0f;

		hitpoints = 0.0f;
		maxHealth = 0.0f;
		maxHealthMod = 0.0f;

		magicpoints = 0.0f;
		maxMana = 0.0f;
		maxManaMod = 0.0f;

		strength = 0.0f;
		strengthMod = 0.0f;

		critical = 0.0f;
		criticalMod = 0.0f;

		agility = 0.0f;
		agilityMod = 0.0f;

		rage = 0.0f;

		speed = 0.0f;
		speedMod = 0.0f;

		level = 1;
		currentExperience = 0;
		neededExperience = 500;
		statPoints = 0;

        validEquipables = vEquipables;
	}

	// stats								       
	public float attack, attackMod;
	public float defence, defenceMod;
	public float hitpoints, maxHealth, maxHealthMod;
	public float magicpoints, maxMana, maxManaMod;
	public float strength, strengthMod;
	public float critical, criticalMod;
	public float agility, agilityMod;
	public float rage;
	public float speed, speedMod;

    // The weapon and armor types each player can equip
	readonly public int[] validEquipables;

	///returns the total of each
	public float TotalAttack => attack + attackMod;
	public float TotalDefence => defence + defenceMod;
	public float TotalMaxHealth => maxHealth + maxHealthMod;
	public float TotalMaxMana => maxMana + maxManaMod;
	public float TotalStrength => strength + strengthMod;
	public float TotalAgility => agility + agilityMod;
	public float TotalCritical => critical + criticalMod;
	public float TotalSpeed => speed + speedMod;

	//experience, level, and skill points
	public int level;
	public int currentExperience;
	public int neededExperience;
	public int statPoints;
}

