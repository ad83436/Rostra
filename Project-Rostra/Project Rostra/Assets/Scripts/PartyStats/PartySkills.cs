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
        float[] skillStats = { 0.0f, 0.0f, 0.0f };
        // Idk what to make these values yet so I just put three for now
        // I was thinking the first is the damage it deals, the second
        // is its accuracy, and then the third could be how much time it
        // takes to execute the skill when in battle or some shit

        // Find the required stats and return those to the caller
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

        return skillStats;
    }
}
