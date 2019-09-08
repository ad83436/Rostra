
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
		hitpoints = 0;
		magicpoints = 0;
		strength = 0;
		critical = 0;
		agility = 0;
		range = 0;
		skill = new Skills[] { Skills.None, Skills.None, Skills.None, Skills.None };
		level = 1;
		currentExperience = 0;				       
		neededExperience = 500;
		statPoints = 0;
	}										       
	// stats								       
	public int attack, attackMod;
	public int defence, defenceMod;
	public int hitpoints;
	public int magicpoints;
	public int strength;
	public int critical;
	public int agility;
	public int range;
	///returns the total of each
	public int TotalAttack => attack + attackMod;
	public int TotalDefence => defence + defenceMod;
	
	//skills
	public Skills[] skill;
	//returns a reference to the corisponding skill
	public ref Skills SkillOne   => ref skill[0];
	public ref Skills SkillTwo   => ref skill[1];
	public ref Skills SkillThree => ref skill[2];
	public ref Skills SkillFour  => ref skill[3];

	//experience, level, and skill points
	public int level;
	public int currentExperience;
	public int neededExperience;
	public int statPoints;
}

