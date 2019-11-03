using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Enemy : MonoBehaviour
{
    public int enemyIndexInBattleManager;
    protected BattleManager battleManager;
    protected UIBTL uiBTL;
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
    int amountModed;
    public int waitTime;
    int waitTimeAtStart;
    public int playerIndexHolder;
    public int timeAttacking;
    int countDownToBlow;
    public int blowStrength;
    int enemyStartingAtk;
    int enemyStartingDefence;
    int[] skills;
    private int defenceMod;
    public List<float> playerStatNeeded;
    public List<float> enemyStatNeeded;
    public float eCritical;
    public Sprite qImage;
    protected Player attackThisPlayer;
    Enemy theHealer;
    Enemy enemyToHeal;

    protected SpriteRenderer spriteRenderer;
    protected Color spriteColor;
    protected Animator animator;

    public Image HP;
    public float maxHP;
    public float currentHP;
    public Text damageText;
    public Text healText;
    public GameObject enemyCanvas;

    protected bool haveAddedMyself;
    public bool dead;
    protected bool skillPointAdded;
    public bool hit;
    public bool blow;
    public bool isStatModed;
    [SerializeField] bool skillNeedsCharge; // check if the skill in use uses a charge time or the skill is instantly activated 

    protected GameObject demoEffect;
    protected ObjectPooler objPooler;

    public EnemyClassType enemyClass;
    public EnemyAttackType enemyAttack;
    public AllEnemySkills canUseSkill;
    public EnemyName enemyName;
    PlayerStatReference statNeeded;

    [SerializeField] protected EnemyState currentState;
    protected int waitQTurns = 0; //Update this when you want a skill to have wait time
    public Text waitTurnsText;

    protected Player tieThisPlayer; 
    protected int tiedTimer = 4; //Used to nullify the tied player reference whenever the Farea takes damage or gets healed 
    public GameObject healthObject; 
    public GameObject chain;


    protected void Awake()
    {
        print("Very First  " + currentHP);
        IncreaseStatsBasedOnLevel(eCurrentLevel);
        AssingClassSkills(this);
        GiveNamesAndSkills();
    }
    protected virtual void Start()
    {
        print("First" + currentHP);

        amountModed = 0;
        timeAttacking = 1;
        waitTimeAtStart = waitTime;
        countDownToBlow = Random.Range(5, 10);

        battleManager = BattleManager.instance;
        objPooler = ObjectPooler.instance;
        uiBTL = UIBTL.instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
        animator = gameObject.GetComponent<Animator>();

        damageText.gameObject.SetActive(false);
        healText.gameObject.SetActive(false);

        haveAddedMyself = false;
        hit = false;
        dead = false;
        isStatModed = false;
        blow = false;


        ChoosePlayer();
        enemyStartingAtk = Mathf.CeilToInt(eAttack);
        enemyStartingDefence = Mathf.CeilToInt(eDefence);
        print(currentHP);

        currentState = EnemyState.idle;
        //waitTurnsText.gameObject.SetActive(false);
    }
    protected virtual void Update()
    {
        //If the enemy is yet to add itself to the Q and the btl manager is currently adding enemies, then add this enemy to the Q
        if (!haveAddedMyself && battleManager.addEnemies)
        {
            AddEnemyToBattle();
            haveAddedMyself = true;
        }

    }
    //Every enemy scales differently based on its warrior class (DPS,Tanks, Support)
    // also assigns skills that only certain types can use 
    public virtual void AddEnemyToBattle()
    {
        battleManager.AddEnemy(enemyIndexInBattleManager, Mathf.RoundToInt(eAgility), Mathf.RoundToInt(eStrength), Mathf.RoundToInt(eCritical), Mathf.RoundToInt(eSpeed), Mathf.RoundToInt(currentHP), Mathf.RoundToInt(maxHP), this, name);
    }
    /// <summary>
    /// HEY LISTEN !!!!!!!! Feel Free Change the values in EnemyTurn if you must do so, as these are all test values sho like a good tweaking now and then.
    /// </summary>
    /// 
    public virtual void EnemyTurn()
    {
        uiBTL.DisableActivtyText();

        if (currentState == EnemyState.waiting)
        {
            waitTime--;
            waitQTurns--;
            //waitTurnsText.text = waitQTurns.ToString(); //Update the UI
            if (waitQTurns <= 0)
            {
                //waitTurnsText.gameObject.SetActive(false); //Turn off the text. Don't forget to enable it when the enemy goes to waiting state
                MakeSkillsWork(canUseSkill);
             
                //Execute skill here 
            }

            else
            {
                //End the turn
                uiBTL.EndTurn();
            }

        }

        else if (currentState == EnemyState.skilling)
        {
            waitQTurns--;
            waitTime--;
            if (waitQTurns <= 0) { MakeSkillsWork(canUseSkill); }
            EndTurn();
        }

        else
        {
            float attackChance = Random.Range(0, 100); // determines if the ememy will use its type attack or a dumb attack 
            float skillChance = Random.Range(0, 50);// determines if the enemy will use a skill or not //TEMP VALUES//

            if (!dead)
            {
                switch (enemyAttack)
                {
                    case EnemyAttackType.Dumb:

                        if (skillChance >= 47)
                        {
                            if (skillNeedsCharge)
                            {
                                print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                currentState = EnemyState.waiting;
                                waitQTurns = waitTime;
                                animator.SetBool("isWaiting", true);
                                EndTurn();
                            }

                            else
                            {
                                print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                currentState = EnemyState.skilling;
                                waitQTurns = waitTime;
                                animator.SetBool("SkillInUse", true);
                                MakeSkillsWork(canUseSkill);
                                EndTurn();
                            }
                        }

                        else
                        {
                            DumbAttack();
                        }
                        break;

                    case EnemyAttackType.Opportunistic:

                        if (attackChance > 30)
                        {
                            if (skillChance >= 45)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                AttackHighAgi();
                            }
                        }

                        else
                        {
                            if (skillChance >= 50)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                   
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
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
                            if (skillChance >= 45)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
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
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
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
                            if (skillChance >= 45)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                AttackHighAtk();
                            }
                        }

                        else
                        {
                            if (skillChance >= 50)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                DumbAttack();
                            }
                        }
                        break;

                    case EnemyAttackType.Healer:

                        if (skillChance >= 49)
                        {
                            if (skillNeedsCharge)
                            {
                                print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                currentState = EnemyState.waiting;
                                waitQTurns = waitTime;
                                animator.SetBool("isWaiting", true);
                                EndTurn();
                            }

                            else
                            {
                                print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                currentState = EnemyState.skilling;
                                waitQTurns = waitTime;
                                animator.SetBool("SkillInUse", true);
                                MakeSkillsWork(canUseSkill);
                                EndTurn();
                            }
                        }

                        else
                        {
                            HealEnemy();
                        }
                        break;

                    case EnemyAttackType.Heal_Support:

                        if (skillChance >= 49)
                        {
                            if (skillNeedsCharge)
                            {
                                print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                currentState = EnemyState.waiting;
                                waitQTurns = waitTime;
                                animator.SetBool("isWaiting", true);
                                EndTurn();
                            }

                            else
                            {
                                print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                currentState = EnemyState.skilling;
                                waitQTurns = waitTime;
                                animator.SetBool("SkillInUse", true);
                                MakeSkillsWork(canUseSkill);
                                EndTurn();
                            }
                        }

                        else
                        {
                            SupportHeal(theHealer);
                        }
                        break;

                    case EnemyAttackType.Strategist:

                        if (attackChance > 30)
                        {
                            if (attackChance >= 45)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                AttackHighAgi();
                                print(eName + " Did not use their skill but used their types attack");
                            }
                        }

                        else
                        {
                            if (skillChance >= 50)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                DumbAttack();
                            }
                        }
                        break;

                    case EnemyAttackType.Demo:

                        DumbAttack();
                        break;

                    case EnemyAttackType.Relentless:

                        if (attackChance > 30)
                        {
                            if (skillChance >= 45)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                RelentlessAttack(playerIndexHolder, timeAttacking);
                            }
                        }

                        else
                        {
                            if (skillChance >= 50)
                            {
                                if (skillNeedsCharge)
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                {
                                    print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                DumbAttack();
                            }
                        }
                        break;

                    case EnemyAttackType.Enemy_Blows_Self:

                        if (skillChance >= 49)
                        {
                            if (skillNeedsCharge)
                            {
                                print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                currentState = EnemyState.waiting;
                                waitQTurns = waitTime;
                                animator.SetBool("isWaiting", true);
                                EndTurn();
                            }

                            else
                            {
                                print("Enemy At Index" + enemyIndexInBattleManager + " Used Skill");
                                currentState = EnemyState.skilling;
                                waitQTurns = waitTime;
                                animator.SetBool("SkillInUse", true);
                                MakeSkillsWork(canUseSkill);
                                EndTurn();
                            }
                        }

                        else
                        {
                            BlowSelf();
                        }
                        break;
                }
            }

            else
            {
                uiBTL.EndTurn();
            }
        }
    }
    //Calculate whether the attack is a hit or a miss
    protected virtual void CalculateHit()
    {
        //20 sided die + str <? enemy agility
        if (Random.Range(0.0f, 20.0f) + eStrength < attackThisPlayer.agi)
        {
            hit = false;
        }

        else
        {
            hit = true;
            timeAttacking += 1;
        }

        Debug.Log("Enemy Hit is " + hit);
    }
    protected virtual float CalculateCrit()
    {
        return Random.Range(0.0f, 100.0f);
    }
    protected void DemoAttackEffect()
    {
        demoEffect = objPooler.SpawnFromPool("DemoAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
    }
    //Called from the animator once the attack anaimation ends
    protected virtual void CompleteAttack()
    {
        //  float critMod = 1.2f;

        if (hit)
        {
            objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);

            if (CalculateCrit() <= eCritical)
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
    protected void DumbAttack()
    {
        attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;
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
    void AttackLowHp()
    {
        statNeeded = PlayerStatReference.Health;
        StatNeeded(statNeeded, playerStatNeeded);

        for (int i = 0; i < battleManager.players.Length; i++)
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
        statNeeded = PlayerStatReference.Defence;
        StatNeeded(statNeeded, playerStatNeeded);
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
        statNeeded = PlayerStatReference.Agility;
        StatNeeded(statNeeded, playerStatNeeded);
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
        statNeeded = PlayerStatReference.Attack;
        StatNeeded(statNeeded, playerStatNeeded);
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
    void RelentlessAttack(int playerIndex, int timeAttacking)
    {
        float attackMod;
        attackThisPlayer = battleManager.players[playerIndex].playerReference;

        if (attackThisPlayer.currentHP <= 0)
        {
            ChoosePlayer();
            RelentlessAttack(playerIndexHolder, timeAttacking);
            timeAttacking = 1;
        }

        if (timeAttacking == 1)
        {
            print("enemy has attacked " + timeAttacking + " time(s) since the start of the battle");
            CalculateHit();
            animator.SetBool("Attack", true);
            ++timeAttacking;
        }

        else if (timeAttacking == 2)
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.1f);
            eAttack += attackMod;
            print("enemy has attacked " + timeAttacking + " time(s) since the start of the battle with a attack of " + eAttack);
            ++timeAttacking;
            CalculateHit();
            animator.SetBool("Attack", true);

        }

        else if (timeAttacking == 3)
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.2f);
            eAttack += attackMod;
            print("enemy has attacked " + timeAttacking + " time(s) since the start of the battle with a attack of " + eAttack);
            ++timeAttacking;
            CalculateHit();
            animator.SetBool("Attack", true);


        }

        else if (timeAttacking == 4)
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.3f);
            eAttack += attackMod;
            print("enemy has attacked " + timeAttacking + " time(s) since the start of the battle with a attack of " + eAttack);
            ++timeAttacking;
            CalculateHit();
            animator.SetBool("Attack", true);
        }

        else if (timeAttacking == 5)
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.4f);
            eAttack += attackMod;
            print("enemy has attacked " + timeAttacking + " time(s) since the start of the battle with a attack of " + eAttack);
            ++timeAttacking;
            CalculateHit();
            animator.SetBool("Attack", true);

        }

        else
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.5f);
            eAttack += attackMod;
            print("enemy has attacked " + timeAttacking + " time(s) since the start of the battle with a attack of " + eAttack);
            CalculateHit();
            animator.SetBool("Attack", true);
        }

        eAttack = enemyStartingAtk;
    }
    void BlowSelf()
    {
        countDownToBlow--;
        blowStrength += Random.Range(10, 15);

        if (countDownToBlow <= 0)
        {
            blow = true;
            currentHP = 0;

        }

        if (blow)
        {
            print("Enemy blew self with a blow strength of " + blowStrength);
            AttackWholeField(blowStrength);
            blow = false;
            animator.SetBool("Death", true);
        }

        uiBTL.EndTurn();
    }
    void HealEnemy()
    {
        int[] healthHolder = new int[battleManager.enemies.Length];// why i did this i will never know change it when not too lazy
        float lowestHealth; //holds ref the lowest health in enemyStat List
        int healthMod = Random.Range(5, 20);// how much health should be applied to the enemies currentHP
        float chanceOfHealth = Random.Range(0.2f, 0.9f); //  how low should  the health be before it is healed //CHANGE THIS FUCKING VARIABLE NAME ANDRE!!

        for (int i = 0; i < healthHolder.Length; ++i)
        {
            //add enemies currentHp to enemyStat List only if they arnt dead
            if (battleManager.enemies[i].currentHP > 0 && !dead)
            {
                enemyStatNeeded.Add(battleManager.enemies[i].currentHP);
            }
        }

        lowestHealth = Mathf.Min(enemyStatNeeded.ToArray());

        for (int i = 0; i < enemyStatNeeded.Count; ++i)
        {
            //if enemyStat is not the lowestHealth Remove it 
            if (enemyStatNeeded[i] != lowestHealth)
            {
                enemyStatNeeded.RemoveAt(i);
            }
        }

        for (int i = 0; i < healthHolder.Length; ++i)
        {
            if (battleManager.enemies[i].currentHP == lowestHealth)
            {
                enemyToHeal = battleManager.enemies[i].enemyReference;
            }
        }

        //if the enemy that needs to be healths current hp is less that a percentage of max hp plus 0.1 so that its never below 20%
        if (enemyToHeal.currentHP <= (enemyToHeal.maxHP * chanceOfHealth))
        {
            if (enemyToHeal.currentHP + healthMod >= enemyToHeal.maxHP)
            {
                enemyToHeal.currentHP = enemyToHeal.maxHP;
                battleManager.enemies[enemyToHeal.enemyIndexInBattleManager].currentHP = enemyToHeal.currentHP;
            }

            else
            {
                enemyToHeal.currentHP += healthMod;
                battleManager.enemies[enemyToHeal.enemyIndexInBattleManager].currentHP = enemyToHeal.currentHP;
            }
            uiBTL.EndTurn();
        }

        else
        {
            DumbAttack();
        }
        enemyStatNeeded.Clear();
    }
    void SupportHeal(Enemy theHealer)
    {
        int statMod; // temp values 
        List<Enemy> enemyToHealRef = new List<Enemy>(); //list of all the enemies other then the instance calling this function
        int randomStat = Random.Range(0, 2); //pick a random stat to add to 
        int randomStatAtk = Random.Range(0, 3);//pick a random stat based Attack
        int healAtIndex;

        //add enemies to the list
        for (int i = 0; i < battleManager.enemies.Length; ++i)
        {
            if (battleManager.enemies[i].enemyReference == this)
            {
                theHealer = battleManager.enemies[i].enemyReference; // which enemy instance is calling this function he is  the healer 
            }

            if (battleManager.enemies[i].enemyReference != this)
            {
                if (battleManager.enemies[i].enemyReference != null && battleManager.enemies[i].currentHP > 0)
                {
                    enemyToHealRef.Add(battleManager.enemies[i].enemyReference); // all the other enemies in the battle go into here 
                }
            }
        }

        if (enemyToHealRef.Count == 5)
        {
            healAtIndex = PickRandomNumber(enemyToHealRef[0].enemyIndexInBattleManager, enemyToHealRef[1].enemyIndexInBattleManager, enemyToHealRef[2].enemyIndexInBattleManager, enemyToHealRef[3].enemyIndexInBattleManager, enemyToHealRef[4].enemyIndexInBattleManager);
            print("Heal at Index " + healAtIndex);
            print(enemyToHealRef[healAtIndex].enemyIndexInBattleManager);
            enemyToHeal = battleManager.enemies[healAtIndex].enemyReference;
        }

        else if (enemyToHealRef.Count == 4)
        {
            healAtIndex = PickRandomNumber(enemyToHealRef[0].enemyIndexInBattleManager, enemyToHealRef[1].enemyIndexInBattleManager, enemyToHealRef[2].enemyIndexInBattleManager, enemyToHealRef[3].enemyIndexInBattleManager);
            print("Heal at Index " + healAtIndex);

            enemyToHeal = battleManager.enemies[healAtIndex].enemyReference;
        }

        else if (enemyToHealRef.Count == 3)
        {
            healAtIndex = PickRandomNumber(enemyToHealRef[0].enemyIndexInBattleManager, enemyToHealRef[1].enemyIndexInBattleManager, enemyToHealRef[2].enemyIndexInBattleManager);
            print("Heal at Index " + healAtIndex);
            print(enemyToHealRef[healAtIndex].enemyIndexInBattleManager);
            enemyToHeal = battleManager.enemies[healAtIndex].enemyReference; ;
        }

        else if (enemyToHealRef.Count == 2)
        {
            healAtIndex = PickRandomNumber(enemyToHealRef[0].enemyIndexInBattleManager, enemyToHealRef[1].enemyIndexInBattleManager);
            print("Heal at Index " + healAtIndex);
            enemyToHeal = battleManager.enemies[healAtIndex].enemyReference;
        }

        else
        {
            healAtIndex = enemyToHealRef[0].enemyIndexInBattleManager;
            print("Heal at Index " + healAtIndex);
            print(enemyToHealRef[healAtIndex].enemyIndexInBattleManager);
            enemyToHeal = enemyToHealRef[healAtIndex];
        }

        if (enemyToHeal.isStatModed)
        {
            if (theHealer.eAttack > enemyToHeal.eAttack && randomStat == 0)
            {
                enemyToHeal.amountModed++;
                statMod = Random.Range(5, 20);
                enemyToHeal.eAttack += statMod;
                print(enemyToHeal.eName + " At index " + enemyToHeal.enemyIndexInBattleManager + " had " + statMod + " Added to it's Attack");
                print("enemy at index " + enemyToHeal.enemyIndexInBattleManager + " Had stats moded " + enemyToHeal.amountModed + " time(s) ");
                uiBTL.EndTurn();
            }

            else if (theHealer.eDefence > enemyToHeal.eDefence && randomStat == 1)
            {
                enemyToHeal.amountModed++;
                statMod = Random.Range(5, 20);
                enemyToHeal.eDefence += statMod;
                print(enemyToHeal.eName + " At index " + enemyToHeal.enemyIndexInBattleManager + "had " + statMod + " Added to it's Defence");
                print("enemy at index " + enemyToHeal.enemyIndexInBattleManager + " Had stats moded " + enemyToHeal.amountModed + " time(s) ");
                uiBTL.EndTurn();
            }

            else if (theHealer.eAgility > enemyToHeal.eAgility && randomStat == 2)
            {
                enemyToHeal.amountModed++;
                statMod = Random.Range(5, 20);
                enemyToHeal.eAgility += statMod;
                print(enemyToHeal.eName + " At index " + enemyToHeal.enemyIndexInBattleManager + " had " + statMod + " Added to it's Agailty");
                print("enemy at index " + enemyToHeal.enemyIndexInBattleManager + " Had stats moded " + enemyToHeal.amountModed + " time(s) ");
                uiBTL.EndTurn();
            }

            else
            {
                if (randomStatAtk == 0)
                {
                    AttackLowDef();
                    print("used AttackLowDef()");
                }

                else if (randomStatAtk == 1)
                {
                    AttackHighAtk();
                    print("used AttackHighAtk()");
                }

                else if (randomStat == 2)
                {
                    HealEnemy();
                }

                else
                {
                    AttackHighAgi();
                    print("used AttackHighAgi()");
                }
            }
        }

        else
        {
            if (randomStatAtk == 0)
            {
                AttackLowDef();
                print("used AttackLowDef()");
            }

            else if (randomStatAtk == 1)
            {
                AttackHighAtk();
                print("used AttackHighAtk()");
            }

            else if (randomStat == 2)
            {
                HealEnemy();
            }

            else
            {
                AttackHighAgi();
                print("used AttackHighAgi()");
            }
        }

        if (enemyToHeal.amountModed >= 2)
        {
            enemyToHeal.isStatModed = true;
        }

        enemyToHealRef.Clear();
    }
    void StatNeeded(PlayerStatReference pStatNeeded, List<float> ListNeeded)
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
                    ListNeeded.Add(stat.currentHP);
                }
            }
            //sort the list DUH
            playerStatNeeded.Sort();

            /* statsRefForCheck = Mathf.Min(ListNeeded.ToArray());

             for (int i = 0; i < ListNeeded.Count; i++)
             {
                 if(ListNeeded[i] != statsRefForCheck)
                 {
                     ListNeeded.Remove(playerStatNeeded[i]);
                     print("Removed" + battleManager.players[i].name);
                 }

                 if (ListNeeded[i] == statsRefForCheck)
                 {
                     ListNeeded.Remove(playerStatNeeded.Count - 1);
                 }
             }*/
        }

        else if (statNeeded == PlayerStatReference.Agility)
        {
            foreach (BattleManager.PlayerInformtion stat in battleManager.players)
            {
                //same idea as with hp
                if (stat.playerReference != null && stat.currentHP > 0)
                {
                    ListNeeded.Add(stat.agi);
                }
            }

            playerStatNeeded.Sort();

            /* statsRefForCheck = Mathf.Max(ListNeeded.ToArray());

             for (int i = 0; i < ListNeeded.Count; i++)
             {
                 if (ListNeeded[i] != statsRefForCheck)
                 {
                     ListNeeded.Remove(ListNeeded[i]);
                     print("Removed" + battleManager.players[i].name);
                 }

                 else if (ListNeeded[i] == statsRefForCheck)
                 {
                     ListNeeded.Remove(playerStatNeeded.Count - 1);
                 }
             }*/
        }

        else if (statNeeded == PlayerStatReference.Attack)
        {
            foreach (BattleManager.PlayerInformtion stat in battleManager.players)
            {
                //same idea as with hp
                if (stat.playerReference != null && stat.currentHP > 0)
                {
                    ListNeeded.Add(stat.atk);
                }
            }

            ListNeeded.Sort();

            statsRefForCheck = Mathf.Max(playerStatNeeded.ToArray());

            /*for (int i = 0; i < playerStatNeeded.Count; i++)
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
            }*/
        }

        else
        {
            //you get it by now 
            foreach (BattleManager.PlayerInformtion stat in battleManager.players)
            {
                //same idea as with the other two ... just incase you forgot if you are Dead you didnt make the cut
                if (stat.playerReference != null && stat.currentHP > 0)
                {
                    ListNeeded.Add(stat.def);
                }
            }
            ListNeeded.Sort();

            /* statsRefForCheck = Mathf.Min(playerStatNeeded.ToArray());
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
             }*/
        }
    }
    //function override used to get the stats a enemy same idea as with the player 
    //  void StatNeeded(EnemyStatReference eStatNeeded)
    // {

    // }
    //Calcualte the damage
    public virtual void TakeDamage(float playerAttack, int numberOfAttacks)
    {
        Debug.Log("Received player attack: " + playerAttack);
        float damage = playerAttack - ((eDefence / (20.0f + eDefence)) * playerAttack);
        currentHP -= damage;
        damageText.gameObject.SetActive(true);
        damageText.text = Mathf.RoundToInt(damage).ToString();
        battleManager.enemies[enemyIndexInBattleManager].currentHP = currentHP; //Update the BTL manager with the new health
        HP.fillAmount = currentHP / maxHP;
        animator.SetBool("Hit", true);

        if (currentHP <= 0.0f)
        {
            animator.SetBool("Death", true);
        }
        else
        {
            if (numberOfAttacks <= 0)
            {
                uiBTL.EndTurn(); //Only end the turn after the damage has been taken
            }
        }
    }
    public void EndHitAnimation()
    {
        animator.SetBool("Hit", false);
    }
    public void GiveNamesAndSkills()
    {
        switch (enemyName)
        {
            case EnemyName.Bat:
                eName = "The Bat";
                //Dps
                // canUseSkill = (AllEnemySkills)skills[1];
                skillNeedsCharge = true;
                break;

            case EnemyName.Boar:
                //Dps
                // canUseSkill = (AllEnemySkills)skills[1];
                skillNeedsCharge = true;
                eName = "The Boar";
                break;

            case EnemyName.Dino:
                //Tank
                //canUseSkill = (AllEnemySkills)skills[0];
                eName = "The Dino";
                skillNeedsCharge = true;
                break;

            case EnemyName.Dragon:
                //Tank
                // canUseSkill = (AllEnemySkills)skills[0];
                eName = "The Dragon";
                skillNeedsCharge = true;
                break;

            case EnemyName.Ghost:
                //Support
                //canUseSkill = (AllEnemySkills)skills[0];
                eName = "The Ghost";
                skillNeedsCharge = true;
                break;

            case EnemyName.Giant:
                //Tank
                // canUseSkill = (AllEnemySkills)skills[0];
                skillNeedsCharge = true;
                eName = "The Giant";
                break;

            case EnemyName.Mimic:
                //Tank
                //  canUseSkill = (AllEnemySkills)skills[1];
                eName = "The Mimic";
                skillNeedsCharge = false;
                break;

            case EnemyName.Mushroom:
                //support
                // canUseSkill = (AllEnemySkills)skills[0];
                eName = "The Most Dangerous Mushroom";
                skillNeedsCharge = true;
                break;

            case EnemyName.Octodad:
                //Tank
                // canUseSkill = (AllEnemySkills)skills[0];
                eName = "The Octodad";
                skillNeedsCharge = true;
                break;

            case EnemyName.Reptile:
                //Dps
                // canUseSkill = (AllEnemySkills)skills[0];
                skillNeedsCharge = false;
                eName = "King K rool";
                break;

            case EnemyName.Slime:
                //Support
                //canUseSkill = (AllEnemySkills)skills[0];
                eName = "The Slime";
                skillNeedsCharge = true;
                break;

            case EnemyName.Snake:
                //Dps
                // canUseSkill = (AllEnemySkills)skills[1];
                eName = "The Ekans";
                skillNeedsCharge = true;
                break;

            case EnemyName.Yeti:
                //Tank
                // canUseSkill = (AllEnemySkills)skills[0];
                eName = "The Abominable Snowman";
                skillNeedsCharge = true;
                break;

            case EnemyName.Solider:
                //Dps
                //canUseSkill = (AllEnemySkills)skills[0];
                eName = "The Soilder";
                skillNeedsCharge = true;
                break;

            case EnemyName.Lieutenant:
                //Dps
                //canUseSkill = (AllEnemySkills)skills[0];
                skillNeedsCharge = true;
                eName = "The Lieutentant";
                break;
        }
    }
    public void IncreaseStatsBasedOnLevel(int enemyCurrentLevel)
    {
        enemyCurrentLevel = eCurrentLevel;
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
                eDefence = Mathf.CeilToInt(eDefence + (skillPoints * 0.7f));
                currentHP = Mathf.CeilToInt(currentHP + (skillPoints * 60.0f * 0.5f));
                Debug.Log(eName + " is a " + enemyClass + " Class of enemy");
                break;

            case EnemyClassType.Support:
                eAttack = Mathf.CeilToInt(eAttack + (skillPoints * 0.4f));
                eAgility = Mathf.CeilToInt(eAgility + (skillPoints * 0.6f));
                currentHP = Mathf.CeilToInt(currentHP + (skillPoints * 85.0f * 0.5f));
                Debug.Log(eName + " is a " + enemyClass + " Class of enemy");
                break;
        }

        maxHP = currentHP;
    }
    void AssingClassSkills(Enemy enemy)
    {
        switch (enemyClass)
        {
            case EnemyClassType.DPS:

                enemy.skills = new int[]
                {
                    (int) AllEnemySkills.Slice_And_Dice,
                    (int)AllEnemySkills.Bite,
                    (int)AllEnemySkills.Ball_Roll,
                    (int)AllEnemySkills.Attack_Multiple
                };

                // give wait times to skills 
                if ((int)canUseSkill == skills[0])
                {
                    waitTime = 4;
                }

                else if ((int)canUseSkill == skills[1])
                {
                    waitTime = 3;
                }

                else if ((int)canUseSkill == skills[2])
                {
                    waitTime = 2;

                }

                else if ((int)canUseSkill == skills[3])
                {
                    waitTime = 2;
                }

                break;

            case EnemyClassType.Tank:

                enemy.skills = new int[]
                {
                    (int) AllEnemySkills.Ground_Smash,
                    (int)AllEnemySkills.Blow_Self,
                    (int)AllEnemySkills.Earth_Smash,
                    (int) AllEnemySkills.Raise_Defence
                };

                if ((int)canUseSkill == skills[0]) { waitTime = 3; }
               
                else if ((int)canUseSkill == skills[1]) { }
               
                else if ((int)canUseSkill == skills[2]) { waitTime = 2; }
               
                else if((int)canUseSkill == skills[3]) { waitTime = 3; }

                break;

            case EnemyClassType.Support:

                enemy.skills = new int[]
                {
                    (int) AllEnemySkills.Increase_Multiple_Stats,
                    (int) AllEnemySkills.All_Enemy_Heal,
                    
                };

                if ((int)canUseSkill == skills[0])
                {
                    waitTime = 3;
                }

                else if ((int)canUseSkill == skills[1])
                {
                    waitTime = 4;
                }

                else if ((int)canUseSkill == skills[2])
                {
                     waitTime = 2;
                }

                break;
        }
    }
    //assigns skills functionalilty for skills that effect more than one in the current battle
    //called in enemyTurn
    void MakeSkillsWork(AllEnemySkills ChosenSkill)
    {
        float attackMod;
        int healthMod;
        int playerIndexRef;
        int randomRow = Random.Range(0, 1);

        switch (ChosenSkill)
        {
           
            case AllEnemySkills.Ground_Smash:

                animator.SetBool("isWaiting", false);
                animator.SetBool("SkillInUse", true);
                break;

            //Skill that hits a player random amount of time 
            #region slice and dice
            case AllEnemySkills.Slice_And_Dice:

                int hitAmount = Random.Range(0, 5);

                playerIndexRef = Random.Range(0, battleManager.players.Length - 1);

                attackThisPlayer = battleManager.players[playerIndexRef].playerReference;

                if (attackThisPlayer.currentHP <= 0)
                {
                    MakeSkillsWork((AllEnemySkills)skills[0]);
                }

                else
                {
                    if (hitAmount == 0)
                    {
                        print("Attack Missed");
                        uiBTL.EndTurn();
                    }

                    else if (hitAmount == 1)
                    {

                        attackThisPlayer.TakeDamage(eAttack);

                        uiBTL.EndTurn();
                    }

                    else if (hitAmount == 2)
                    {
                        print("I sliced and diced " + hitAmount + " time(s)");
                        for (int i = 0; i < 2; ++i)
                        {
                            if (i == 0)
                            {
                                attackThisPlayer.TakeDamage(eAttack);
                                print("First Attack does " + eAttack + " Damage");
                            }

                            else if (i == 1)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .1f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Second Attack does " + attackMod + " Damage extra");
                            }
                        }

                        uiBTL.EndTurn();
                    }

                    else if (hitAmount == 3)
                    {
                        print("I sliced and diced " + hitAmount + " time(s)");
                        for (int i = 0; i < 3; ++i)
                        {
                            if (i == 0)
                            {
                                attackThisPlayer.TakeDamage(eAttack);
                                print("First Attack does " + eAttack + " Damage");
                            }

                            else if (i == 1)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .1f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Second Attack does " + attackMod + " Damage extra");
                            }

                            else if (i == 2)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .2f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Third Attack does " + attackMod + " Damage extra");
                            }
                        }

                        uiBTL.EndTurn();
                    }

                    else if (hitAmount == 4)
                    {
                        print("I sliced and diced " + hitAmount + " time(s)");

                        for (int i = 0; i < 4; ++i)
                        {
                            if (i == 0)
                            {
                                attackThisPlayer.TakeDamage(eAttack);
                                print("First Attack does " + eAttack + " Damage");
                            }

                            else if (i == 1)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .1f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Second Attack does " + attackMod + " Damage extra");
                            }

                            else if (i == 2)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .2f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Third Attack does " + attackMod + " Damage extra");
                            }

                            else if (i == 3)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .3f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Fourth Attack does " + attackMod + " Damage extra");
                            }

                        }

                        uiBTL.EndTurn();
                    }

                    else
                    {
                        print("I sliced and diced " + hitAmount + " time(s)");
                        for (int i = 0; i < 5; ++i)
                        {
                            if (i == 0)
                            {
                                attackThisPlayer.TakeDamage(eAttack);
                                print("First Attack does " + eAttack + "Damage");
                            }

                            else if (i == 1)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .1f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Second Attack does " + attackMod + " Damage extra");
                            }

                            else if (i == 2)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .2f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Third Attack does " + attackMod + " Damage extra");
                            }

                            else if (i == 3)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .3f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Fourth Attack does " + attackMod + " Damage extra");
                            }

                            else if (i == 4)
                            {
                                Mathf.CeilToInt(attackMod = (eAttack * .4f));
                                attackThisPlayer.TakeDamage(eAttack + attackMod);
                                print("Fifth Attack does " + attackMod + " Damage extra");
                            }
                        }

                        uiBTL.EndTurn();
                    }
                }
                #endregion
                break;

            #region increase multiple stats
            //increase a stat based off class
            case AllEnemySkills.Increase_Multiple_Stats:
                int statIncrease = PickRandomNumber(10, 20);
                for (int i = 0; i < battleManager.enemies.Length; ++i)
                {
                    if (this == battleManager.enemies[i].enemyReference)
                    {
                        theHealer = battleManager.enemies[i].enemyReference;
                    }

                    if (this != battleManager.enemies[i].enemyReference && battleManager.enemies[i].enemyReference != null)
                    {
                        enemyToHeal = battleManager.enemies[i].enemyReference;
                    }
                }

                if (enemyToHeal.dead)
                {
                    MakeSkillsWork(AllEnemySkills.Increase_Multiple_Stats);
                }

                if (!enemyToHeal.isStatModed)
                {
                    if (enemyToHeal.enemyClass == EnemyClassType.DPS)
                    {
                        enemyToHeal.eAttack += statIncrease * 0.6f;
                        enemyToHeal.eAgility += statIncrease * 0.4f;
                        print("Enemy At Index " + enemyToHeal.enemyIndexInBattleManager + " Has had Attack modified by " + (statIncrease * 0.6f) + " Also has had Agility modfied by " + (statIncrease * 0.4f));
                        enemyToHeal.isStatModed = true;
                    }

                    else if (enemyToHeal.enemyClass == EnemyClassType.Tank)
                    {
                        enemyToHeal.eAttack += statIncrease * 0.3f;
                        enemyToHeal.eDefence += statIncrease * 0.7f;
                        print("Enemy At Index " + enemyToHeal.enemyIndexInBattleManager + " Has had Attack modified by " + (statIncrease * 0.3f) + " Also has had Defence modfied by " + (statIncrease * 0.7f));
                        enemyToHeal.isStatModed = true;
                    }

                    else if (enemyToHeal.enemyClass == EnemyClassType.Support)
                    {
                        enemyToHeal.eAttack += statIncrease * 0.4f;
                        enemyToHeal.eAgility += statIncrease * 0.6f;
                        print("Enemy At Index " + enemyToHeal.enemyIndexInBattleManager + " Has had Attack modified by " + (statIncrease * 0.4f) + " Also has had Agility modfied by " + (statIncrease * 0.6f));
                        enemyToHeal.isStatModed = true;
                    }
                }

                else
                {
                    HealEnemy();
                }

                uiBTL.EndTurn();

                break;
            #endregion

            #region heal all enemies
            case AllEnemySkills.All_Enemy_Heal:
                healthMod = 100;
                print("Used The Heal All Skill");

                for (int i = 0; i < battleManager.enemies.Length; ++i)
                {
                    if (battleManager.enemies[i].enemyReference != null && this != battleManager.enemies[i].enemyReference)
                    {
                        if (!battleManager.enemies[i].enemyReference.dead)
                        {
                            enemyToHeal = battleManager.enemies[i].enemyReference;
                            print("Enemy was healed at index " + enemyToHeal.enemyIndexInBattleManager);
                            enemyToHeal.currentHP += healthMod;
                            print("Enemies new Hp is " + enemyToHeal.currentHP);

                            if (enemyToHeal.currentHP > enemyToHeal.maxHP)
                            {
                                enemyToHeal.currentHP = enemyToHeal.maxHP;
                                print("Enemies new Hp is " + enemyToHeal.currentHP);
                            }
                        }
                    }
                }

                uiBTL.EndTurn();

                break;
            #endregion

            
            case AllEnemySkills.Bite:

                animator.SetBool("isWaiting", false);
                animator.SetBool("SkillInUse", true);
                break;
            

            #region earth smash
            case AllEnemySkills.Earth_Smash:

                for (int i = 0; i < battleManager.players.Length; ++i)
                {
                    attackThisPlayer = battleManager.players[i].playerReference;
                    attackThisPlayer.TakeDamage(eAttack);
                }

                uiBTL.EndTurn();

                break;
            #endregion

            #region blow self
            case AllEnemySkills.Blow_Self:
                print("Enemy Blew slef skillfully ");
                currentHP = 0;
                blowStrength = 200;
                AttackWholeField(blowStrength);
                blowStrength = 0;
                break;
            #endregion

            #region ball roll
            case AllEnemySkills.Ball_Roll:

                if (randomRow == 0)
                {
                    AttackOberon();
                    Invoke("AttackFrea", 1);
                    Invoke("AttackArcelus", 3);
                    Invoke("AttackFargas", 4);

                }

                else if (randomRow == 1)
                {
                    AttackFargas();
                    Invoke("AttackFargas", 1);
                    Invoke("AttackFrea", 3);
                    Invoke("AttackArcelus", 4);

                }

                Invoke("EndTurn", 5.5f);
                break;
            #endregion

            #region attack multiple
            case AllEnemySkills.Attack_Multiple:

                int firstSelected;

                PickPlayer(PickRandomNumber(battleManager.players[0].playerReference.playerIndex, battleManager.players[1].playerReference.playerIndex, battleManager.players[2].playerReference.playerIndex, battleManager.players[3].playerReference.playerIndex));

                if (attackThisPlayer.currentHP <= 0)
                {
                    MakeSkillsWork(AllEnemySkills.Attack_Multiple);
                }

                firstSelected = attackThisPlayer.playerIndex;

                AttackPlayer(attackThisPlayer.playerIndex, (int)eAttack);

                PickPlayer(PickRandomNumber(battleManager.players[0].playerReference.playerIndex, battleManager.players[1].playerReference.playerIndex, battleManager.players[2].playerReference.playerIndex, battleManager.players[3].playerReference.playerIndex));

                if (attackThisPlayer.playerIndex == firstSelected)
                {
                    PickPlayer(PickRandomNumber(battleManager.players[0].playerReference.playerIndex, battleManager.players[1].playerReference.playerIndex, battleManager.players[2].playerReference.playerIndex, battleManager.players[3].playerReference.playerIndex));
                }


                Invoke("AttackPlayer", 1);

                Invoke("EndTurn", 1.2f);

                break;
            #endregion

            case AllEnemySkills.Raise_Defence:
                RaiseDefSkill();
                break;
        }
    }
    void ChoosePlayer()
    {
        if (enemyAttack == EnemyAttackType.Relentless)
        {
            playerIndexHolder = battleManager.players[PickRandomNumber(battleManager.players[0].playerReference.playerIndex, battleManager.players[1].playerReference.playerIndex, battleManager.players[2].playerReference.playerIndex, battleManager.players[3].playerReference.playerIndex)].playerReference.playerIndex;
            print(playerIndexHolder);
        }
    } //choose a player for Relentless Enemy 
    // pick a random index
    int PickRandomNumber(int value_0, int value_1)
    {
        int pickedValue;
        int randomizeValues = Random.Range(0, 1);
        if (randomizeValues == 0)
        {
            pickedValue = value_0;
            return pickedValue;
        }
        else
        {
            pickedValue = value_1;
            return pickedValue;
        }
    }
    int PickRandomNumber(int value_0, int value_1, int value_2)
    {
        int pickedValue;
        int randomizeValues = Random.Range(0, 2);
        if (randomizeValues == 0)
        {
            pickedValue = value_0;
            return pickedValue;
        }

        else if (randomizeValues == 1)
        {
            pickedValue = value_1;
            return pickedValue;
        }

        else
        {
            pickedValue = value_2;
            return pickedValue;
        }
    }
    int PickRandomNumber(int value_0, int value_1, int value_2, int value_3)
    {
        int pickedValue;
        int randomizeValues = Random.Range(0, 3);

        if (randomizeValues == 0)
        {
            pickedValue = value_0;
            return pickedValue;
        }

        else if (randomizeValues == 1)
        {
            pickedValue = value_1;
            return pickedValue;
        }

        else if (randomizeValues == 2)
        {
            pickedValue = value_2;
            return pickedValue;
        }

        else
        {
            pickedValue = value_3;
            return pickedValue;
        }
    }
    int PickRandomNumber(int value_0, int value_1, int value_2, int value_3, int value_4)
    {
        int pickedValue;
        int randomizeValues = Random.Range(0, 3);

        if (randomizeValues == 0)
        {
            pickedValue = value_0;
            return pickedValue;
        }

        else if (randomizeValues == 1)
        {
            pickedValue = value_1;
            return pickedValue;
        }

        else if (randomizeValues == 2)
        {
            pickedValue = value_2;
            return pickedValue;
        }

        else if (randomizeValues == 3)
        {
            pickedValue = value_3;
            return pickedValue;
        }

        else
        {
            pickedValue = value_4;
            return pickedValue;
        }
    }
    void AttackWholeField(int attack)
    {
        for (int i = 0; i < battleManager.players.Length; ++i)
        {
            if (battleManager.players[i].playerReference != null && !battleManager.players[i].playerReference.dead)
            {
                battleManager.players[i].playerReference.TakeDamage(attack);
            }
        }

        for (int i = 0; i < battleManager.enemies.Length; ++i)
        {
            if (battleManager.enemies[i].enemyReference != null && !battleManager.enemies[i].enemyReference.dead)
            {
                battleManager.enemies[i].enemyReference.TakeDamage(attack, 1);
            }
        }
    }
    void AttackFargas(float damage)
    {
        attackThisPlayer = battleManager.players[0].playerReference;
        attackThisPlayer.TakeDamage(Mathf.CeilToInt(damage));
    }
    void AttackOberon(float damage)
    {
        attackThisPlayer = battleManager.players[1].playerReference;
        attackThisPlayer.TakeDamage(Mathf.CeilToInt(damage));
    }
    void AttackFrea(float damage)
    {
        attackThisPlayer = battleManager.players[2].playerReference;
        attackThisPlayer.TakeDamage(Mathf.CeilToInt(damage));
    }
    void AttackArcelus(float damage)
    {
        attackThisPlayer = battleManager.players[3].playerReference;
        attackThisPlayer.TakeDamage(Mathf.CeilToInt(damage));
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Get rid of these functions after set up Ball Roll Animation
    //these ones below
    void AttackFargas()
    {
        attackThisPlayer = battleManager.players[0].playerReference;
        attackThisPlayer.TakeDamage(eAttack);
    }
    void AttackOberon()
    {
        attackThisPlayer = battleManager.players[1].playerReference;
        attackThisPlayer.TakeDamage(eAttack);
    }
    void AttackFrea()
    {
        attackThisPlayer = battleManager.players[2].playerReference;
        attackThisPlayer.TakeDamage(eAttack);
    }
    void AttackArcelus()
    {
        attackThisPlayer = battleManager.players[3].playerReference;
        attackThisPlayer.TakeDamage(eAttack);
    }
    //these ones above
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    void PickPlayer(int playerindex)
    {
        attackThisPlayer = battleManager.players[playerindex].playerReference;
    }
    void AttackPlayer(int playerindex, int damage)
    {
        attackThisPlayer = battleManager.players[playerindex].playerReference;
        attackThisPlayer.TakeDamage(damage);
    }
    void PickEnemy(int enemyIndex)
    {
        enemyToHeal = battleManager.enemies[enemyIndex].enemyReference;
    }
    void EndTurn()
    {
        uiBTL.EndTurn();
    }
    void EndSkill()
    {
        currentState = EnemyState.idle;
        waitQTurns = waitTimeAtStart;
        waitTime = waitTimeAtStart;
        animator.SetBool("SkillInUse", false);
        EndTurn();
    }
    void GroundSmashSkill()
    {
        int randomRow = Random.Range(0, 1);
        

        if (randomRow == 0)
        {
            print("Picked Back Row");

            AttackOberon(eAttack * 1.5f);
            print("Attacked" + attackThisPlayer.name);


            AttackFrea(eAttack * 1.5f);
            print("Then Attacked " + attackThisPlayer.name);
            attackThisPlayer.TakeDamage(eAttack * 1.5f);

            attackThisPlayer = battleManager.players[1].playerReference;
            print("Attacked" + attackThisPlayer.nameOfCharacter);
            attackThisPlayer.TakeDamage(eAttack);

            attackThisPlayer = battleManager.players[2].playerReference;
            print("Then Attacked " + attackThisPlayer.nameOfCharacter);
            attackThisPlayer.TakeDamage(eAttack);

        }

        else if (randomRow == 1)
        {
            print("Picked Back Row");

            AttackFargas(eAttack * 1.5f);
            print("Attacked" + attackThisPlayer.name);

            AttackArcelus(eAttack * 1.5f);
            print("Then Attacked " + attackThisPlayer.name);
            attackThisPlayer.TakeDamage(eAttack * 1.5f);

            attackThisPlayer = battleManager.players[0].playerReference;
            print("Attacked" + attackThisPlayer.nameOfCharacter);
            attackThisPlayer.TakeDamage(eAttack);

            attackThisPlayer = battleManager.players[3].playerReference;
            print("Then Attacked " + attackThisPlayer.nameOfCharacter);
            attackThisPlayer.TakeDamage(eAttack);
        }
    }
    void BiteSkill()
    {
        float attackMod;
        attackThisPlayer = battleManager.players[PickRandomNumber(battleManager.players[0].playerReference.playerIndex, battleManager.players[1].playerReference.playerIndex, battleManager.players[2].playerReference.playerIndex, battleManager.players[3].playerReference.playerIndex)].playerReference;
        if (attackThisPlayer.currentHP <= 0)
        {
            BiteSkill();
        }

        print("Enemy Attack is " + eAttack);
        print(attackThisPlayer.nameOfCharacter + " Was Attacked With the Bite Skill");
        Mathf.CeilToInt(attackMod = (eAttack * .5f));
        eAttack += attackMod;
        print("Enemy New Attack is " + eAttack);
        attackThisPlayer.TakeDamage(eAttack);
        eAttack = enemyStartingAtk;
    }
    void RaiseDefSkill()
    {
        if (waitTime == waitTimeAtStart)
        {
            defenceMod = Mathf.CeilToInt(Random.Range((eDefence * .2f), (eDefence * .5f)));
            eDefence += defenceMod;

            print("A Defence mod of " + defenceMod + " Will be added to the " + eName + " At Index " + enemyIndexInBattleManager + " For " + waitTime + " Turns");
        }

        if(waitTime <= 0)
        {
            animator.SetBool("SkillInUse", false);
            eDefence = enemyStartingDefence;
            currentState = EnemyState.idle;
            waitTime = waitTimeAtStart;
        }
      
    }

    protected void Death()
    {
        if (!dead)
        {
            currentState = EnemyState.idle;
            spriteRenderer.enabled = false;
            enemyCanvas.SetActive(false);
            dead = true;
            gameObject.SetActive(false);
            uiBTL.EnemyIsDead(enemyIndexInBattleManager);

            uiBTL.EndTurn(); //Only end the turn after the enemy is dead
        }
    }
        //An enemy tied to a player should get healed when the player is healed. Called from the tied player 
    public virtual void HealDueToTied(float healAmount)
    { 
        currentHP += healAmount; 
        HP.fillAmount = currentHP / maxHP; 
        healthObject.gameObject.SetActive(true); 
        tiedTimer--; //Tied timer decreases when damaged or healed 
        if (tiedTimer <= 0) 
        { 
            tiedTimer = 0; 
            tieThisPlayer.Untie(); 
            chain.gameObject.SetActive(false); 
            tieThisPlayer = null; 
        } 
    }

    public virtual void Heal(float healAmount)
    {
        currentHP += healAmount;
        HP.fillAmount = currentHP / maxHP;
        healthObject.gameObject.SetActive(true);
    }


}

