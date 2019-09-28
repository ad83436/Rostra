using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        September 26h, 2019

public class SkillsInventory : MonoBehaviour {
    public static SkillsInventory invInstance;  // Holds the current inventory instance in a single variable

    // The variables that are used for drawing the GUI to the screen
    public Font GuiSmall;
    public bool isVisible = false;

    // Variables for selecting the unlocked skills to equip/uneuqip them
    private int curOption = 0;                  // The current skill the player has their cursor over
    private int selectedOption = -1;            // The skill that the player has selected
    private int subCurOption = 0;               // The current option the player has their cursor over after selecting a skill
    private int playerIndex = 0;                // The current skill inventory that is being looked at (0 = Fargas, 1 = Oberon, etc.)

    // Variables for drawing the unlocked skills list to the screen
    private int firstToDraw = 0;                // The first item from the unlocked skills array to draw out of the full list
    private int numToDraw = 15;                 // The number of skills that are visible to the player at any given time

    // Set the skill inventory instance to this one if no skill inventory is active, delete otherwise
    public void Awake() {
        if (invInstance == null) {
            invInstance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // Handling keyboard functionality
    private void Update() {
        // Getting Keyboard Input
        bool keySelect, keyReturn, keyUp, keyDown, keyLeft, keyRight;
        keySelect = Input.GetKeyDown(KeyCode.Z);
        keyReturn = Input.GetKeyDown(KeyCode.X);
        keyUp = Input.GetKeyDown(KeyCode.UpArrow);
        keyDown = Input.GetKeyDown(KeyCode.DownArrow);
        keyLeft = Input.GetKeyDown(KeyCode.RightArrow);
        keyRight = Input.GetKeyDown(KeyCode.LeftArrow);

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

            // Selecting a Skill
            if (keySelect) {
                selectedOption = curOption;
            }
        }
    }

    // Drawing the Skill Inventory to the Screen
    private void OnGUI() {
        GUIStyle style = new GUIStyle(GUI.skin.label) {
            font = GuiSmall,
            fontSize = 30,
        };
        var fontHeight = style.lineHeight;

        // Drawing the inventory items
        for (int i = firstToDraw; i <= firstToDraw + numToDraw; i++) {
            GUI.Label(new Rect(45.0f, 15.0f + (fontHeight * (i - firstToDraw)), 200.0f, 50.0f), "", style);
            // Drawing a cursor that points to the item the player has highlighted
            GUI.Label(new Rect(25.0f, 15.0f + (fontHeight * (curOption - firstToDraw)), 50.0f, 50.0f), ">", style);
        }
    }

    #region Skill Manipulation Scripts (Adding, Removing, Unlocking)

    // Attempts to equip a skill to the current player's active skills. If the skill list is full, it will let the player swap a
    // currently equipped skill with the newly selected one
    public bool EquipSkill(int skillID) {
        bool emptySlotExists = false;
        // Find an empty slot in the equipped skill list
        var length = PartySkills.skills[playerIndex].equippedSkills.Length;
        for (int i = 0; i < length; i++) {
            // Equip the skill when an empty slot is found
            if (PartySkills.skills[playerIndex].equippedSkills[i] == (int)SKILLS.NO_SKILL) {
                PartySkills.skills[playerIndex].equippedSkills[i] = skillID;
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
            PartySkills.skills[playerIndex].equippedSkills[skillSlot] = (int)SKILLS.NO_SKILL;
            return true;
        }
        return false;
    }

    // Unlocks a skill for the player to use. In order to do this, the method will check if the player can actually use the
    // skill given its ID. If the skill is one that the player can learn, it will be added to the unlocked list. Otherwise,
    // the skill will not be able to be added to the list of unlocked skills.
    public bool AddToUnlockedSkills(int skillID) {
        bool unlockSuccess = false;

        // Check if the current player is able to learn this skill
        var length = PartySkills.skills[playerIndex].unlockableSkills.Length;
        for (int i = 0; i < length; i++) {
            // Skill found, add to unlocked skills
            if (PartySkills.skills[playerIndex].unlockableSkills[i] == skillID) {
                PartySkills.skills[playerIndex].learnedSkills[PartySkills.skills[playerIndex].numSkillsLearned] = skillID;
                PartySkills.skills[playerIndex].numSkillsLearned++;
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

    #endregion

    #region Skill Descriptions

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

    #endregion
}
