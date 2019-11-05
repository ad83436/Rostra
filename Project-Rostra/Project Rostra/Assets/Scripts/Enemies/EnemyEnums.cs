using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyClassType
{
    DPS,
    Tank,
    Support
};
public enum EnemyState
{
    idle,
    waiting,
    skilling,// used for skills that last more than one turn
};

public enum EnemyAttackType
{
    Dumb,
    Opportunistic,
    Assassin,
    Bruiser,
    Healer,
    Heal_Support,
    Strategist,
    Relentless,
    The_Saint,
    Enemy_Blows_Self,
    Demo
};

public enum PlayerStatReference
{
    Health,
    Agility,
    Defence,
    Attack
};

public enum EnemyStatReference
{
    Health
};

public enum AllEnemySkills
{
    //Default
    No_Skill,

    //Tank
    Ground_Smash,
    Blow_Self,
    Earth_Smash,

    //Dps
    Slice_And_Dice,
    Bite,
    Ball_Roll,
    Attack_Multiple,

    //support
    Increase_Multiple_Stats,
    All_Enemy_Heal,
    Raise_Defence   
};

public enum EnemyName
{
    Bat,
    Boar,
    Dino,
    Dragon,
    Ghost,
    Giant,
    Mimic,
    Mushroom,
    Reptile,
    Slime,
    Snake,
    Octodad,
    Yeti,
    Solider,
    Lieutenant
};
