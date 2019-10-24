﻿// Written by: Sean Fowke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestManager
{
	public static string questName { get; private set; }
	public static int milestone { get; private set; }
	public static string description { get; private set; }
	public static int location { get; private set; }
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
				questName = "Test Quest";
				location = 1;
				description = "This is a test case in order to test to see if Sean is a good programmer or not. Hint Hint he's not";
				milestone = m;
				break;
				
		}

	}
}