using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Code written by: Your moma and I
//Date: Who the fuck knows

public class Enemy : MonoBehaviour
{
    public enum EnemyClassType
    {
        DPS,    
        Tank,   
        Support 
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
        Demo
    };

    enum PlayerStatReference
    {
        Health,
        Agility,
        Defence,
        Attack

    };

    public int enemyIndexInBattleManager;
    private BattleManager battleManager;
    private UIBTL uiBTL;
    public float eMana;
    public float eAttack;
    public float eAgility;
    public float eDefence;
    public float eStrength;
    public float eSpeed;
    public float eBaseLevel;
    public int eCurrentLevel;
    public string eName;
    public float eRange;
    public List <float> playerStatNeeded;
    public float eCritical;
    public Sprite qImage;
    Player attackThisPlayer;

    private SpriteRenderer spriteRenderer;
    private Color spriteColor;
    private Animator animator;

    public Image HP;
    public float maxHP;
    public float currentHP;
    public GameObject enemyCanvas;

    private bool haveAddedMyself;
    public bool dead;
    private bool skillPointAdded;
    public bool hit;

    private GameObject demoEffect;
    private ObjectPooler objPooler;

    public EnemyClassType enemyClass;
    public EnemyAttackType enemyAttack;

    private void Start()
    {
        battleManager = BattleManager.instance;
        objPooler = ObjectPooler.instance;
        uiBTL = UIBTL.instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
        animator = gameObject.GetComponent<Animator>();

        haveAddedMyself = false;
        hit = false;
        dead = false;

        IncreaseStatsBasedOnLevel(eCurrentLevel);
    }

    private void Update()
    {
        //If the enemy is yet to add itself to the Q and the btl manager is currently adding enemies, then add this enemy to the Q
        if (!haveAddedMyself && battleManager.addEnemies)
        {
            AddEnemyToBattle();
            haveAddedMyself = true;
        }

        if (Input.GetKey(KeyCode.G))
        {
            AttackLowHp();
        }
    }

    //Every enemy scales differently based on its warrior class (DPS,Tanks, Support)
    public void IncreaseStatsBasedOnLevel(int enemyCurrentLevel)
    {
        eCurrentLevel = enemyCurrentLevel;
        //eHP increase is still temporary until we agree how much each class'es HP increases with leveling up
        float skillPoints = enemyCurrentLevel - eBaseLevel;

        switch (enemyClass)
        {
            case EnemyClassType.DPS:
                eAttack = Mathf.CeilToInt(eAttack + (skillPoints * 0.6f));
                eAgility = Mathf.CeilToInt(eAgility + (skillPoints * 0.4f));
                currentHP = Mathf.CeilToInt(currentHP + (skillPoints * 35.0f * 0.5f));
                Debug.Log(eName + " is a " + enemyClass + " Class of enemy");
                break;

            case EnemyClassType.Tank:
                eAttack = Mathf.CeilToInt(eAttack + (skillPoints * 0.3f));
                eAgility = Mathf.CeilToInt(eDefence + (skillPoints * 0.7f));
                currentHP = Mathf.CeilToInt(currentHP + (skillPoints * 60.0f * 0.5f));
                Debug.Log(eName + " is a " + enemyClass + " Class of enemy");
                break;

            case EnemyClassType.Support:
                eAttack = Mathf.CeilToInt(eAttack + (skillPoints * 0.4f));
                eAgility = Mathf.CeilToInt(eAttack + (skillPoints * 0.6f));
                currentHP = Mathf.CeilToInt(currentHP + (skillPoints * 85.0f * 0.5f));
                Debug.Log(eName + " is a " + enemyClass + " Class of enemy");
                break;
        }

        maxHP = currentHP;
    }

    public void AddEnemyToBattle()
    {
        battleManager.AddEnemy(enemyIndexInBattleManager, Mathf.RoundToInt(eAgility), Mathf.RoundToInt(eStrength), Mathf.RoundToInt(eCritical), Mathf.RoundToInt(eSpeed), this, name);
    }

    /// <summary>
    /// HEY LISTEN !!!!!!!! Feel Free Change the values in EnemyTurn if you must do so, as these are all test values sho like a good tweaking now and then.
    /// </summary>

    public void EnemyTurn()
    {
        float attackChance = Random.Range(0, 100); // determines if the ememy will use its type attack or a dumb attack 
        float skillChance = Random.Range(0, 60);// determines if the enemy will use a skill or not //TEMP VALUES//

        if (!dead)
        {
            switch (enemyAttack)
            {
                case EnemyAttackType.Dumb:
                    DumbAttack();
                    break;

                case EnemyAttackType.Opportunistic:

                    if (attackChance > 30)
                    {
                        if (skillChance > 40)
                        {
                            AttackLowDef();
                            print(eName + " Used their skill aswell as their types attack");
                        }

                        else
                        {
                            AttackLowDef();
                            print(eName + " Did not use their skill but used their types attack");
                        }
                    }

                    else
                    {
                        if (skillChance > 40) 
                        {
                            DumbAttack();
                            print(eName + " Used their skill aswell as a Dumb attack");
                        }

                        else
                        {
                            DumbAttack();
                            print(eName + " Did not use their skill but used a Dumb attack");
                        }
                    }

                    break;

                case EnemyAttackType.Assassin:

                    if (attackChance > 30)
                    {
                        if (skillChance > 40)
                        {
                            AttackLowHp();
                            print(eName + " Used their skill aswell as their types attack");
                        }

                        else
                        {
                            AttackLowHp();
                            print(eName + " Did not use their skill but used their types attack");
                        }
                    }
            

                    else
                    {
                        if (skillChance > 40)
                        {
                            DumbAttack();
                            print(eName + " Used their skill aswell as a Dumb attack");
                        }

                        else
                        {
                            DumbAttack();
                            print(eName + " Did not use their skill but used a Dumb attack");
                        }
                    }

                    break;

                case EnemyAttackType.Bruiser:
                    if (attackChance > 30)
                    {
                        if (skillChance > 40)
                        {
                            AttackHighAtk();
                            print(eName + " Used their skill aswell as their types attack");
                        }

                        else
                        {
                            AttackHighAtk();
                            print(eName + " Did not use their skill but used their types attack");
                        }
                    }


                    else
                    {
                        if (skillChance > 40)
                        {
                            DumbAttack();
                            print(eName + " Used their skill aswell as a Dumb attack");
                        }

                        else
                        {
                            DumbAttack();
                            print(eName + " Did not use their skill but used a Dumb attack");
                        }
                    }

                    break;

                case EnemyAttackType.Healer:

                    break;

                case EnemyAttackType.Heal_Support:

                    break;

                case EnemyAttackType.Strategist:

                    if (attackChance > 30)
                    {
                        if (skillChance > 40)
                        {
                            AttackHighAgi();
                            print(eName + " Used their skill aswell as their types attack");
                        }

                        else
                        {
                            AttackHighAgi();
                            print(eName + " Did not use their skill but used their types attack");
                        }
                    }

                    else
                    {
                        DumbAttack();
                    }

                    break;

                case EnemyAttackType.Demo:

                    DumbAttack();

                    break;
            }
        }

        else
        {
            uiBTL.EndTurn();
        }
    }

    //Calculate whether the attack is a hit or a miss
    private void CalculateHit()
    {
        //20 sided die + str <? enemy agility
        if (Random.Range(0.0f, 20.0f) + eStrength < attackThisPlayer.agi)
        {
            hit = false;
        }
        else
        {
            hit = true;
        }

        Debug.Log("Enemy Hit is " + hit);
    }

    private float CalculateCrit()
    {
        return Random.Range(0.0f, 100.0f);
    }

    void DumbAttack()
    {
        attackThisPlayer = battleManager.players[Random.Range(0,4)].playerReference;
        //if the player is dead, try again
        if (attackThisPlayer.currentHP <= 0.0f)
        {
            DumbAttack();
        }

        else
        {
            //Run the animation
            CalculateHit();
            animator.SetBool("Attack", true);
        }
        
    }

    private void DemoAttackEffect()
    {
        demoEffect = objPooler.SpawnFromPool("DemoAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
    }

    //Called from the animator once the attack anaimation ends
    private void CompleteAttack()
    {
        if (hit)
        {
            objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
            if(CalculateCrit() <= eCritical)
            {
                Debug.Log("Critical Hit from Enemy");
                attackThisPlayer.TakeDamage(eAttack * 1.2f);
            }

            else
            {
                attackThisPlayer.TakeDamage(eAttack);
            }           
        }

        else
        {
            Debug.Log("Enemy has missed");
        }
        animator.SetBool("Attack", false);
        uiBTL.EndTurn();
    }

    void AttackLowHp()
    {
        StatNeeded(PlayerStatReference.Health);
         for (int i = 0; i < 4; i++)
         {
            //checking if hp stat for BM is the same as the smallest value in the list is so do yo thang 
             if (battleManager.players[i].currentHP == Mathf.Min(playerStatNeeded.ToArray()))
             {
                attackThisPlayer = battleManager.players[i].playerReference;                
                 CalculateHit();
                 animator.SetBool("Attack", true);
             }
         }
         //clear the list for the next use 
        playerStatNeeded.Clear();
    }

    void AttackLowDef()
    {
        StatNeeded(PlayerStatReference.Defence);
        for (int i = 0; i < 4; i++)
        {
            if (battleManager.players[i].def == Mathf.Min(playerStatNeeded.ToArray()))
            {
                attackThisPlayer = battleManager.players[i].playerReference;
                print(eName + " Attacked " + battleManager.players[i].name + " Who has a defence of " + battleManager.players[i].def);
                CalculateHit();
                animator.SetBool("Attack", true);
            }
        }

        //clear the list for the next use 
        playerStatNeeded.Clear();
    }

    void AttackHighAgi()
    {
        StatNeeded(PlayerStatReference.Agility);
        for (int i = 0; i < 4; i++)
        {
            if (battleManager.players[i].agi == Mathf.Max(playerStatNeeded.ToArray()))
            {
                attackThisPlayer = battleManager.players[i].playerReference;
                CalculateHit();
                animator.SetBool("Attack", true);
            }
        }

        //clear the list for the next use 
        playerStatNeeded.Clear();
    }

    void AttackHighAtk()
    {
        StatNeeded(PlayerStatReference.Attack);
        for (int i = 0; i < 4; i++)
        {
            if (battleManager.players[i].atk == Mathf.Max(playerStatNeeded.ToArray()))
            {
                attackThisPlayer = battleManager.players[i].playerReference;
                CalculateHit();
                animator.SetBool("Attack", true);
            }
        }

        //clear the list for the next use 
        playerStatNeeded.Clear();
    }

    // returns the stat needed for the enemies that attack based on player stats 
    void StatNeeded(PlayerStatReference statNeeded)
    {
        float statsRefForCheck; //  i know shitty name 
        //returns the lowest HP of the party 
        if (statNeeded == PlayerStatReference.Health)
        {
            foreach (BattleManager.PlayerInformtion stat in battleManager.players)
            {
                //checks if player current hp from battlemanager is above 0 and if player exist if that's the case add to list
                if (stat.playerReference != null && stat.currentHP > 0)
                {
                    playerStatNeeded.Add(stat.currentHP);
                }
            }
            //sort the list DUH
            playerStatNeeded.Sort();

            statsRefForCheck = Mathf.Min(playerStatNeeded.ToArray());

            for (int i = 0; i <playerStatNeeded.Count; i++)
            {
                if(playerStatNeeded[i] != statsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                    print("Removed" + battleManager.players[i].name);
                }
                
                else if (playerStatNeeded[i] == statsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded.Count - 1);
                }
            }
        }
        

        else if(statNeeded == PlayerStatReference.Agility)
        {
            foreach (BattleManager.PlayerInformtion stat in battleManager.players)
            {
                //same idea as with hp
                if (stat.playerReference != null && stat.currentHP > 0)
                {
                    playerStatNeeded.Add(stat.agi);
                }
            }
         
            playerStatNeeded.Sort();

            statsRefForCheck = Mathf.Max(playerStatNeeded.ToArray());

            for (int i = 0; i < playerStatNeeded.Count; i++)
            {
                if (playerStatNeeded[i] != statsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                    print("Removed" + battleManager.players[i].name);
                }
                
                else if (playerStatNeeded[i] == statsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded.Count - 1);
                }
            }
        }

        else if (statNeeded == PlayerStatReference.Attack)
        {
            foreach (BattleManager.PlayerInformtion stat in battleManager.players)
            {
                //same idea as with hp
                if (stat.playerReference != null && stat.currentHP > 0)
                {
                    playerStatNeeded.Add(stat.atk);
                }
            }

            playerStatNeeded.Sort();

            statsRefForCheck = Mathf.Max(playerStatNeeded.ToArray());

            for (int i = 0; i < playerStatNeeded.Count; i++)
            {
                if (playerStatNeeded[i] != statsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                    print("Removed" + battleManager.players[i].name);
                }
               
                else if (playerStatNeeded[i] == statsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded.Count - 1);
                }
            }
        }

        else
        {
            //you get it by now 
            foreach (BattleManager.PlayerInformtion stat in battleManager.players)
            {
                //same idea as with the other two ... just incase you forgot if you are Dead you didnt make the cut
                if (stat.playerReference != null && stat.currentHP > 0)
                {
                    playerStatNeeded.Add(stat.def);
                }
            }
            playerStatNeeded.Sort();

            statsRefForCheck = Mathf.Min(playerStatNeeded.ToArray());
            for (int i = 0; i < playerStatNeeded.Count; i++)
            {
                if (playerStatNeeded[i] != statsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                    print("Removed" + battleManager.players[i].name);
                }
               
                else if (playerStatNeeded[i] == statsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded.Count - 1);
                }
            }
        }
    }


    //Calcualte the damage
    public void TakeDamage(float playerAttack)
    {
        Debug.Log("Received player attack: " + playerAttack);
        float damage = playerAttack - ((eDefence / (20.0f + eDefence)) * playerAttack);
        currentHP -= damage;
        battleManager.enemies[enemyIndexInBattleManager].currentHP = currentHP; //Update the BTL manager with the new health
        HP.fillAmount = currentHP / maxHP;
        animator.SetBool("Hit", true);

        if (currentHP <= 0.0f)
        {
            Death();
        }
    }

    public void EndHitAnimation()
    {
        animator.SetBool("Hit", false);
    }

    private void Death()
    {
        spriteRenderer.enabled = false;
        enemyCanvas.SetActive(false);
        dead = true;
        uiBTL.EnemyIsDead(enemyIndexInBattleManager);
    }
}
