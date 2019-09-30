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
        Defence

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

    public void EnemyTurn()
    {
        float attackChance = Random.Range(0, 100); // determins if the ememy will use its type attack or a dumb attack 

        if (!dead)
        {
            switch (enemyAttack)
            {
                case EnemyAttackType.Dumb:
                    DumbAttack();
                    break;

                case EnemyAttackType.Opportunistic:
                   // AttackLowDef();
                    break;

                case EnemyAttackType.Assassin:
                    AttackLowHp();
                    break;

                case EnemyAttackType.Bruiser:
              
                    break;

                case EnemyAttackType.Healer:

                    break;

                case EnemyAttackType.Heal_Support:

                    break;

                case EnemyAttackType.Strategist:
                   // AttackHighAgi();
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
                 print(eName + " Attacked " + battleManager.players[i].name + " Who has a health of " + battleManager.players[i].currentHP);
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
                print(eName + " Attacked " + battleManager.players[i].name + " Who has a agility of " + battleManager.players[i].agi);
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
        float whateverStatNeeded;

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
           
            //TODO: check if 2 player have the same agi if so pick one, and also other stuffs
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
            //TODO: check if 2 player have the same agi if so pick one, and also other stuffs
        }
    }

    public void becomeLessVisbile() //Called from UIBTL when this enemy is NOT chosen for attack
    {
        spriteColor.a = 0.5f;
        spriteRenderer.color = spriteColor;
    }

    public void resetVisibility() //Called from UIBTL when this enemy is NOT chosen for attack, and either the player doesn't attack or the attack finishes
    {
        
        spriteColor.a = 1.0f;
        spriteRenderer.color = spriteColor;
    }

    //Calcualte the damage
    public void TakeDamage(float playerAttack)
    {
        float damage = playerAttack - ((eDefence / (20.0f + eDefence)) * playerAttack);
        currentHP -= damage;
        battleManager.enemies[enemyIndexInBattleManager].currentHP = currentHP; //Update the BTL manager with the new health
        HP.fillAmount = currentHP / maxHP;
        animator.SetBool("Hit", true);

        if(currentHP<=0.0f)
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
