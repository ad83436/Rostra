// Code Written By:     Christopher Brine
// Last Updated:        September 17th, 2019

public static class PartySkills {
    // This value represents the maximum number of skills that a player can haave equipped at once
    public const int MAX_SKILLS = 4;   

    public static CharacterSkills[] skills = {
        new CharacterSkills(
            new int[] {
                // Unlockable Skills for Fargas
                (int)SKILLS.TEST_SKILL1,
            }),
        new CharacterSkills(
            new int[] {
                // Unlockable Skills for Oberon
                (int)SKILLS.TEST_SKILL2,
            }),
        new CharacterSkills(
            new int[] {
                // Unlockable Skills for Frea
                (int)SKILLS.TEST_SKILL3,
            }),
        new CharacterSkills(
            new int[] {
                // Unlockable Skills for Arcelus
                (int)SKILLS.TEST_SKILL4,
            }),
    };

    //returns a reference to the corisponding character
    public static ref CharacterSkills setOne => ref skills[0];              // Fargas  == 0
    public static ref CharacterSkills setTwo => ref skills[1];              // Oberon  == 1
    public static ref CharacterSkills setThree => ref skills[2];            // Frea    == 2
    public static ref CharacterSkills setFour => ref skills[3];             // Arcelus == 3
}

public struct CharacterSkills {
    public int[] unlockableSkills;      // The list of skills that a character is able to learn
    public int[] equippedSkills;        // The skills that the player currently has equipped to the character
    public int[] learnedSkills;         // The array of skills that the character has unlocked

    public int numSkillsLearned;        // The number of skills that the character has learned so far 

    // Initialize the player's list of learnable skills
    public CharacterSkills(int[] uSkills) {
        unlockableSkills = uSkills;
        learnedSkills = new int[unlockableSkills.Length];
        // Set the current learned skill to the deafult value (int)SKILLS.NO_SKILL
        var length = learnedSkills.Length;
        for (int i = 0; i < length; i++) {
            learnedSkills[i] = (int)SKILLS.NO_SKILL;
        }
        // Set the character's current equipped skills to the default value as well
        equippedSkills = new int[PartySkills.MAX_SKILLS]{
            (int)SKILLS.NO_SKILL,
            (int)SKILLS.NO_SKILL,
            (int)SKILLS.NO_SKILL,
            (int)SKILLS.NO_SKILL,
        };
        numSkillsLearned = 0;
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

    // Attempts to remove an equipped skill for the respective array. If it cannot remove the skill because the slot provided
    // was out of bounds of there was no skill equipped, this code will return false.
    public bool UnequipSkill(int skillSlot) {
        if (skillSlot >= 0 && skillSlot < PartySkills.MAX_SKILLS) {
            equippedSkills[skillSlot] = (int)SKILLS.NO_SKILL;
            return true;
        }
        return false;
    }

    // Finds the name of the skill relative to the ID provided in the argument
    public string SkillName(int skillID) {
        string name = "";

        // Find the name relative to the ID given
        switch (skillID) {
            case (int)SKILLS.TEST_SKILL1:
                name = "Booty Destroyer";
                break;
            case (int)SKILLS.TEST_SKILL2:
                name = "Spinng Ass Shot";
                break;
            case (int)SKILLS.TEST_SKILL3:
                name = "Implant Popper";
                break;
            case (int)SKILLS.TEST_SKILL4:
                name = "Healing Anal Needle";
                break;
        }

        return name;
    }

    // Finds the skill's description relative to the ID provided in the argument parameter
    public string SkillDescription(int skillID) {
        string description = "";

        // Find the description relative to the ID given
        switch (skillID) {
            case (int)SKILLS.TEST_SKILL1:
                break;
            case (int)SKILLS.TEST_SKILL2:
                break;
            case (int)SKILLS.TEST_SKILL3:
                break;
            case (int)SKILLS.TEST_SKILL4:
                break;
        }

        return description;
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
        //         Element 5 is the skill's total MP usage

        // Find the required stats and return those to the caller
        switch (skillID) {
            case (int)SKILLS.TEST_SKILL1:
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_ATK;
                break;
            case (int)SKILLS.TEST_SKILL2:
                skillStat[4] = (float)SKILL_TYPE.ALL_TARGETS_ATK;
                break;
            case (int)SKILLS.TEST_SKILL3:
                skillStat[4] = (float)SKILL_TYPE.FULL_ROW_ATK;
                break;
            case (int)SKILLS.TEST_SKILL4:
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                break;
        }

        return skillStat;
    }
}
