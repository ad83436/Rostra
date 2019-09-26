using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        September 24th, 2019

public class SkillsInventory : MonoBehaviour {
    public static SkillsInventory invInstance;  // Holds the current inventory instance in a single variable
    public CharacterSkills currentPlayer;       // Holds whatever player'sskill are currently being viewed right now

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
                int numSkills = currentPlayer.numSkillsLearned;
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
                int numSkills = currentPlayer.numSkillsLearned;
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
                currentPlayer = PartySkills.skills[playerIndex];
            } else if (keyLeft) {
                playerIndex--;
                // Loop back to the last character's skill inventory
                if (playerIndex < 0) {
                    playerIndex = 3;
                }
                currentPlayer = PartySkills.skills[playerIndex];
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

    // Unlocks a skill for the player to use. In order to do this, the method will check if the player can actually use the
    // skill given its ID. If the skill is one that the player can learn, it will be added to the unlocked list. Otherwise,
    // the skill will not be able to be added to the list of unlocked skills.
    public bool AddToUnlockedSkills(int skillID) {
        bool unlockSuccess = false;

        // Check if the current player is able to learn this skill
        var length = currentPlayer.unlockableSkills.Length;
        for (int i = 0; i < length; i++) {
            // Skill found, add to unlocked skills
            if (currentPlayer.unlockableSkills[i] == skillID) {
                currentPlayer.learnedSkills[currentPlayer.numSkillsLearned] = skillID;
                currentPlayer.numSkillsLearned++;
                unlockSuccess = true;
                i = length; // Exit the loop
            }
        }

        return unlockSuccess;
    }
}
