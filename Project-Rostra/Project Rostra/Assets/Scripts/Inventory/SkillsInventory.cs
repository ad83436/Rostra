using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        September 17th, 2019

public class SkillsInventory : MonoBehaviour {
    public static SkillsInventory invInstance; // Holds the current inventory instance in a single variable

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
    }

    // Drawing the Skill Inventory to the Screen
    private void OnGUI() {
        
    }

    // Unlocks a skill for the player to use. In order to do this, the method will check if the player can actually use the
    // skill given its ID. If the skill is one that the player can learn, it will be added to the unlocked list. Otherwise,
    // the skill will not be able to be added to the list of unlocked skills.
    public bool AddToUnlockedSkills(int skillID) {
        bool unlockSuccess = false;

        // Check if the current player is able to learn this skill
        PlayerSkills player = CharacterSkills.skills[playerIndex];
        var length = player.unlockableSkills.Length;
        for (int i = 0; i < length; i++) {
            // Skill found, add to unlocked skills
            if (player.unlockableSkills[i] == skillID) {
                player.learnedSkills[player.numSkillsLearned] = skillID;
                player.numSkillsLearned++;
                unlockSuccess = true;
                i = length; // Exit the loop
            }
        }

        return unlockSuccess;
    }
}
