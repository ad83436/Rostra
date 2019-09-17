using System.Collections.Generic;
using UnityEngine;

// Code Written By:     Christopher Brine
// Last Updated:        September 15th, 2019

public class SkillsInventory : MonoBehaviour {
    public static SkillsInventory invInstance;  // Holds the current skills inventory instance in a single variable
    public static int MAX_SKILLS = 4;           // The maximum number of skills that one character can have equipped at a single time
    public int[] equippedSkills = new int[MAX_SKILLS];
    public List<int> skillsList = new List<int>();
    public int playerIndex;                     // The player that this skill inventory belongs to

    // The fonts that are used for drawing the Skill Inventory's GUI
    public Font GuiSmall;

    // Variables for selecting options and inventory items
    private int curOption = 0;                  // The current inventory item the player has their cursor over
    private int selectedOption = -1;            // The item that the player has selected in the inventory
    private int subCurOption = 0;               // The current option the player has their cursor over after selecting an item

    // Variables for drawing the skill inventory to the screen
    private int firstToDraw = 0;                // The first item from the inventory to draw out of the full inventory
    private int numToDraw = 15;                 // How many inventory items that can be visible to the player at any given time

    // Set the main skill inventory instance to this one if no skill inventory is active, delete otherwise
    public void Awake() {
        if (invInstance == null) {
            invInstance = this;
        } else {
            Destroy(gameObject);
        }
    }
}
