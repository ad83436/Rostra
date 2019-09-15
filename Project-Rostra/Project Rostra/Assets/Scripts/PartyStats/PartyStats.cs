
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
		
		level = 1;
		currentExperience = 0;				       
		neededExperience = 500;
		statPoints = 0;
	}
	
	// stats								       
	public float attack, attackMod;
	public float defence, defenceMod;
	public float hitpoints, maxHealth, maxHealthMod;
	public float magicpoints, maxMana, maxManaMod;
	public float strength, strengthMod;
	public float critical, criticalMod;
	public float agility, agilityMod;

	///returns the total of each
	public float TotalAttack    => attack    + attackMod;
	public float TotalDefence   => defence   + defenceMod;
	public float TotalMaxHealth => maxHealth + maxHealthMod;
	public float TotalMaxMana   => maxMana   + maxManaMod;
	public float TotalStrength  => strength  + strengthMod;
	public float TotalCritical  => critical  + criticalMod;
	public float TotalAgility   => agility   + agilityMod;
	
	//experience, level, and skill points
	public int level;
	public int currentExperience;
	public int neededExperience;
	public int statPoints;
}

