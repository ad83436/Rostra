using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        September 30th, 2019

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
        } else {
            Destroy(gameObject);
        }
    }

    // Handling keyboard functionality
    private void Update() {
        // Getting Keyboard Input
        bool keyOpen, keySelect, keyReturn, keyUp, keyDown, keyLeft, keyRight;
        keyOpen = Input.GetKeyDown(KeyCode.K);
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
        string name = "---";

        // Find the name relative to the ID given
        switch (skillID) {
            case (int)SKILLS.TEST_SKILL1:
                name = "Offense Skill 1";
                break;
            case (int)SKILLS.TEST_SKILL2:
                name = "Offense Skill 2";
                break;
            case (int)SKILLS.TEST_SKILL3:
                name = "Buff Skill 1";
                break;
            case (int)SKILLS.TEST_SKILL4:
                name = "Heal Skill 1";
                break;
            default: //In case no skill is equipped at that slot
                name = "---";
                break;
        }

        return name;
    }

    #endregion

    #region Skill Descriptions

    // Finds the skill's description relative to the ID provided in the argument parameter
    public string SkillDescription(int skillID) {
        string description = "";

        // Find the description relative to the ID given
        switch (skillID) {
            case (int)SKILLS.TEST_SKILL1:
                return "Offense skill 1 targets enemies";
            case (int)SKILLS.TEST_SKILL2:
               return "Offense skill 2 targets enemies";
            case (int)SKILLS.TEST_SKILL3:
                return "Buff skill 1 targets players";
            case (int)SKILLS.TEST_SKILL4:
                return "Heal skill 1 targets players";
        }

        return description;
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
            case (int)SKILLS.TEST_SKILL1:
                skillStat[0] = 25;
                skillStat[1] = 95;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_TARGET_ATK;
                skillStat[5] = 50;
                break;
            case (int)SKILLS.TEST_SKILL2:
                skillStat[0] = 70;
                skillStat[1] = 80;
                skillStat[4] = (float)SKILL_TYPE.ALL_TARGETS_ATK;
                skillStat[5] = 190;
                break;
            case (int)SKILLS.TEST_SKILL3:
                skillStat[0] = 50;
                skillStat[1] = 85;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_BUFF;
                skillStat[5] = 115;
                break;
            case (int)SKILLS.TEST_SKILL4:
                skillStat[0] = 20;
                skillStat[1] = 100;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                skillStat[5] = 40;
                break;
            default:
                skillStat[0] = 0;
                skillStat[1] = 0;
                skillStat[4] = (float)SKILL_TYPE.SINGLE_PLAYER_HEAL;
                skillStat[5] = 0;
                break;
        }

        return skillStat;
    }

    #endregion
}
