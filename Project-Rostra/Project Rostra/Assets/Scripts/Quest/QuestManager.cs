// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestManager
{
	public static string questName { get; private set; }
	public static int milestone { get; private set; }
	public static string description { get; private set; }
	public static int location { get; private set; }
	public static string milestoneName { get; private set; }
	public static Dictionary<string, bool> visitedLocals;
	/// <summary>
	/// Okay so let's run through all the locations
	/// 1 = Military camp
	/// 2 = Brenna's house
	/// 3 = Town
	/// 4 = Inn
	/// 5 = Domel's house
	/// This should be all that's needed for now
	/// if this changes I will update this section
	/// </summary>

	//this will cause more info to tick up, just pass it the next number and you're all good
	public static void AddMilestone(int m)
	{
		// I will add cases to this when I have the time, please be patient as I am making a fuckton of cutscenes right now
		switch (m)
		{
			case 1:
				questName = "Find Brenna";
				location = 1;
				description = "Well, I should go and talk to the commander. ";
				milestone = m;
				milestoneName = "Talk to the Commander";
				PauseMenuController.ChangeQuestPopupState(1);
				AddVisitedLocation("Military Camp");
				break;
			case 2:
				questName = "Find Brenna";
				location = 1;
				description = "I need to go find the others, they should be somewhere around the camp.";
				milestone = m;
				milestoneName = "Find the others";
				PauseMenuController.ChangeQuestPopupState(1);
				break;
			case 3:
				questName = "Find Brenna";
				location = 1;
				description = "We should go to the south entrance it's the least guarded";
				milestone = m;
				milestoneName = "Leave Camp";
				PauseMenuController.ChangeQuestPopupState(1);
				break;
			case 4:
				questName = "Find Brenna";
				location = 2;
				description = "We've escaped from the camp, now we should go to Brenna's house";
				milestone = m;
				milestoneName = "Go to Brenna's house";
				PauseMenuController.ChangeQuestPopupState(1);
				AddVisitedLocation("Brenna's House");
				break;
			case 5:
				questName = "Find Brenna";
				location = 3;
				description = "We need to go look for Domel, the barkeep in Hadria should know";
				milestone = m;
				milestoneName = "Go To Hadria and speak to the barkeep";
				PauseMenuController.ChangeQuestPopupState(1);
				AddVisitedLocation("Town");
				break;
			case 6:
				questName = "Find Brenna";
				location = 4;
				description = "I've spoken to the Barkeep, now we just need to find out where our contact is";
				milestone = m;
				milestoneName = "Find the contact";
				PauseMenuController.ChangeQuestPopupState(1);
				AddVisitedLocation("Inn");
				break;
			case 7:
				questName = "Find Brenna";
				location = 5;
				description = "We have all the info we need, we should go find Domel's house";
				milestone = m;
				milestoneName = "Leave the Inn";
				PauseMenuController.ChangeQuestPopupState(1);
				AddVisitedLocation("Domel's house");
				break;
			case 8:
				questName = "Find Brenna";
				location = 3;
				description = "We should go back to Hadria";
				milestone = m;
				milestoneName = "Return to Hadria";
				PauseMenuController.ChangeQuestPopupState(1);
				break;
		}

	}

	public static void AddVisitedLocation(string i)
	{
		if (visitedLocals.ContainsKey(i))
		{
			visitedLocals[i] = true;
		}
		else
		{
			Debug.LogError("Dictionary does not contain the locale you input, fix you peasant!");
		}
	}

	static QuestManager()
	{
		visitedLocals = new Dictionary<string, bool>();
		visitedLocals.Add("Military Camp", false);
		visitedLocals.Add("Brenna's House", false);
		visitedLocals.Add("Town", false);
		visitedLocals.Add("Inn", false);
		visitedLocals.Add("Domel's house", false);
		questName = "";
		location = 0;
		description = "";
		milestone = 0;
		milestoneName = "";
	}

}
