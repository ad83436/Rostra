using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// oo oo ah ah
[System.Serializable]
public class SaveData 
{
    public float[] maxHealth;
    public float[] currentHealth;
    public float[] maxMP;
    public float[] currentMP;
    public float[] attack;
    public float[] strength;
    public float[] critical;
    public float[] agility;
    public float[] rage;
    public float[] speed;
    public int[] level;
    public int[] currentXP;
    public int[] neededXP;
    public int[] statPoints;
    public float[] position;
    public int location;
    public int milestone;
    public int money;
    public int[,] inventory;
    public int[] equippedSkills;
    public int[] learnedSkills;

    public SaveData (Player player)
    {
        maxHealth[0] = PartyStats.chara[0].maxHealth;
        maxHealth[1] = PartyStats.chara[1].maxHealth;
        maxHealth[2] = PartyStats.chara[2].maxHealth;
        maxHealth[3] = PartyStats.chara[3].maxHealth;

        currentHealth[0] = PartyStats.chara[0].hitpoints;
        currentHealth[1] = PartyStats.chara[1].hitpoints;
        currentHealth[2] = PartyStats.chara[2].hitpoints;
        currentHealth[3] = PartyStats.chara[3].hitpoints;

        currentMP[0] = PartyStats.chara[0].magicpoints;
        currentMP[1] = PartyStats.chara[1].magicpoints;
        currentMP[2] = PartyStats.chara[2].magicpoints;
        currentMP[3] = PartyStats.chara[3].magicpoints;

        maxMP[0] = PartyStats.chara[0].maxMana;
        maxMP[1] = PartyStats.chara[1].maxMana;
        maxMP[2] = PartyStats.chara[2].maxMana;
        maxMP[3] = PartyStats.chara[3].maxMana;

        attack[0] = PartyStats.chara[0].attack;
        attack[1] = PartyStats.chara[1].attack;
        attack[2] = PartyStats.chara[2].attack;
        attack[3] = PartyStats.chara[3].attack;

        strength[0] = PartyStats.chara[0].strength;
        strength[1] = PartyStats.chara[1].strength;
        strength[2] = PartyStats.chara[2].strength;
        strength[3] = PartyStats.chara[3].strength;

        critical[0] = PartyStats.chara[0].critical;
        critical[1] = PartyStats.chara[1].critical; 
        critical[2] = PartyStats.chara[2].critical;
        critical[3] = PartyStats.chara[3].critical;

        agility[0] = PartyStats.chara[0].agility;
        agility[1] = PartyStats.chara[1].agility;
        agility[2] = PartyStats.chara[2].agility;
        agility[3] = PartyStats.chara[3].agility;

        rage[0] = PartyStats.chara[0].rage;
        rage[1] = PartyStats.chara[1].rage;
        rage[2] = PartyStats.chara[2].rage;
        rage[3] = PartyStats.chara[3].rage;

        speed[0] = PartyStats.chara[0].speed;
        speed[1] = PartyStats.chara[1].speed;
        speed[2] = PartyStats.chara[2].speed;
        speed[3] = PartyStats.chara[3].speed;

        currentXP[0] = PartyStats.chara[0].currentExperience;
        currentXP[1] = PartyStats.chara[1].currentExperience;
        currentXP[2] = PartyStats.chara[2].currentExperience;
        currentXP[3] = PartyStats.chara[3].currentExperience;

        neededXP[0] = PartyStats.chara[0].neededExperience;
        neededXP[1] = PartyStats.chara[1].neededExperience;
        neededXP[2] = PartyStats.chara[2].neededExperience;
        neededXP[3] = PartyStats.chara[3].neededExperience;

        level[0] = PartyStats.chara[0].level;
        level[1] = PartyStats.chara[1].level;
        level[2] = PartyStats.chara[2].level;
        level[3] = PartyStats.chara[3].level;

        statPoints[0] = PartyStats.chara[0].statPoints;
        statPoints[1] = PartyStats.chara[1].statPoints;
        statPoints[2] = PartyStats.chara[2].statPoints;
        statPoints[3] = PartyStats.chara[3].statPoints;

        location = QuestManager.location;

        milestone = QuestManager.milestone;

        money = MainInventory.totalMoney;

        inventory = MainInventory.invItem;

        equippedSkills = CharacterSkills.equippedSkills;

        learnedSkills = CharacterSkills.learnedSkills;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
