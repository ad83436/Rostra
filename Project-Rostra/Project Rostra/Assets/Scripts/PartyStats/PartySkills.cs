// Code Written By:     Christopher Brine
// Last Updated:        September 26th, 2019

public static class PartySkills {
    // This value represents the maximum number of skills that a player can haave equipped at once
    public const int MAX_SKILLS = 4;   

    public static CharacterSkills[] skills = {
        new CharacterSkills(
            new int[] {
                // Unlockable Skills for Fargas
                (int)SKILLS.Fa_SwiftStrike,
                (int)SKILLS.Fa_SwordOfFury,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
            }),
        new CharacterSkills(
            new int[] {
                // Unlockable Skills for Oberon
                (int)SKILLS.Ob_ShieldAlly,
                (int)SKILLS.Ob_ShieldAllAllies,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
            }),
        new CharacterSkills(
            new int[] {
                // Unlockable Skills for Frea
                (int)SKILLS.Fr_DoubleShot,
                (int)SKILLS.Fr_PiercingShot,
                (int)SKILLS.Fr_ArrowRain,
                (int)SKILLS.Fr_IDontMiss,
                (int)SKILLS.Fr_PhantomShot,
                (int)SKILLS.Fr_NeverAgain,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
            }),
        new CharacterSkills(
            new int[] {
                // Unlockable Skills for Arcelus
                (int)SKILLS.Ar_HealingAura,
                (int)SKILLS.Ar_DrainEye,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
                (int)SKILLS.NO_SKILL,
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
        numSkillsLearned = 0;
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
    }
}
