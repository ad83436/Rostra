
public static class PartyStats {
	public static CharacterStats[] chara = 
		{ new CharacterStats(4), new CharacterStats(4), new CharacterStats(4), new CharacterStats(4) };

	//returns a reference to the corisponding character
	public static ref CharacterStats CharaOne   => ref chara[0]; 
	public static ref CharacterStats CharaTwo   => ref chara[1]; 
	public static ref CharacterStats CharaThree => ref chara[2]; 
	public static ref CharacterStats CharaFour  => ref chara[3]; 
}

public struct CharacterStats {
	public CharacterStats(int i) {
		attack = 0;
		attackMod = 0;

		defence = 0;
		defenceMod = 0;

		curHealth = 0;
		maxHealth = 0;
		maxHealthMod = 0;

		curMana = 0;
		maxMana = 0;
		maxManaMod = 0;

		strength = 0;
		strengthMod = 0;

		critical = 0;
		criticalMod = 0;

		agility = 0;
		agilityMod = 0;
		
		level = 1;
		currentExperience = 0;				       
		neededExperience = 500;
		statPoints = 0;
	}
	
	// stats								       
	public int attack, attackMod;
	public int defence, defenceMod;
	public int curHealth, maxHealth, maxHealthMod;
	public int curMana, maxMana, maxManaMod;
	public int strength, strengthMod;
	public int critical, criticalMod;
	public int agility, agilityMod;

	///returns the total of each
	public int TotalAttack    => attack    + attackMod;
	public int TotalDefence   => defence   + defenceMod;
	public int TotalMaxHealth => maxHealth + maxHealthMod;
	public int TotalMaxMana  => maxMana   + maxManaMod;
	public int TotalStrength  => strength  + strengthMod;
	public int TotalCritical  => critical  + criticalMod;
	public int TotalAgility   => agility   + agilityMod;
	
	//experience, level, and skill points
	public int level;
	public int currentExperience;
	public int neededExperience;
	public int statPoints;
}

