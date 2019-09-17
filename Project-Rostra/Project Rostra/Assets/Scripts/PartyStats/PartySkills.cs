// Code Written By:     Christopher Brine
// Last Updated:        September 17th, 2019

public static class CharacterSkills {
    // This value represents the maximum number of skills that a player can haave equipped at once
    public const int MAX_SKILLS = 4;   

    public static PlayerSkills[] skills = {
        new PlayerSkills(
            new int[] {
                // Unlockable Skills for Fargas
                (int)SKILLS.TEST_SKILL1,
            }),
        new PlayerSkills(
            new int[] {
                // Unlockable Skills for Oberon
                (int)SKILLS.TEST_SKILL2,
            }),
        new PlayerSkills(
            new int[] {
                // Unlockable Skills for Frea
                (int)SKILLS.TEST_SKILL3,
            }),
        new PlayerSkills(
            new int[] {
                // Unlockable Skills for Arcelus
                (int)SKILLS.TEST_SKILL4,
            }),
    };

    //returns a reference to the corisponding character
    public static ref PlayerSkills setOne => ref skills[0];             // Fargas  == 0
    public static ref PlayerSkills setTwo => ref skills[1];             // Oberon  == 1
    public static ref PlayerSkills setThree => ref skills[2];           // Frea    == 2
    public static ref PlayerSkills setFour => ref skills[3];            // Arcelus == 3
}

public struct PlayerSkills {
    public int[] unlockableSkills;      // The list of skills that a character is able to learn
    public int[] equippedSkills;        // The skills that the player currently has equipped to the character
    public int[] learnedSkills;         // The array of skills that the character has unlocked

    // Initialize the player's list of learnable skills
    public PlayerSkills(int[] uSkills) {
        unlockableSkills = uSkills;
        learnedSkills = new int[unlockableSkills.Length];
        // Set the current learned skill to the deafult value (int)SKILLS.NO_SKILL
        var length = learnedSkills.Length;
        for (int i = 0; i < length; i++) {
            learnedSkills[i] = (int)SKILLS.NO_SKILL;
        }
        // Set the character's current equipped skills to the default value as well
        equippedSkills = new int[CharacterSkills.MAX_SKILLS]{
            (int)SKILLS.NO_SKILL,
            (int)SKILLS.NO_SKILL,
            (int)SKILLS.NO_SKILL,
            (int)SKILLS.NO_SKILL,
        };
    }

    // Attempts to equip a skill to the current player's active skills. If the skill list is full, it will let the player swap a
    // currently equipped skill with the newly selected one
    public bool EquipSkill(int skillID) {
        bool emptySlotExists = false;
        // Find an empty slot in the equipped skill list
        var length = equippedSkills.Length;
        for (int i = 0; i < length; i++) {
            // Equip the skill when an empty slot is found
            if (equippedSkills[i] == (int)SKILLS.NO_SKILL) {
                equippedSkills[i] = skillID;
                emptySlotExists = true;
                i = length; // Exit the loop
            }
        }
        // NOTE -- If this returns false, the skill inventory will open up the option to swap an equipped skill with
        // the current one the player is trying to equip
        return emptySlotExists;
    }

    // This method holds the stats for every single skill in the game
    // When called, it will search for the ID that was provided by the 
    // caller and return those stats in an array
    public float[] SkillStats(int skillID) {
        float[] skillStat = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        // NOTE -- Element 0 is the skill's damage/healing capabilities
        //         Element 1 is the skill's accuracy out of 100, I guess
        //         Element 2 is the skill's is how long it will take to execute the skill after casting it
        //         Element 3 is the skill's total range
        //         Element 4 is the skill's damage/healing size (Single Target, AoE, Full Row, etc)
        //                      0.0f == Single Target, 1.0f == AoE, 2.0f == Full Row
        //                      3.0f == Single Player Heal, 4.0 == Full Party Heal
        //         Element 5 is the skill's total MP usage

        // Find the required stats and return those to the caller
        switch (skillID) {
            case (int)SKILLS.TEST_SKILL1:
                skillStat[4] = (int)SKILL_TYPE.SINGLE_TARGET_ATK;
                break;
            case (int)SKILLS.TEST_SKILL2:
                skillStat[4] = (int)SKILL_TYPE.ALL_TARGETS_ATK;
                break;
            case (int)SKILLS.TEST_SKILL3:
                skillStat[4] = (int)SKILL_TYPE.FULL_ROW_ATK;
                break;
            case (int)SKILLS.TEST_SKILL4:
                skillStat[4] = (int)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                break;
        }

        return skillStat;
    }
}
