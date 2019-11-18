using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        October 3rd, 2019

public class SkillsInventory : MonoBehaviour {
    public static SkillsInventory invInstance;  // Holds the current inventory instance in a single variable

    // The variables that are used for drawing the GUI to the screen
    public Font GuiSmall;
    public bool isVisible = false;

    // Variables for selecting the unlocked skills to equip/unequip them
    private int curOption = 0;                  // The current skill the player has their cursor over
    private int selectedOption = -1;            // The skill that the player has selected
    private int subCurOption = 0;               // The current option the player has their cursor over after selecting a skill
    private int playerIndex = 0;                // The current skill inventory that is being looked at (0 = Fargas, 1 = Oberon, etc.)

    // Variables for drawing the unlocked skills list to the screen
    private int firstToDraw = 0;                // The first item from the unlocked skills array to draw out of the full list
    private int numToDraw = 5;                  // The number of skills that are visible to the player at any given time

    // Set the skill inventory instance to this one if no skill inventory is active, delete otherwise
    public void Awake() {
        if (invInstance == null) {
            invInstance = this;
            DontDestroyOnLoad(gameObject);
            GameManager.instance.listOfUndestroyables.Add(this.gameObject);

        } else {
            Destroy(gameObject);
        }

        AddToUnlockedSkills((int)SKILLS.Fa_SwordOfFury, 0);
        EquipSkill((int)SKILLS.Fa_SwordOfFury, 0, 0);
        AddToUnlockedSkills((int)SKILLS.Ob_ShieldAlly, 1);
        EquipSkill((int)SKILLS.Ob_FierceStrike, 0, 1);
        AddToUnlockedSkills((int)SKILLS.Fr_DoubleShot, 2);
        EquipSkill((int)SKILLS.Fr_DoubleShot, 0, 2);
        AddToUnlockedSkills((int)SKILLS.Ar_Heal, 3);
        EquipSkill((int)SKILLS.Ar_Heal, 0, 3);
    }

    private void OnDestroy()
    {
        if (invInstance == this)
        {
            invInstance = null;
        }
    }

    // Handling keyboard functionality
    private void Update() {
        // Getting Keyboard Input
        bool keyOpen, keySelect, keyReturn, keyUp, keyDown, keyLeft, keyRight;
        keyOpen = Input.GetKeyDown(KeyCode.M);
        keySelect = Input.GetKeyDown(KeyCode.Z);
        keyReturn = Input.GetKeyDown(KeyCode.X);
        keyUp = Input.GetKeyDown(KeyCode.UpArrow);
        keyDown = Input.GetKeyDown(KeyCode.DownArrow);
        keyLeft = Input.GetKeyDown(KeyCode.RightArrow);
        keyRight = Input.GetKeyDown(KeyCode.LeftArrow);

        // Opening and Closing the Inventory Window
        if (keyOpen) {
            isVisible = !isVisible;
            return;
        }

        // Don't allow any input functionality when the skill inventory is not open
        if (!isVisible) {
            return;
        }

        // Moving through the unlocked skills list
        if (selectedOption == -1) {
            if (keyUp) { // Moving up through the list
                int numSkills = PartySkills.skills[playerIndex].numSkillsLearned;
                curOption--;
                // Shifting the skill list's view up
                if (curOption < firstToDraw + (numToDraw / 2) - 1 && firstToDraw > 0) {
                    firstToDraw--;
                }
                // Looping to the end of the skill list if the player presses up when on the first element in the list
                if (curOption < 0) {
                    curOption = numSkills - 1;
                    firstToDraw = curOption - numToDraw;
                    if (firstToDraw < 0) {
                        firstToDraw = 0;
                    }
                    if (numSkills <= 0) {
                        curOption = 0;
                    }
                }
            } else if (keyDown) { // Moving down through the list
                int numSkills = PartySkills.skills[playerIndex].numSkillsLearned;
                curOption++;
                // Shifting the skill list's view down
                if (curOption > firstToDraw + (numToDraw / 2) + 1 && firstToDraw < numSkills - 1 - numToDraw) {
                    firstToDraw++;
                }
                // Looping to the start of the skill list if the player presses down when on the last element in the list
                if (curOption >= numSkills) {
                    curOption = 0;
                    firstToDraw = 0;
                }
            }
            // Shifting to another player's skill page
            if (keyRight) {
                playerIndex++;
                // Loop back to the first character's skill inventory
                if (playerIndex > 3) {
                    playerIndex = 0;
                }
            } else if (keyLeft) {
                playerIndex--;
                // Loop back to the last character's skill inventory
                if (playerIndex < 0) {
                    playerIndex = 3;
                }
            }

            // Selecting a Skill and opening the option menu
            if (keySelect) {
                selectedOption = curOption;
            }
        } else { // Equipping/unequipping skills from the four equipped skills the player can have at once
            // Shifting up and down through the sub-menu options
            if (keyUp) {
                subCurOption--;
                if (subCurOption < 0) {
                    subCurOption = PartySkills.skills[playerIndex].equippedSkills.Length - 1;
                }
            } else if (keyDown) {
                subCurOption++;
                if (subCurOption > PartySkills.skills[playerIndex].equippedSkills.Length - 1) {
                    subCurOption = 0;
                }
            }

            // Selecting one of the given options
            if (keySelect) {
                EquipSkill(PartySkills.skills[playerIndex].learnedSkills[selectedOption], subCurOption, playerIndex);
                // Exit out of the sub-menu
                selectedOption = -1;
                subCurOption = 0;
            }

            // Unselecting the current item, returning the player back to the main inventory window
            if (keyReturn) {
                selectedOption = -1;
                subCurOption = 0;
            }
        }
    }

    // Drawing the Skill Inventory to the Screen
    private void OnGUI() {
        // Don't allow the skill inventory to be drawn when it isn't open
        if (!isVisible) {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label) {
            font = GuiSmall,
            fontSize = 30,
        };
        var fontHeight = style.lineHeight;

        // Drawing the player's equipped skills
        var length = PartySkills.skills[playerIndex].equippedSkills.Length;
        for (int e = 0; e < length; e++) {
            GUI.Label(new Rect(45.0f, 265.0f + (fontHeight * e), 200.0f, 50.0f), SkillName(PartySkills.skills[playerIndex].equippedSkills[e]), style);
            // Drawing a cursor that points to the item the player has highlighted
            if (selectedOption != -1) { GUI.Label(new Rect(25.0f, 265.0f + (fontHeight * subCurOption), 50.0f, 50.0f), ">", style); }
        }

        // Drawing the skill inventory items
        for (int i = firstToDraw; i <= firstToDraw + numToDraw; i++) {
            if (PartySkills.skills[playerIndex].learnedSkills[i] != (int)SKILLS.NO_SKILL) {
                GUI.Label(new Rect(45.0f, 15.0f + (fontHeight * (i - firstToDraw)), 200.0f, 50.0f), SkillName(PartySkills.skills[playerIndex].learnedSkills[i]), style);
                // Check if this skill has been equipped by the current player
                for (int ii = 0; ii < length; ii++) {
                    if (PartySkills.skills[playerIndex].learnedSkills[i] == PartySkills.skills[playerIndex].equippedSkills[ii]) {
                        GUI.Label(new Rect(510.0f, 15.0f + (fontHeight * (i - firstToDraw)), 50.0f, 50.0f), "(E)", style);
                        ii = length; // Exit the loop
                    }
                }
            }
            // Drawing a cursor that points to the item the player has highlighted
            GUI.Label(new Rect(25.0f, 15.0f + (fontHeight * (curOption - firstToDraw)), 50.0f, 50.0f), ">", style);
        }
    }

    #region Skill Manipulation Scripts (Adding, Removing, Unlocking)

    // Attempts to equip a skill to the current player's active skills. If the skill list is full, it will let the player swap a
    // currently equipped skill with the newly selected one
    public void EquipSkill(int skillID, int slotID, int playerID) {
        // Equip the skill to the currently selected slot
        PartySkills.skills[playerID].equippedSkills[slotID] = skillID;
        // Check through the four slots to stop any duplicate skills from being equipped
        var length = PartySkills.skills[playerID].equippedSkills.Length;
        for (int i = 0; i < length; i++) {
            // Remove the duplicate if one exists
            if (i != slotID && PartySkills.skills[playerID].equippedSkills[i] == PartySkills.skills[playerID].equippedSkills[slotID]) {
                PartySkills.skills[playerID].equippedSkills[i] = (int)SKILLS.NO_SKILL;
            }
        }
    }

    // Unlocks a skill for the player to use. In order to do this, the method will check if the player can actually use the
    // skill given its ID. If the skill is one that the player can learn, it will be added to the unlocked list. Otherwise,
    // the skill will not be able to be added to the list of unlocked skills.
    public bool AddToUnlockedSkills(int skillID, int playerID) {
        bool unlockSuccess = false;

        // Check if the current player is able to learn this skill
        var length = PartySkills.skills[playerID].unlockableSkills.Length;
        for (int i = 0; i < length; i++) {
            // Skill found, add to unlocked skills
            if (PartySkills.skills[playerID].unlockableSkills[i] == skillID) {
                PartySkills.skills[playerID].learnedSkills[PartySkills.skills[playerID].numSkillsLearned] = skillID;
				PartySkills.skills[playerID].unlockableSkills[i] = (int)SKILLS.NO_SKILL;
                PartySkills.skills[playerID].numSkillsLearned++;
                unlockSuccess = true;
                i = length; // Exit the loop
            }
        }
        return unlockSuccess;
    }

    #endregion

    #region Skill Names

    // Finds the name of the skill relative to the ID provided in the argument
    public string SkillName(int skillID) {
        // Find the name relative to the ID given
        switch (skillID) {
            case (int)SKILLS.TEST_Fargas:
                return "Offense Skill 1";
            case (int)SKILLS.TEST_Frea:
                return "Offense Skill 2";
            case (int)SKILLS.TEST_Oberon:
                return "Buff Skill 1";
            case (int)SKILLS.TEST_Arcelus:
                return "Heal Skill 1";
            case (int)SKILLS.Fa_SwiftStrike:
                return "Swift Strike";
            case (int)SKILLS.Fa_SwordOfFury:
                return "Sword of Fury";
            case (int)SKILLS.Fa_WarCry:
                return "War Cry ";
            case (int)SKILLS.Fa_Sunguard:
                return "The Sunguard Elite";
            case (int)SKILLS.Fa_Rally:
                return "Rally";
            case (int)SKILLS.Fa_BladeOfTheFallen:
                return "Blade Of The Fallen";
            case (int)SKILLS.Fr_DoubleShot:
                return "Double Shot";
            case (int)SKILLS.Fr_PiercingShot:
                return "Piercing Shot";
            case (int)SKILLS.Fr_ArrowRain:
                return "Arrow Rain";
            case (int)SKILLS.Fr_IDontMiss:
                return "I Don't Miss.";
            case (int)SKILLS.Fr_BleedingEdge:
                return "Bleeding Edge";
            case (int)SKILLS.Fr_NeverAgain:
                return "Never Again!";
            case (int)SKILLS.Ob_ShieldAlly:
                return "Shield Ally";
            case (int)SKILLS.Ob_ShieldAllAllies:
                return "Shield All Allies";
            case (int)SKILLS.Ob_SpearDance:
                return "Spear Dance";
            case (int)SKILLS.Ob_LionsPride:
                return "Lion's pride";
            case (int)SKILLS.Ob_FierceStrike:
                return "Fierce Strike";
            case (int)SKILLS.Ob_Lutenist:
                return "The Lutenist of Ocrest";
            case (int)SKILLS.Ar_Heal:
                return "Heal";
            case (int)SKILLS.Ar_HealingAura:
                return "Healing Aura";
            case (int)SKILLS.Ar_DrainEye:
                return "Drain Eye";
            case (int)SKILLS.Ar_LullabyOfHope:
                return "Lullaby Of Hope";
            case (int)SKILLS.Ar_ManaCharge:
                return "Mana Charge";
            case (int)SKILLS.Ar_Armageddon:
                return "Armageddon";
            case (int)SKILLS.Ar_IceAge:
                return "Ice Age";
            default: //In case no skill is equipped at that slot
                return "---";
        }
    }

    #endregion

    #region Skill Descriptions

    // Finds the skill's description relative to the ID provided in the argument parameter
    public string SkillDescription(int skillID) {

        // Find the description relative to the ID given
        switch (skillID) {
            case (int)SKILLS.TEST_Fargas:
                return "Offense skill \n\n1 targets enemies";
            case (int)SKILLS.TEST_Frea:
               return "Offense skill 2 \n\ntargets enemies";
            case (int)SKILLS.TEST_Oberon:
                return "Buff skill 1 \n\ntargets players";
            case (int)SKILLS.TEST_Arcelus:
                return "Heal skill 1 \n\ntargets players";
            case (int)SKILLS.Fa_SwiftStrike:
                return "Inflict a flurry of strikes on a row of enemies.";
            case (int)SKILLS.Fa_SwordOfFury:
                return "Inflict a great deal of damage to one enemy.";
            case (int)SKILLS.Fa_WarCry:
                return "Increase the attack of all allies for three turns. ";
            case (int)SKILLS.Fa_Sunguard:
                return "The Sun shines brightly on all. Chain three enemies together so they all share the damage taken.";
            case (int)SKILLS.Fa_Rally:
                return "Fargas marks one enemy. For three turns, this enemy sustains double damage. Only one enemy can be rallied against at a time.";
            case (int)SKILLS.Fa_BladeOfTheFallen:
                return "Never forget the fallen. Fargas damages all enemies. Damage is increased by the attack points of every dead enemy.";
            case (int)SKILLS.Fr_DoubleShot:
                return "Frea shoots two arrows in succession at one enemy.";
            case (int)SKILLS.Fr_PiercingShot:
                return "A single strong shot aimed at one enemy.";
            case (int)SKILLS.Fr_ArrowRain:
                return "Frea shoots a rain of arrows that damages all enemies.";
            case (int)SKILLS.Fr_IDontMiss:
                return "Frea takes a deep breath and concentrates. Increases base Strength by 30% for three turns. ";
            case (int)SKILLS.Fr_BleedingEdge:
                return "Frea pulls out a dagger and takes matters into her own hands. She inflicts damage X 3 to an enemy and sustains their defense X 2 as damage to herself.";
            case (int)SKILLS.Fr_NeverAgain:
                return "Frea channels all her anger into a single devastating shot that pierces through all enemies.";
            case (int)SKILLS.Ob_ShieldAlly:
                return "Increase the defense of one ally for three turns.";
            case (int)SKILLS.Ob_ShieldAllAllies:
                return "Increase the defense of all allies for three turns.";
            case (int)SKILLS.Ob_SpearDance:
                return "Oberon uses his spear to damage a single row of enemies with an increased chance for critical hits.";
            case (int)SKILLS.Ob_LionsPride:
                return "Doubles Oberon’s defense. He becomes the sole target of all enemy attacks. Lasts for 3 turns.";
            case (int)SKILLS.Ob_FierceStrike:
                return "Wait for the enemy to believe you're beaten, then exact sweet revenge. This attack gets stronger the more damage Oberon sustains.";
            case (int)SKILLS.Ob_Lutenist:
                return "Oberon plays the lute to improve morale. All enemies suffer a random debuff and all allies get a random buff.";
            case (int)SKILLS.Ar_Heal:
                return "Restores a portion of a single alive ally’s HP.";
            case (int)SKILLS.Ar_HealingAura:
                return "Restore a portion of all alive allies' HP.";
            case (int)SKILLS.Ar_DrainEye:
                return "Ally affected by Drain Eye gains health every time they deal damage. Lasts for 3 turns.";
            case (int)SKILLS.Ar_LullabyOfHope:
                return "Arcelus never gives up on a patient. Revives one ally with half of their HP.";
            case (int)SKILLS.Ar_ManaCharge:
                return "The Circle of Mages advises caution when using this spell for its effects on one's life force. Trade HP to restore MP ";
            case (int)SKILLS.Ar_Armageddon:
                return " Sometimes to save someone, another must perish. Arcelus summons pillars of fire burning all enemies.";
            case (int)SKILLS.Ar_IceAge:
                return "Arcelus summons ice damaging a row of enemies and lowering their agility.";
            default: return "";
        }
    }

    #endregion

    #region Skill Stats

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
            case (int)SKILLS.TEST_Fargas:
                skillStat[0] = 25;
                skillStat[1] = 95;
                skillStat[2] = 2;
                skillStat[4] = (float)SKILL_TYPE.ALL_TARGETS_ATK;
                skillStat[5] = 50;
                break;
            case (int)SKILLS.TEST_Frea:
                skillStat[0] = 70;
                skillStat[1] = 80;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_ATK;
                skillStat[5] = 45;
                break;
            case (int)SKILLS.TEST_Oberon:
                skillStat[0] = 50;
                skillStat[1] = 85;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.ALL_PLAYER_BUFF;
                skillStat[5] = 115;
                break;
            case (int)SKILLS.TEST_Arcelus:
                skillStat[0] = 20;
                skillStat[1] = 100;
                skillStat[2] = 2;
                skillStat[4] = (float)SKILL_TYPE.ALL_PLAYER_HEAL;
                skillStat[5] = 40;
                break;
            case (int)SKILLS.Fa_SwiftStrike:
                skillStat[0] = 70;
                skillStat[1] = 16;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.FULL_ROW_ATK;
                skillStat[5] = 50;
                break;
            case (int)SKILLS.Fa_SwordOfFury:
                skillStat[0] = 60;
                skillStat[1] = 16;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_ATK;
                skillStat[5] = 25;
                break;
            case (int)SKILLS.Fa_WarCry:
                skillStat[0] = 0.3f;
                skillStat[1] = 100;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.ALL_PLAYER_BUFF;
                skillStat[5] = 40;
                break;
            case (int)SKILLS.Fa_Sunguard:
                skillStat[0] = 40;
                skillStat[1] = 18;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_ATK;
                skillStat[5] = 60;
                break;
            case (int)SKILLS.Fa_Rally:
                skillStat[0] = 0;
                skillStat[1] = 100;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_DEBUFF;
                skillStat[5] = 60;
                break;
            case (int)SKILLS.Fa_BladeOfTheFallen:
                skillStat[0] = 40;
                skillStat[1] = 18;
                skillStat[2] = 2;
                skillStat[4] = (float)SKILL_TYPE.ALL_TARGETS_ATK;
                skillStat[5] = 100;
                break;
            case (int)SKILLS.Fr_DoubleShot:
                skillStat[0] = 20;
                skillStat[1] = 16;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_ATK;
                skillStat[5] = 25;
                break;
            case (int)SKILLS.Fr_PiercingShot:
                skillStat[0] = 70;
                skillStat[1] = 18;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_ATK;
                skillStat[5] = 50;
                break;
            case (int)SKILLS.Fr_ArrowRain:
                skillStat[0] = 50;
                skillStat[1] = 18;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.ALL_TARGETS_ATK;
                skillStat[5] = 45;
                break;
            case (int)SKILLS.Fr_IDontMiss:
                skillStat[0] = 0.3f;
                skillStat[1] = 100;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_BUFF;
                skillStat[5] = 35;
                break;
            case (int)SKILLS.Fr_BleedingEdge:
                skillStat[0] = 40;
                skillStat[1] = 16;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_ATK;
                skillStat[5] = 35;
                break;
            case (int)SKILLS.Fr_NeverAgain:
                skillStat[0] = 160;
                skillStat[1] = 18;
                skillStat[2] = 2;
                skillStat[4] = (float)SKILL_TYPE.ALL_TARGETS_ATK;
                skillStat[5] = 100;
                break;
            case (int)SKILLS.Ob_ShieldAlly:
                skillStat[0] = 0.3f;
                skillStat[1] = 100;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_BUFF;
                skillStat[5] = 20;
                break;
            case (int)SKILLS.Ob_ShieldAllAllies:
                skillStat[0] = 0.3f;
                skillStat[1] = 100;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.ALL_PLAYER_BUFF;
                skillStat[5] = 40;
                break;
            case (int)SKILLS.Ob_SpearDance:
                skillStat[0] = 60;
                skillStat[1] = 18;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.FULL_ROW_ATK;
                skillStat[5] = 50;
                break;
            case (int)SKILLS.Ob_LionsPride:
                skillStat[0] = 2.0f;
                skillStat[1] = 100;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_BUFF;
                skillStat[5] = 40;
                break;
            case (int)SKILLS.Ob_FierceStrike:
                skillStat[0] = 0;
                skillStat[1] = 18;
                skillStat[2] = 2;
                skillStat[4] = (float)SKILL_TYPE.ALL_TARGETS_ATK;
                skillStat[5] = 100;
                break;
            case (int)SKILLS.Ob_Lutenist:
                skillStat[0] = 0.3f;
                skillStat[1] = 100;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.ALL_PLAYER_HEAL;
                skillStat[5] = 50;
                break;
            case (int)SKILLS.Ar_Heal:
                skillStat[0] = 20;
                skillStat[1] = 100;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                skillStat[5] = 40;
                break;
            case (int)SKILLS.Ar_HealingAura:
                skillStat[0] = 30;
                skillStat[1] = 100;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.ALL_PLAYER_HEAL;
                skillStat[5] = 60;
                break;
            case (int)SKILLS.Ar_DrainEye:
                skillStat[0] = 40;
                skillStat[1] = 100;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                skillStat[5] = 40;
                break;
            case (int)SKILLS.Ar_LullabyOfHope:
                skillStat[0] = 50;
                skillStat[1] = 100;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                skillStat[5] = 60;
                break;
            case (int)SKILLS.Ar_ManaCharge:
                skillStat[0] = 50;
                skillStat[1] = 100;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                skillStat[5] = 50;
                break;
            case (int)SKILLS.Ar_Armageddon:
                skillStat[0] = 70;
                skillStat[1] = 18;
                skillStat[2] = 2;
                skillStat[4] = (float)SKILL_TYPE.ALL_TARGETS_ATK;
                skillStat[5] = 100;
                break;
            case (int)SKILLS.Ar_IceAge:
                skillStat[0] = 50;
                skillStat[1] = 16;
                skillStat[2] = 1;
                skillStat[4] = (float)SKILL_TYPE.FULL_ROW_ATK;
                skillStat[5] = 50;
                break;
            default:
                skillStat[0] = 0;
                skillStat[1] = 0;
                skillStat[2] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                skillStat[5] = 0;
                break;
        }

        return skillStat;
    }

    #endregion
}
