using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Enemy : MonoBehaviour
{
    public int enemyIndexInBattleManager;
    protected BattleManager battleManager;
    protected AudioManager audioManager;
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
    int timeAttacking;
    public int timeAttackingForSlice;
    [SerializeField] int countDownToBlow;
    public int blowStrength;
    int enemyStartingAtk;
    int enemyStartingDefence;
    int[] skills;
    [SerializeField] int hitAmount;
    private int defenceMod;
    public List<float> playerStatNeeded;
    public List<float> enemyStatNeeded;
    List<Enemy> enemyToHealRef = new List<Enemy>(); //list of all the enemies other then the instance calling this function
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
    public Text missText;
    public GameObject enemyCanvas;

    protected bool haveAddedMyself;
    public bool dead;
    protected bool skillPointAdded;
    public bool hit;
    public bool blow;
    private bool playedWaitingText;
    public bool isStatModed;
    public bool test;
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

    //Tied
    protected Player tieThisPlayer; 
    protected int tiedTimer = 4; //Used to nullify the tied player reference whenever the Farea takes damage or gets healed 
    public GameObject healthObject; 
    public GameObject chain; //Symbol used for tied 
    public GameObject lightingObject;
    public GameObject deadlyTiesObject;

    //Blowself
    public GameObject blowSelfObject;

    //Buffs/Debuffs

    protected bool defenseBuffed = false;
    protected bool attackBuffed = false;
    protected bool agilityBuffed = false;
    protected bool strBuffed = false;
    protected float defenseBuffSkillQCounter = 0; //How many turns until the defense buff is reversed. Need three counters as multiple stats could be buffed/debuffed at the same time
    protected float attackBuffSkillQCounter = 0;
    protected float agilityBuffSkillQCounter = 0;
    protected float strBuffSkillQCounter = 0;
    public GameObject defBuffEffect;
    public GameObject atkBuffEffect;
    public GameObject agiBuffEffect;
    public GameObject strBuffEffect;
    public SpriteRenderer debuffArrow;
    public GameObject atkBuffArrowIndicator;
    public GameObject defBuffArrowIndicator;
    public GameObject agiBuffArrowIndicator;
    public GameObject strBuffArrowIndicator;
    protected Quaternion arrowRotator;
    protected Color debuffColor;
    //Actual stats --> Stats after they've been modified in battle
    protected float atkBeforeBUff; //Since the code is usin modAttack for certain skills, I will need to buff eAttack directly and use this to return it
    protected float actualDEF;
    protected float actualAgi;
    protected float actualCRIT;
    protected float actualSTR;
    protected float actualDamage; //Made zero when it's a miss

    //Status ailments
    //An enemy can be affected by two ailments at max
    public EnemyStatusAilment currentStatusAilment0 = EnemyStatusAilment.none;
    public EnemyStatusAilment currentStatusAilment1 = EnemyStatusAilment.none;

    //+Chained
    protected int chainedWaitTime = 0; //Keep track of when chained is reverted
    protected Enemy[] chainedEnemy; //Stores the information for the chained enemies
    public GameObject chainedSymbol;
    public GameObject primaryChainedSymbol; //Used to distinguish the primary target of a chain
    protected bool primaryChainedTarget = false; // The primary target is the only one to unchain the other targets

    //+Rallied
    protected int ralliedWaitTime = 0;
    protected float ralliedDamageModifier = 1.0f; //When rallied, this modifier increases damage taken
    public GameObject ralliedSymbol;

    //+Burn
    protected int burnedWaitTime = 0;
    protected float burnDamage = 20.0f; //Damages the enemy at the start of every turn
    public GameObject burnSymbol;

    protected void Awake()
    {
        //IncreaseStatsBasedOnLevel(eCurrentLevel);
        AssingClassSkills(this);
        GiveNamesAndSkills();
    }

    protected virtual void Start()
    {
      

        amountModed = 0;
        timeAttacking = 1;
        timeAttackingForSlice = 1;
        waitTimeAtStart = waitTime;
        countDownToBlow = Random.Range(3, 5);

        if (enemyName == EnemyName.Mimic)
        {
            waitTurnsText.gameObject.SetActive(true);
            waitTurnsText.text = countDownToBlow.ToString();
            blowSelfObject.SetActive(false);
        }

        else
        {
            blowSelfObject = null;
        }


        battleManager = BattleManager.instance;
        objPooler = ObjectPooler.instance;
        uiBTL = UIBTL.instance;
        audioManager = AudioManager.instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
        animator = gameObject.GetComponent<Animator>();

        damageText.gameObject.SetActive(false);
        healText.gameObject.SetActive(false);
        if(missText != null)
        {
            missText.gameObject.SetActive(false);
        }


        haveAddedMyself = false;
        hit = false;
        dead = false;
        playedWaitingText = false;
        isStatModed = false;
        blow = false;


        ChoosePlayer();
        enemyStartingAtk = Mathf.CeilToInt(eAttack);
        enemyStartingDefence = Mathf.CeilToInt(eDefence);

        currentState = EnemyState.idle;

        chainedEnemy = new Enemy[2]; //2 enemies can be chained

        atkBeforeBUff = eAttack;
        actualDEF = eDefence;
        actualAgi = eAgility;
        actualSTR = eStrength;


        if(chainedSymbol)
        {
            chainedSymbol.gameObject.SetActive(false);
        }

        if(primaryChainedSymbol)
        {
            primaryChainedSymbol.gameObject.SetActive(false);
        }

        if(ralliedSymbol)
        {
            ralliedSymbol.gameObject.SetActive(false);
        }

        if(burnSymbol)
        {
            burnSymbol.gameObject.SetActive(false);
        }

        if(debuffArrow)
        {
            debuffArrow.gameObject.SetActive(false);
        }

        if(atkBuffArrowIndicator)
        {
            atkBuffArrowIndicator.gameObject.SetActive(false);
        }

        if (strBuffArrowIndicator)
        {
            strBuffArrowIndicator.gameObject.SetActive(false);
        }

        if (defBuffArrowIndicator)
        {
            defBuffArrowIndicator.gameObject.SetActive(false);
        }

        if (agiBuffArrowIndicator)
        {
            agiBuffArrowIndicator.gameObject.SetActive(false);
        }

        if(healthObject)
        {
            healthObject.gameObject.SetActive(false);
        }

        if(waitTurnsText && enemyName != EnemyName.Mimic)
        {
            waitTurnsText.gameObject.SetActive(false);
        }

        if(atkBuffEffect)
        {
            atkBuffEffect.gameObject.SetActive(false);
        }

        if (defBuffEffect)
        {
            defBuffEffect.gameObject.SetActive(false);
        }

        if (agiBuffEffect)
        {
            agiBuffEffect.gameObject.SetActive(false);
        }

        if (strBuffEffect)
        {
            strBuffEffect.gameObject.SetActive(false);
        }

        debuffColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    protected virtual void Update()
    {
        //If the enemy is yet to add itself to the Q and the btl manager is currently adding enemies, then add this enemy to the Q
        if (!haveAddedMyself && battleManager.addEnemies)
        {
            AddEnemyToBattle();
            haveAddedMyself = true;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            EndTurn();
        }

    }

    //Every enemy scales differently based on its warrior class (DPS,Tanks, Support)
    // also assigns skills that only certain types can use 
    public virtual void AddEnemyToBattle()
    {
        battleManager.AddEnemy(enemyIndexInBattleManager, Mathf.RoundToInt(eAgility), Mathf.RoundToInt(eStrength), Mathf.RoundToInt(eCritical), Mathf.RoundToInt(eSpeed), Mathf.RoundToInt(currentHP), Mathf.RoundToInt(maxHP), this, name);
    }

    public virtual void EnemyTurn()
    {
        if (!battleManager.battleHasEnded) //If all the players are dead, don't run the turn. Precautionary if statement.
        {
            if (currentState == EnemyState.waiting)
            {
                CheckForAilments();
                CheckForBuffs();
                waitTime--;
                waitQTurns--;
                waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                if (waitQTurns <= 0)
                {
                    waitTurnsText.gameObject.SetActive(false); //Turn off the text. Don't forget to enable it when the enemy goes to waiting state
                    MakeSkillsWork(canUseSkill);
                    
                    //Execute skill here 
                }

                else
                {
                    //End the turn
                    uiBTL.EndTurn();
                }

            }

            // used for skills that dont need to wait to activate instead happen right away and last for multiple turns 
            else if (currentState == EnemyState.skilling)
            {
                CheckForAilments();
                CheckForBuffs();
                waitQTurns--;
                waitTime--;
                waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                if (waitQTurns <= 0) 
                {
                    waitTurnsText.gameObject.SetActive(false);
                    MakeSkillsWork(canUseSkill); 
                }
                EndTurn();
            }

            else
            {
                float attackChance = Random.Range(0, 100); // determines if the ememy will use its type attack or a dumb attack 
                float skillChance = Random.Range(0, 50);// determines if the enemy will use a skill or not //TEMP VALUES//

                if (canUseSkill == AllEnemySkills.No_Skill)
                {
                    skillChance = -1.0f;
                }

                if (!dead)
                {
                    uiBTL.backdropHighlighter.gameObject.SetActive(true);
                    uiBTL.DisableActivtyText();
                    CheckForAilments();
                    CheckForBuffs();

                    switch (enemyAttack)
                    {
                        #region Dumb
                        case EnemyAttackType.Dumb:

                            if (skillChance >= 47)
                            {
                                if (skillNeedsCharge)
                                {
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                    waitTurnsText.gameObject.SetActive(true);
                                    playedWaitingText = true;
                                    animator.SetBool("isWaiting", true);
                                    EndTurn();
                                }

                                else
                                { 
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                    waitTurnsText.gameObject.SetActive(true);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                uiBTL.UpdateActivityText("Attack");
                                DumbAttack();
                            }
                            break;
                        #endregion

                        #region Opportunistic
                        case EnemyAttackType.Opportunistic:

                            if (attackChance > 30)
                            {
                                if (skillChance >= 47)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        animator.SetBool("isWaiting", true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                       
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    AttackHighAgi();
                                }
                            }

                            else
                            {
                                if (skillChance >= 50)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        animator.SetBool("isWaiting", true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                     
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    DumbAttack();
                                }
                            }
                            break;
                        #endregion

                        #region Assassin
                        case EnemyAttackType.Assassin:

                            if (attackChance > 30)
                            {
                                if (skillChance >= 30)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        animator.SetBool("isWaiting", true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                  
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    AttackLowHp();
                                }
                            }


                            else
                            {
                                if (skillChance >= 49)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        animator.SetBool("isWaiting", true); 
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                  
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    DumbAttack();
                                }
                            }
                            break;
                        #endregion

                        #region Bruiser
                        case EnemyAttackType.Bruiser:

                            if (attackChance > 30)
                            {
                                if (skillChance >= 45)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        waitTurnsText.gameObject.SetActive(true);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        animator.SetBool("isWaiting", true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    AttackHighAtk();
                                }
                            }

                            else
                            {
                                if (skillChance >= 50)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        waitTurnsText.gameObject.SetActive(true);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        animator.SetBool("isWaiting", true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                    
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    DumbAttack();
                                }
                            }
                            break;
                        #endregion

                        #region Healer
                        case EnemyAttackType.Healer:

                            float chanceOfHealth = Random.Range(0.2f, 0.9f); //  how low should  the health be before it is healed //CHANGE THIS FUCKING VARIABLE NAME ANDRE!!
                            if (skillChance >= 49)
                            {
                                if (skillNeedsCharge)
                                {
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    waitTurnsText.gameObject.SetActive(true);
                                    waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                    animator.SetBool("isWaiting", true);
                                    playedWaitingText = true;
                                    EndTurn();
                                }

                                else
                                {
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                    waitTurnsText.gameObject.SetActive(true);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                EnemyToHeal();

                                if (enemyToHeal == null)
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    DumbAttack();
                                }

                                else
                                {

                                    if (enemyToHeal.currentHP <= (enemyToHeal.maxHP * chanceOfHealth))
                                    {
                                        animator.SetBool("Heal", true);
                                    }

                                    else
                                    {
                                        uiBTL.UpdateActivityText("Attack");
                                        DumbAttack();
                                    }
                                }


                            }

                            break;
                        #endregion

                        #region Heal Support
                        case EnemyAttackType.Heal_Support:

                            if (skillChance >= 49)
                            {
                                if (skillNeedsCharge)
                                {
                                    currentState = EnemyState.waiting;
                                    waitQTurns = waitTime;
                                    waitTurnsText.gameObject.SetActive(true);
                                    waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                    animator.SetBool("isWaiting", true);
                                    playedWaitingText = true;
                                    EndTurn();
                                }

                                else
                                {
                                    currentState = EnemyState.skilling;
                                    waitQTurns = waitTime;
                                    
                                    animator.SetBool("SkillInUse", true);
                                    MakeSkillsWork(canUseSkill);
                                    waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                    waitTurnsText.gameObject.SetActive(true);
                                    EndTurn();
                                }
                            }

                            else
                            {
                                EnemyToMod();
                                int randomStat = Random.Range(0, 2); //pick a random stat to add to 
                                float toBuffrNoBuff = Random.value;

                                if(enemyToHeal == null || enemyToHeal.dead)
                                {
                                    if (randomStat == 0)
                                    {
                                        uiBTL.UpdateActivityText("Attack");
                                        AttackHighAtk();
                                    }

                                    else if (randomStat == 1)
                                    {
                                        uiBTL.UpdateActivityText("Attack");
                                        AttackLowDef();
                                    }

                                    else if (randomStat == 2)
                                    {
                                        uiBTL.UpdateActivityText("Attack");
                                        AttackLowHp();
                                    }
                                }

                                if (toBuffrNoBuff <=.3f)
                                {
                                    if (randomStat == 0)
                                    {
                                        uiBTL.UpdateActivityText("Attack");
                                        AttackHighAtk();
                                    }

                                    else if (randomStat == 1 )
                                    {
                                        uiBTL.UpdateActivityText("Attack");
                                        AttackLowDef();
                                    }

                                    else if (randomStat == 2)
                                    {
                                        uiBTL.UpdateActivityText("Attack");
                                        AttackLowHp();
                                    }
                                }

                                else 
                                {
                                    if (randomStat == 0)
                                    {
                                        animator.SetBool("ModAtk", true);
                                    }

                                   else if (randomStat == 1)
                                   {
                                        animator.SetBool("ModDef", true);
                                   }

                                   
                                    else if (randomStat == 2)
                                    {
                                        animator.SetBool("ModAgi", true);

                                    }

                                }
                            }
                            break;
                        #endregion

                        #region Strategist
                        case EnemyAttackType.Strategist:

                            if (attackChance > 30)
                            {
                                if (attackChance >= 45)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);                                   
                                        animator.SetBool("isWaiting", true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                       
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    AttackHighAgi();
                                }
                            }

                            else
                            {
                                if (skillChance >= 50)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        animator.SetBool("isWaiting", true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                        
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    DumbAttack();
                                }
                            }
                            break;

                        case EnemyAttackType.Demo:

                            DumbAttack();
                            break;
                        #endregion

                        #region Relentless Attack
                        case EnemyAttackType.Relentless:

                            if (attackChance > 30)
                            {
                                if (skillChance >= 45)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        
                                        animator.SetBool("isWaiting", true);
                                        hitAmount = Random.Range(1, 5);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                       
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    RelentlessAttack(playerIndexHolder, timeAttacking);
                                }
                            }

                            else
                            {
                                if (skillChance >= 50)
                                {
                                    if (skillNeedsCharge)
                                    {
                                        currentState = EnemyState.waiting;
                                        waitQTurns = waitTime;
                                        
                                        animator.SetBool("isWaiting", true);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        playedWaitingText = true;
                                        EndTurn();
                                    }

                                    else
                                    {
                                        currentState = EnemyState.skilling;
                                        waitQTurns = waitTime;
                                        
                                        animator.SetBool("SkillInUse", true);
                                        MakeSkillsWork(canUseSkill);
                                        waitTurnsText.text = waitQTurns.ToString(); //Update the UI
                                        waitTurnsText.gameObject.SetActive(true);
                                        EndTurn();
                                    }
                                }

                                else
                                {
                                    uiBTL.UpdateActivityText("Attack");
                                    DumbAttack();
                                }
                            }
                            break;
                        #endregion

                        #region Blow Self
                        case EnemyAttackType.Enemy_Blows_Self:

                            if (skillChance >= 49)
                            {
                                MakeSkillsWork(canUseSkill);
                            }

                            else
                            {
                                countDownToBlow--;
                                BlowSelfCountDown();
                                waitTurnsText.text = countDownToBlow.ToString();
                            }
                            break;
                            #endregion
                    }
                }
                else
                {
                    uiBTL.EndTurn();
                }
            }
        }
    }

    //Calculate whether the attack is a hit or a miss
    protected virtual void CalculateHit()
    {
        //20 sided die + str <? enemy agility
        if (Random.Range(0.0f, 20.0f) + eStrength < attackThisPlayer.actualAgi)
        {
            hit = false;
        }

        else
        {
            hit = true;
            timeAttacking += 1;
        }

       
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

        if (hit)
        {
            uiBTL.UpdateActivityText("Attack");
            objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);

            if (CalculateCrit() <= eCritical)
            {
               
                attackThisPlayer.TakeDamage(eAttack * 1.2f);
                audioManager.PlayThisEffect("attack");
            }

            else
            {
                attackThisPlayer.TakeDamage(eAttack);
                audioManager.PlayThisEffect("attack");
            }
        }
        else
        {
            uiBTL.UpdateActivityText("Attack Missed");
            
            attackThisPlayer.TakeDamage(0.0f); //Pass in zero, i.e miss
        }
        animator.SetBool("Attack", false);
        uiBTL.EndTurn();
    }

    protected void DumbAttack()
    {
        uiBTL.UpdateActivityText("Attack");
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
        StatNeeded(statNeeded);

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
        StatNeeded(statNeeded);
        for (int i = 0; i < 4; i++)
        {
            if (battleManager.players[i].def == Mathf.Min(playerStatNeeded.ToArray()))
            {
                attackThisPlayer = battleManager.players[i].playerReference;
                
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
        StatNeeded(statNeeded);
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
        StatNeeded(statNeeded);
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

        if (attackThisPlayer.dead)
        {
            ChoosePlayer();
            RelentlessAttack(playerIndexHolder, timeAttacking);
            timeAttacking = 1;
        }

        if (timeAttacking == 1)
        {
            
            CalculateHit();
            animator.SetBool("Attack", true);
            ++timeAttacking;
        }

        else if (timeAttacking == 2)
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.1f);
            eAttack += attackMod;
            CalculateHit();
            animator.SetBool("Attack", true);
            ++timeAttacking;
        }

        else if (timeAttacking == 3)
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.2f);
            eAttack += attackMod;
           
            CalculateHit();
            animator.SetBool("Attack", true);
            ++timeAttacking;
        }

        else if (timeAttacking == 4)
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.3f);
            eAttack += attackMod;
           
            CalculateHit();
            animator.SetBool("Attack", true);
            ++timeAttacking;
        }

        else if (timeAttacking == 5)
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.4f);
            eAttack += attackMod;
           
            CalculateHit();
            animator.SetBool("Attack", true);
            ++timeAttacking;
        }

        else
        {
            Mathf.CeilToInt(attackMod = eAttack * 0.5f);
            eAttack += attackMod;
          
            CalculateHit();
            animator.SetBool("Attack", true);
        }
        eAttack = enemyStartingAtk;
    }

    void BlowSelfCountDown()
    {
        blowStrength += Random.Range(10, 15);

        if (countDownToBlow > 0)
        {
            blowSelfObject.SetActive(true);
            animator.SetBool("Collect", true);
        }

        else if (countDownToBlow <= 0)
        {
            blow = true;
            currentHP = 0;
        }

        if (blow)
        {
            animator.SetBool("SkillInUse", true);
        }
    }

    void EnemyToHeal()
    {
        int[] healthHolder = new int[battleManager.enemies.Length];// why i did this i will never know change it when not too lazy
        float lowestHealth; //holds ref the lowest health in enemyStat List
       
        for (int i = 0; i < healthHolder.Length; ++i)
        {
            //add enemies currentHp to enemyStat List only if they arnt dead
            if (battleManager.enemies[i].currentHP > 0 && !dead && this != battleManager.enemies[i].enemyReference)
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
    }

    void ModAttack()
    {
        audioManager.PlayThisEffect("buff");
        float modAmount = Random.Range(5, 15) * 0.01f;
        enemyToHeal.BuffStats("Attack", modAmount, 3);
        animator.SetBool("ModAtk", false);
        EndTurn();
    }

    void ModDefence()
    {
        audioManager.PlayThisEffect("buff");
        float modAmount = Random.Range(5, 20) * 0.01f;
        enemyToHeal.BuffStats("Defense", modAmount, 3);
        animator.SetBool("ModDef", false);
        EndTurn();
    }

    void ModAgility()
    {
        audioManager.PlayThisEffect("buff");
        float modAmount = Random.Range(5, 20) * 0.01f;
        enemyToHeal.BuffStats("Agility", modAmount, 3);
        animator.SetBool("ModAgi", false);
        EndTurn();
    }

    void EnemyToMod()
    {
        
        int healAtIndex;
        //add enemies to the list
        for (int i = 0; i < battleManager.enemies.Length; ++i)
        {
            if (battleManager.enemies[i].enemyReference != this && battleManager.enemies[i].enemyReference != null && battleManager.enemies[i].currentHP > 0)
            {
                enemyToHealRef.Add(battleManager.enemies[i].enemyReference); // all the other enemies in the battle go into here 
            }
        }

        if (enemyToHealRef.Count == 5)
        {
            healAtIndex = PickRandomNumber(enemyToHealRef[0].enemyIndexInBattleManager, enemyToHealRef[1].enemyIndexInBattleManager, enemyToHealRef[2].enemyIndexInBattleManager, enemyToHealRef[3].enemyIndexInBattleManager, enemyToHealRef[4].enemyIndexInBattleManager);
            enemyToHeal = battleManager.enemies[healAtIndex].enemyReference;
        }

        else if (enemyToHealRef.Count == 4)
        {
            healAtIndex = PickRandomNumber(enemyToHealRef[0].enemyIndexInBattleManager, enemyToHealRef[1].enemyIndexInBattleManager, enemyToHealRef[2].enemyIndexInBattleManager, enemyToHealRef[3].enemyIndexInBattleManager);
            enemyToHeal = battleManager.enemies[healAtIndex].enemyReference;
        }

        else if (enemyToHealRef.Count == 3)
        {
            healAtIndex = PickRandomNumber(enemyToHealRef[0].enemyIndexInBattleManager, enemyToHealRef[1].enemyIndexInBattleManager, enemyToHealRef[2].enemyIndexInBattleManager);
            enemyToHeal = battleManager.enemies[healAtIndex].enemyReference; ;
        }

        else if (enemyToHealRef.Count == 2)
        {
            healAtIndex = PickRandomNumber(enemyToHealRef[0].enemyIndexInBattleManager, enemyToHealRef[1].enemyIndexInBattleManager);
            enemyToHeal = battleManager.enemies[healAtIndex].enemyReference;
        }

        else
        {
            healAtIndex = enemyToHealRef[enemyToHealRef.Count-1].enemyIndexInBattleManager;
            enemyToHeal = enemyToHealRef[healAtIndex];
        }

        if (enemyToHeal.amountModed >= 2)
        {
            enemyToHeal.isStatModed = true;
        }
    }

    void StatNeeded(PlayerStatReference pStatNeeded)
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

             for (int i = 0; i < playerStatNeeded.Count; i++)
             {
                 if(playerStatNeeded[i] != statsRefForCheck)
                 {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                     
                 }

                 else if (playerStatNeeded[i] == statsRefForCheck)
                 {
                    playerStatNeeded.Remove(playerStatNeeded.Count - 1);
                 }
             }
        }

        else if (statNeeded == PlayerStatReference.Agility)
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
                     
                 }

                 else if (playerStatNeeded[i] == statsRefForCheck)
                 {
                     playerStatNeeded.Remove(playerStatNeeded.Count - 1);
                 }
             }
        }
    }

    //function override used to get the stats a enemy same idea as with the player 
    //  void StatNeeded(EnemyStatReference eStatNeeded)
    // {

    // }
    //Calcualte the damage
    public virtual void TakeDamage(float playerAttack, int numberOfAttacks)
    {
        if (playerAttack > 0.0f)
        {
            float damage = playerAttack - ((actualDEF / (20.0f + actualDEF)) * playerAttack);
            damage *= ralliedDamageModifier;
            currentHP -= damage;
            damageText.gameObject.SetActive(true);
            damageText.text = Mathf.RoundToInt(damage).ToString();
            battleManager.enemies[enemyIndexInBattleManager].currentHP = currentHP; //Update the BTL manager with the new health
            HP.fillAmount = currentHP / maxHP;
            animator.SetBool("Hit", true);


            if (currentStatusAilment0 == EnemyStatusAilment.chained || currentStatusAilment1 == EnemyStatusAilment.chained)
            {
                //If I'm not the primary target, 0 is. Damage 1 before 0 to avoid the turn ending in the case that 0 dies breaking the chain before 1 gets damaged
                if (chainedEnemy[1] != null && !chainedEnemy[1].dead)
                {
                    chainedEnemy[1].TakeChainedDamage(playerAttack * ralliedDamageModifier, numberOfAttacks);
                }

                if (chainedEnemy[0] != null && !chainedEnemy[0].dead)
                {
                    chainedEnemy[0].TakeChainedDamage(playerAttack * ralliedDamageModifier, numberOfAttacks);
                }
            }

            if (currentHP < 1.0f) // Avoid near zero
            {
                if ((currentStatusAilment0 == EnemyStatusAilment.chained || currentStatusAilment1 == EnemyStatusAilment.chained) && primaryChainedTarget) //If the enemy is chained and is the primary target
                {
                    UnchainEnemies();
                }
                animator.SetBool("Death", true);
            }
            else
            {
                if (numberOfAttacks <= 0)
                {
                    //Only end the turn after the damage has been taken
                    //If the enemy is chained, only the primary target can end the turn
                    //All to avoid turn skips
                    if ((currentStatusAilment0 == EnemyStatusAilment.chained || currentStatusAilment1 == EnemyStatusAilment.chained) && primaryChainedTarget)
                    {
                        uiBTL.EndTurn();
                    }
                    else if (currentStatusAilment0 != EnemyStatusAilment.chained && currentStatusAilment1 != EnemyStatusAilment.chained)
                    {
                        uiBTL.EndTurn();
                    }
                }
            }
        }
        else
        {
            if(missText != null)
            {
                missText.gameObject.SetActive(true);
            }
        }
    }

    public virtual void TakeDamage(float playerAttack, int numberOfAttacks, int debuffIndex, float debuffValuePercent, int debuffTimer, string debuffSubIndex,  EnemyStatusAilment ailment )
    {
        if (playerAttack > 0.0f) //Don't need to calcualte damage if the incoming attack is debuff only
        {
            //Didn't recall the original function cause the "Hit" animation ends the turn
            float damage = playerAttack - ((actualDEF / (20.0f + actualDEF)) * playerAttack);
            damage *= ralliedDamageModifier; //Increase damage if rallied;
            currentHP -= damage;
            damageText.gameObject.SetActive(true);
            damageText.text = Mathf.RoundToInt(damage).ToString();
            battleManager.enemies[enemyIndexInBattleManager].currentHP = currentHP; //Update the BTL manager with the new health
            HP.fillAmount = currentHP / maxHP;


            //Damage chained enemies should be chained too
            if (currentStatusAilment0 == EnemyStatusAilment.chained || currentStatusAilment1 == EnemyStatusAilment.chained)
            {
                if (chainedEnemy[1] != null && !chainedEnemy[1].dead)
                {
                    chainedEnemy[1].TakeChainedDamage(playerAttack * ralliedDamageModifier, numberOfAttacks);
                }

                if (chainedEnemy[0] != null && !chainedEnemy[0].dead)
                {
                    chainedEnemy[0].TakeChainedDamage(playerAttack * ralliedDamageModifier, numberOfAttacks);
                }
            }
        }
        if (playerAttack >= 0.0f) //if incoming damage is negative, then it was a miss
        {
            switch (debuffIndex)
            {
                case 0: // Ailment

                    switch (ailment)
                    {
                        case EnemyStatusAilment.chained:
                            if (currentStatusAilment0 != EnemyStatusAilment.chained && currentStatusAilment1 != EnemyStatusAilment.chained) //You can't have two primary chain targets in the same chain
                            {
                                chainedWaitTime = debuffTimer;
                                primaryChainedTarget = true;
                                GetNewChainedEnemies();
                                primaryChainedSymbol.gameObject.SetActive(true);
                            }
                            break;
                        case EnemyStatusAilment.rallied:
                            if (currentStatusAilment0 != EnemyStatusAilment.rallied && currentStatusAilment1 != EnemyStatusAilment.rallied) //Cannot rally an enemy that's already been rallied against
                            {
                                ralliedWaitTime = debuffTimer;
                                ralliedDamageModifier = 2.0f;
                                ralliedSymbol.gameObject.SetActive(true);
                                uiBTL.EndTurn(); //Rally doesn't do any damage
                            }
                            break;
                        case EnemyStatusAilment.burn:
                            if (currentStatusAilment0 != EnemyStatusAilment.burn && currentStatusAilment1 != EnemyStatusAilment.burn) //Cannot burn an enemy that's already been burned
                            {
                                burnedWaitTime = debuffTimer;
                                burnSymbol.gameObject.SetActive(true);
                                uiBTL.EndTurn(); //Rally doesn't do any damage
                            }
                            break;
                    }
                    //Check which of the two ailments is set to none.
                    // If both of them are filled, overwrite the 0 spot
                    if (currentStatusAilment0 == EnemyStatusAilment.none)
                    {
                        currentStatusAilment0 = ailment;
                    }
                    else if (currentStatusAilment1 == EnemyStatusAilment.none)
                    {
                        currentStatusAilment1 = ailment;
                    }
                    else if (currentStatusAilment0 == EnemyStatusAilment.chained)
                    {
                        if (primaryChainedTarget)
                        {
                            UnchainEnemies();
                            primaryChainedTarget = false;
                            currentStatusAilment0 = ailment;
                        }
                        else
                        {
                            NoLongerChained();
                            chainedSymbol.gameObject.SetActive(false);
                        }
                    }
                    else if (currentStatusAilment0 == EnemyStatusAilment.rallied)
                    {
                        NoLongerRallied();
                        currentStatusAilment0 = ailment;
                    }
                    break;
                case 1: //Debuff
                    BuffStats(debuffSubIndex, -debuffValuePercent, debuffTimer);
                    break;
            }

            animator.SetBool("Hit", true);

            if (currentHP < 1.0f) //Avoid near zero
            {
                if ((currentStatusAilment0 == EnemyStatusAilment.chained || currentStatusAilment1 == EnemyStatusAilment.chained) && primaryChainedTarget) //If the enemy is chained and is the primary target, then unchain the enemies
                {
                    UnchainEnemies();
                }
                animator.SetBool("Death", true);
            }
            else
            {
                if (numberOfAttacks <= 0)
                {
                    //Only end the turn after the damage has been taken
                    //If the enemy is chained, only the primary target can end the turn
                    //All to avoid turn skips
                    if ((currentStatusAilment0 == EnemyStatusAilment.chained || currentStatusAilment1 == EnemyStatusAilment.chained) && primaryChainedTarget)
                    {
                        uiBTL.EndTurn();
                    }
                    else if (currentStatusAilment0 != EnemyStatusAilment.chained && currentStatusAilment1 != EnemyStatusAilment.chained)
                    {
                        uiBTL.EndTurn();
                    }
                }
            }
        }
        else
        {
            if (missText != null)
            {
                missText.gameObject.SetActive(true);
            }
        }
    }

    public void TakeChainedDamage(float playerAttack, int numberOfAttacks)
    {
        float damage = playerAttack - ((eDefence / (20.0f + eDefence)) * playerAttack);
        damage *= ralliedDamageModifier;
        currentHP -= damage;
        damageText.gameObject.SetActive(true);
        damageText.text = Mathf.RoundToInt(damage).ToString();
        battleManager.enemies[enemyIndexInBattleManager].currentHP = currentHP; //Update the BTL manager with the new health
        HP.fillAmount = currentHP / maxHP;
        animator.SetBool("Hit", true);

        if (currentHP < 1.0f) //Avoid near zero
        {
            if ((currentStatusAilment0 == EnemyStatusAilment.chained || currentStatusAilment1 == EnemyStatusAilment.chained) && primaryChainedTarget) //If the enemy is chained and is the primary target, then unchain the enemies
            {
                UnchainEnemies();
                if (numberOfAttacks <= 0) //Need to check for number of attacks for this scenario: Say the number of attacks is 2, and the primary target dies from the first, this means the chain breaks and the turn ends, but the second attack is still coming, and that target of that attack is no longer chained so it WILL call EndTurn() which can cause a turn skip
                {
                    uiBTL.EndTurn();
                }
            }

            animator.SetBool("Death", true);
        }
        else if(primaryChainedTarget && numberOfAttacks <=0) //If you're not dead, end the turn when getting hit if you're the primary target
        {
            uiBTL.EndTurn();
        }
    }

    private void TakeBurnDamage()
    {
        //Damage the enemy at the beginning of the turn due to the burining effect
        burnDamage *= ralliedDamageModifier;
        currentHP -= burnDamage;
        damageText.gameObject.SetActive(true);
        damageText.text = Mathf.RoundToInt(burnDamage).ToString();
        battleManager.enemies[enemyIndexInBattleManager].currentHP = currentHP; //Update the BTL manager with the new health
        HP.fillAmount = currentHP / maxHP;

        if (currentHP < 1.0f) //Avoid near zero
        {
            uiBTL.EndTurn();
        }
    }

    #region Status Ailments

    protected void CheckForAilments()
    {
        switch(currentStatusAilment0)
        {
            case EnemyStatusAilment.chained:
                if(primaryChainedTarget) //Only the primary chain target can lift off the effect
                {
                    chainedWaitTime--;
                    if(chainedWaitTime<=0)
                    {
                        UnchainEnemies();
                        primaryChainedTarget = false;
                        currentStatusAilment0 = EnemyStatusAilment.none;
                    }
                }
                break;
            case EnemyStatusAilment.rallied:
                ralliedWaitTime--;
                if(ralliedWaitTime<=0)
                {
                    uiBTL.UpdateActivityText("The Rally effect has ended");
                    ralliedSymbol.gameObject.SetActive(false);
                    ralliedDamageModifier = 1.0f;
                    currentStatusAilment0 = EnemyStatusAilment.none;
                }
                break;
            case EnemyStatusAilment.burn:
                burnedWaitTime--;
                TakeBurnDamage();
                if (burnedWaitTime <= 0)
                {
                    uiBTL.UpdateActivityText("The Burn effect has ended");
                    burnSymbol.gameObject.SetActive(false);
                    currentStatusAilment0 = EnemyStatusAilment.none;
                }
                break;
        }

        switch (currentStatusAilment1)
        {
            case EnemyStatusAilment.chained:
                if (primaryChainedTarget) //Only the primary chain target can lift off the effect
                {
                    chainedWaitTime--;
                    if (chainedWaitTime <= 0)
                    {
                        UnchainEnemies();
                        primaryChainedTarget = false;
                        currentStatusAilment1 = EnemyStatusAilment.none;
                    }
                }
                break;
            case EnemyStatusAilment.rallied:
                ralliedWaitTime--;
                if (ralliedWaitTime <= 0)
                {
                    uiBTL.UpdateActivityText("The Rally effect has ended");
                    ralliedSymbol.gameObject.SetActive(false);
                    ralliedDamageModifier = 1.0f;
                    currentStatusAilment1 = EnemyStatusAilment.none;
                }
                break;
            case EnemyStatusAilment.burn:
                burnedWaitTime--;
                TakeBurnDamage();
                if (burnedWaitTime <= 0)
                {
                    uiBTL.UpdateActivityText("The Burn effect has ended");
                    burnSymbol.gameObject.SetActive(false);
                    currentStatusAilment1 = EnemyStatusAilment.none;
                }
                break;
        }
    }

    #region Chained
    private void UnchainEnemies()
    {
        //Unchain the enemies
        primaryChainedSymbol.gameObject.SetActive(false);

        if (chainedEnemy[0] != null && !chainedEnemy[0].dead)
        {
            chainedEnemy[0].NoLongerChained();
            chainedEnemy[0] = null;
        }

        if (chainedEnemy[1] != null && !chainedEnemy[1].dead)
        {
            chainedEnemy[1].NoLongerChained();
            chainedEnemy[1] = null;
        }
    }

    private void GetNewChainedEnemies() //Called by the enemy primary targeted for chaining
    {
        int j = 0;
        for (int i = 0; i < uiBTL.enemiesDead.Length; i++)
        {
            if (i != enemyIndexInBattleManager)
            {
                //Make sure the enemy is alive and has not been chained before
                if (uiBTL.enemiesDead[i] == false && uiBTL.enemies[i] != null && uiBTL.enemies[i] != chainedEnemy[0] && uiBTL.enemies[i].currentStatusAilment0 != EnemyStatusAilment.chained)
                {
                    chainedEnemy[j] = uiBTL.enemies[i];
                    j++;
                    if (j > 1)
                    {
                        break;
                    }
                }
            }
        }

        //Check if the enemy was chained to two other targets
        if(chainedEnemy[0] != null && chainedEnemy[1]!=null)
        {
            //If yes, then tell them to get chained
            chainedEnemy[0].YouAreNowChained(this, chainedEnemy[1]);
            chainedEnemy[1].YouAreNowChained(this, chainedEnemy[0]);
        }
        else if(chainedEnemy[0] != null)
        {
            //If only one target is chained
            chainedEnemy[0].YouAreNowChained(this, null);
        }
        else
        {
            //If no chained enemies exist. Do nothing
        }
    }

    public void YouAreNowChained(Enemy chain0, Enemy chain1) //Chains the enemy when they are not the ones primary targeted
    {
        //Update the ailment and get references to the other two chained enemies
        currentStatusAilment0 = EnemyStatusAilment.chained;
        chainedEnemy[0] = chain0;
        chainedEnemy[1] = chain1;
        chainedSymbol.gameObject.SetActive(true);
    }

    public void NoLongerChained() //Unchains the enemy. Called by the primary target when the timer runs out or when it dies
    {
        currentStatusAilment0 = EnemyStatusAilment.none;
        chainedEnemy[0] = null;
        chainedEnemy[1] = null;
        chainedSymbol.gameObject.SetActive(false);
    }
    #endregion
    #region Rallied

    public void NoLongerRallied() //Called by the player if they change the rallied target
    {
        ralliedWaitTime = 0;
        ralliedSymbol.gameObject.SetActive(false);
        ralliedDamageModifier = 1.0f;
        if(currentStatusAilment0 == EnemyStatusAilment.rallied)
        {
            currentStatusAilment0 = EnemyStatusAilment.none;
        }
        else if (currentStatusAilment1 == EnemyStatusAilment.rallied)
        {
            currentStatusAilment1 = EnemyStatusAilment.none;
        }
    }

    #endregion

    #region Tied

    public void Untie()
    {
        tiedTimer = 0;
        deadlyTiesObject.gameObject.SetActive(false);
        chain.gameObject.SetActive(false);
        tieThisPlayer = null;
    }

    #endregion
    #endregion //Ailments region
    #region buffs and debuffs

    public void BuffStats(string statToBuff, float precentage, float lastsNumberOfTurns)
    {
        lastsNumberOfTurns++; //Add one more turn since the system should count the number of turns based on the caster not the receiver. This way ensures that the queue goes around equal to the number of turns it the buff/debuff is supposed to last
        switch (statToBuff)
        {
            case "Defense":
                if (precentage > 0)
                {
                    EnableEffect("DefBuff", 0);
                }
                else
                {
                    EnableEffect("DefDebuff", 0);
                }
                if (defenseBuffed && ((actualDEF < eDefence && precentage > 0) || (actualDEF > eDefence && precentage < 0))) //Check for debuffs first
                {
                    actualDEF = eDefence;
                    defenseBuffSkillQCounter = 0; //Negate the debuff completely
                    defBuffArrowIndicator.gameObject.SetActive(false);
                }
                else if (defenseBuffed && ((actualDEF > eDefence && precentage > 0) || (actualDEF < eDefence && precentage < 0))) //If defense has already been buffed, update the Q counter
                {
                    actualDEF = eDefence + eDefence * precentage; //Buffs don't stack
                    defenseBuffSkillQCounter = lastsNumberOfTurns;
                }
                else if (!defenseBuffed) //No buffs or debuffs have occurred so far
                {
                    defenseBuffed = true;
                    actualDEF = eDefence + eDefence * precentage;
                    defenseBuffSkillQCounter = lastsNumberOfTurns;
                }
                break;
            case "Attack":
                if (precentage > 0)
                {
                    EnableEffect("AtkBuff", 0);
                }
                else
                {
                    EnableEffect("AtkDebuff", 0);
                }
                if (attackBuffed && ((eAttack < atkBeforeBUff && precentage > 0) || (eAttack > atkBeforeBUff && precentage < 0))) //Check if we'er being debuffed after being buffed or vice versa, if so, reset the attack
                {
                    eAttack = atkBeforeBUff;
                    attackBuffSkillQCounter = 0; //Negate the debuff completely
                    atkBuffArrowIndicator.gameObject.SetActive(false);
                }
                else if (attackBuffed && ((eAttack > atkBeforeBUff && precentage > 0) || (eAttack < atkBeforeBUff && precentage < 0))) //Check if the buff or debuff is being extended
                {
                    eAttack = atkBeforeBUff + atkBeforeBUff * precentage;
                    attackBuffSkillQCounter = lastsNumberOfTurns;

                }
                else if (!attackBuffed) //No buffs or debuffs have occurred so far
                {
                    attackBuffed = true;
                    eAttack = atkBeforeBUff + atkBeforeBUff * precentage;
                    attackBuffSkillQCounter = lastsNumberOfTurns;
                }
                break;
            case "Agility":
                if (precentage > 0)
                {
                    EnableEffect("AgiBuff", 0);
                }
                else
                {
                    EnableEffect("AgiDebuff", 0);
                }
                if (agilityBuffed && ((actualAgi < eAgility && precentage > 0) || (actualAgi > eAgility && precentage < 0))) //Check for debuffs first
                {
                    actualAgi = eAgility;
                    agilityBuffSkillQCounter = 0; //Negate the debuff completely
                    agiBuffArrowIndicator.gameObject.SetActive(false);
                }
                else if (agilityBuffed && ((actualAgi > eAgility && precentage > 0) || (actualAgi < eAgility && precentage < 0))) //If agility has already been buffed, update the Q counter
                {
                    actualAgi = eAgility + eAgility * precentage;
                    agilityBuffSkillQCounter = lastsNumberOfTurns;

                }
                else if (!agilityBuffed) //No buffs or debuffs have occurred so far
                {
                    agilityBuffed = true;
                    actualAgi = eAgility + eAgility * precentage;
                    agilityBuffSkillQCounter = lastsNumberOfTurns;
                }
                break;
            case "Strength":
                if (precentage > 0)
                {
                    EnableEffect("StrBuff", 0);
                }
                else
                {
                    EnableEffect("StrDebuff", 0);
                }
                if (strBuffed && ((actualSTR < eStrength && precentage > 0) || (actualSTR > eStrength && precentage < 0))) //Check for debuffs first
                {
                    actualSTR = eStrength;
                    strBuffSkillQCounter = 0; //Negate the debuff completely
                    strBuffArrowIndicator.gameObject.SetActive(false);
                }
                else if (strBuffed && ((actualSTR > eStrength && precentage > 0) || (actualSTR < eStrength && precentage < 0))) //If str has already been buffed, update the Q counter
                {
                    actualSTR = eStrength + eStrength * precentage;
                    strBuffSkillQCounter = lastsNumberOfTurns;

                }
                else if (!strBuffed) //No buffs or debuffs have occurred so far
                {

                    strBuffed = true;
                    actualSTR = eStrength + eStrength * precentage;
                    strBuffSkillQCounter = lastsNumberOfTurns;
                }
                break;
        }
    }

    protected void CheckForBuffs()
    {
        if (defenseBuffed && defenseBuffSkillQCounter > 0)
        {
            defenseBuffSkillQCounter--;
            if (defenseBuffSkillQCounter <= 0)
            {
                defenseBuffSkillQCounter = 0;
                defenseBuffed = false;
                actualDEF = eDefence;
                uiBTL.UpdateActivityText("Enemy DEF is back to normal");
                defBuffArrowIndicator.gameObject.SetActive(false);

            }
        }

        if (attackBuffed && attackBuffSkillQCounter > 0)
        {
            attackBuffSkillQCounter--;
            if (attackBuffSkillQCounter <= 0)
            {
                attackBuffSkillQCounter = 0;
                attackBuffed = false;
                eAttack = atkBeforeBUff;
                uiBTL.UpdateActivityText("Enemy ATK is back to normal");
                atkBuffArrowIndicator.gameObject.SetActive(false);
            }
        }

        if (agilityBuffed && agilityBuffSkillQCounter > 0)
        {
            agilityBuffSkillQCounter--;
            if (agilityBuffSkillQCounter <= 0)
            {
                agilityBuffSkillQCounter = 0;
                agilityBuffed = false;
                actualAgi = eAgility;
                uiBTL.UpdateActivityText("Enemy AGI is back to normal");
                agiBuffArrowIndicator.gameObject.SetActive(false);
            }
        }

        if (strBuffed && strBuffSkillQCounter > 0)
        {
            strBuffSkillQCounter--;
            if (strBuffSkillQCounter <= 0)
            {
                strBuffSkillQCounter = 0;
                strBuffed = false;
                actualSTR = eStrength;
                uiBTL.UpdateActivityText("Enemy STR is back to normal");
                strBuffArrowIndicator.gameObject.SetActive(false);
            }
        }
    }

    public void EnableEffect(string effectName, int value) //Value is used by items as they add static amounts rather than percentages. Skills will pass value as zero.
    {
        switch (effectName)
        {
            case "Heal":
                /*  healEffect.gameObject.SetActive(true);
                  if (value > 0) //Check if the function call coming from an item.
                  {
                      healText.gameObject.SetActive(true);
                      healText.text = value.ToString();

                      if (currentAilment == playerAilments.tied) //If the player is tied to an enemy, that enemy should be healed as well
                      {
                          tiedToThisEnemy.HealDueToTied(value);
                      }
                  }
                  */
                break;
                
            case "MP":
               /* mpEffect.gameObject.SetActive(true);
                if (value > 0)
                {
                    mpText.gameObject.SetActive(true);
                    mpText.text = value.ToString();
                }
                break;
                */
            case "DefBuff":
                defBuffEffect.gameObject.SetActive(true);
                defBuffArrowIndicator.gameObject.SetActive(true);
                arrowRotator = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                defBuffArrowIndicator.transform.rotation = arrowRotator;
                break;
            case "DefDebuff":
                debuffColor.r = 0.4958386f;
                debuffColor.g = 0.1921569f;
                debuffColor.b = 0.8588235f;
                debuffArrow.gameObject.SetActive(true);
                debuffArrow.color = debuffColor;
                defBuffArrowIndicator.gameObject.SetActive(true);
                arrowRotator = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                defBuffArrowIndicator.transform.rotation = arrowRotator;
                break;
            case "AtkBuff":
                atkBuffEffect.gameObject.SetActive(true);
                atkBuffArrowIndicator.gameObject.SetActive(true);
                arrowRotator = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                atkBuffArrowIndicator.transform.rotation = arrowRotator;
                break;
            case "AtkDebuff":
                debuffColor.r = 0.8584906f;
                debuffColor.g = 0.1903257f;
                debuffColor.b = 0.1903257f;
                debuffArrow.gameObject.SetActive(true);
                debuffArrow.color = debuffColor;
                atkBuffArrowIndicator.gameObject.SetActive(true);
                arrowRotator = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                atkBuffArrowIndicator.transform.rotation = arrowRotator;
                break;
            case "AgiBuff":
                agiBuffEffect.gameObject.SetActive(true);
                agiBuffArrowIndicator.gameObject.SetActive(true);
                arrowRotator = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                agiBuffArrowIndicator.transform.rotation = arrowRotator;
                break;
            case "AgiDebuff":
                debuffColor.r = 0.03582038f;
                debuffColor.g = 0.3113208f;
                debuffColor.b = 0.01909043f;
                debuffArrow.gameObject.SetActive(true);
                debuffArrow.color = debuffColor;
                agiBuffArrowIndicator.gameObject.SetActive(true);
                arrowRotator = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                agiBuffArrowIndicator.transform.rotation = arrowRotator;
                break;
            case "StrBuff":
                strBuffEffect.gameObject.SetActive(true);
                strBuffArrowIndicator.gameObject.SetActive(true);
                arrowRotator = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                strBuffArrowIndicator.transform.rotation = arrowRotator;
                break;
            case "StrDebuff":
                debuffColor.r = 0.896f;
                debuffColor.g = 0.4148713f;
                debuffColor.b = 0.1940637f;
                debuffArrow.gameObject.SetActive(true);
                debuffArrow.color = debuffColor;
                strBuffArrowIndicator.gameObject.SetActive(true);
                arrowRotator = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                strBuffArrowIndicator.transform.rotation = arrowRotator;
                break;
        }
    }

    private void DisableAllBuffs() //Called when the player dies. Disables all buffs.
    {
        if (attackBuffed)
        {
            eAttack = atkBeforeBUff;
            attackBuffed = false;
            attackBuffSkillQCounter = 0;
        }
        if (defenseBuffed)
        {
            actualDEF = eDefence;
            defenseBuffed = false;
            defenseBuffSkillQCounter = 0;
        }
        if (agilityBuffed)
        {
            actualAgi = eAgility;
            agilityBuffed = false;
            agilityBuffSkillQCounter = 0;
        }
        if (strBuffed)
        {
            actualSTR = eStrength;
            strBuffed = false;
            strBuffSkillQCounter = 0;
        }
    }

    #endregion

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

            case EnemyName.Red_Reptile:
                //Dps
                skillNeedsCharge = true;
                eName = "The Red Reptile ";
                break;
        }
    }

    public void IncreaseStatsBasedOnLevel(int enemyCurrentLevel)
    {
        eCurrentLevel = enemyCurrentLevel;
        //eHP increase is still temporary until we agree how much each class'es HP increases with leveling up
        float skillPoints = enemyCurrentLevel - eBaseLevel;

        if (skillPoints > 0)
        {
            switch (enemyClass)
            {
                case EnemyClassType.DPS:
                    eAttack = Mathf.CeilToInt(eAttack + (skillPoints * 0.3f));
                    eAgility = Mathf.CeilToInt(eAgility + (skillPoints * 0.2f));
                    currentHP = Mathf.CeilToInt(currentHP + (skillPoints * 1.2f));
                    break;

                case EnemyClassType.Tank:
                    eAttack = Mathf.CeilToInt(eAttack + (skillPoints * 0.2f));
                    eDefence = Mathf.CeilToInt(eDefence + (skillPoints * 0.3f));
                    currentHP = Mathf.CeilToInt(currentHP + (skillPoints * 1.5f));
                    break;

                case EnemyClassType.Support:
                    eAttack = Mathf.CeilToInt(eAttack + (skillPoints * 0.2f));
                    eAgility = Mathf.CeilToInt(eAgility + (skillPoints * 0.3f));
                    currentHP = Mathf.CeilToInt(currentHP + (skillPoints * 2.0f));
                    break;
            }
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
                };

                // give wait times to skills 
                if ((int)canUseSkill == skills[0])
                {
                    waitTime = 1;
                }

                else if ((int)canUseSkill == skills[1])
                {
                    waitTime = 1;
                }

                else if ((int)canUseSkill == skills[2])
                {
                    waitTime = 1;

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

                if ((int)canUseSkill == skills[0]) { waitTime = 1; }
               
                else if ((int)canUseSkill == skills[1]) { }
               
                else if ((int)canUseSkill == skills[2]) { waitTime = 1; }
               
                else if((int)canUseSkill == skills[3]) { waitTime = 1; }

                break;

            case EnemyClassType.Support:

                enemy.skills = new int[]
                {
                    (int) AllEnemySkills.Increase_Multiple_Stats,
                    (int) AllEnemySkills.All_Enemy_Heal,
                    
                };

                if ((int)canUseSkill == skills[0])
                {
                    waitTime = 1;
                }

                else if ((int)canUseSkill == skills[1])
                {
                    waitTime = 1;
                }

                break;
        }
    }
 
    //called in enemyTurn
    void MakeSkillsWork(AllEnemySkills ChosenSkill)
    {
       
        int randomRow = Random.Range(0, 1);

        switch (ChosenSkill)
        {
           
            case AllEnemySkills.Ground_Smash:
                animator.SetBool("isWaiting", false);
                animator.SetBool("SkillInUse", true);

                break;

            //Skill that hits a player random amount of time 
          case AllEnemySkills.Slice_And_Dice:
                attackThisPlayer = battleManager.players[playerIndexHolder].playerReference;

                if (attackThisPlayer.dead)
                {
                    ChoosePlayer();
                    MakeSkillsWork(AllEnemySkills.Slice_And_Dice);
                }

                animator.SetBool("isWaiting", false);
                animator.SetBool("SkillInUse", true);

                break;

            #region increase multiple stats
            //increase a stat based off class
            case AllEnemySkills.Increase_Multiple_Stats:
                EnemyToMod();

                if(enemyToHeal == null)
                {
                    DumbAttack();
                }

                if (enemyToHeal.isStatModed)
                {
                    animator.SetBool("isWaiting", false);
                    animator.SetBool("SkillInUse1", true);
                }

                else if (!enemyToHeal.isStatModed)
                {

                    animator.SetBool("isWaiting", false);
                    animator.SetBool("SkillInUse", true);
                }

                break;
            #endregion

            case AllEnemySkills.All_Enemy_Heal:
                animator.SetBool("isWaiting", false);
                animator.SetBool("SkillInUse", true);

                break;


            case AllEnemySkills.Bite:

                animator.SetBool("isWaiting", false);
                animator.SetBool("SkillInUse", true);

                break;
            

            case AllEnemySkills.Earth_Smash:
                animator.SetBool("isWaiting", false);
                animator.SetBool("SkillInUse", true);

                break;
           

            case AllEnemySkills.Blow_Self:
                blowStrength = 200;
                currentHP = 0;
                blow = true;
                animator.SetBool("SkillInUse", true);
                
                break;

            case AllEnemySkills.Ball_Roll:
                animator.SetBool("isWaiting", false);
                animator.SetBool("SkillInUse", true);

                break;

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

    //choose a player for Relentless Enemy 
    void ChoosePlayer()
    {
        // pick a random index
        if (enemyAttack == EnemyAttackType.Relentless)
        {
            playerIndexHolder = battleManager.players[PickRandomNumber(battleManager.players[0].playerReference.playerIndex, battleManager.players[1].playerReference.playerIndex, battleManager.players[2].playerReference.playerIndex, battleManager.players[3].playerReference.playerIndex)].playerReference.playerIndex;
           
        }
    }
    
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
        CalculateHit();
        if (hit)
        {
            attackThisPlayer.TakeDamage(Mathf.CeilToInt(damage));
        }
        else
        {
            attackThisPlayer.TakeDamage(0.0f);
        }
    }

    void AttackOberon(float damage)
    {
        attackThisPlayer = battleManager.players[1].playerReference;
        CalculateHit();
        if (hit)
        {
            attackThisPlayer.TakeDamage(Mathf.CeilToInt(damage));
        }
        else
        {
            attackThisPlayer.TakeDamage(0.0f);
        }
    }

    void AttackFrea(float damage)
    {
        attackThisPlayer = battleManager.players[2].playerReference;
        CalculateHit();
        if (hit)
        {
            attackThisPlayer.TakeDamage(Mathf.CeilToInt(damage));
        }
        else
        {
            attackThisPlayer.TakeDamage(0.0f);
        }
    }

    void AttackArcelus(float damage)
    {
        attackThisPlayer = battleManager.players[3].playerReference;
        CalculateHit();
        if (hit)
        {
            attackThisPlayer.TakeDamage(Mathf.CeilToInt(damage));
        }
        else
        {
            attackThisPlayer.TakeDamage(0.0f);
        }
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
    ///
    void PickPlayer(int playerindex)
    {
        attackThisPlayer = battleManager.players[playerindex].playerReference;
    }

    void AttackPlayer(int playerindex, int damage)
    {
        CalculateHit();
        if (hit)
        {
            attackThisPlayer = battleManager.players[playerindex].playerReference;
            attackThisPlayer.TakeDamage(eAttack);
        }
        else
        {
            attackThisPlayer = battleManager.players[playerindex].playerReference;
            attackThisPlayer.TakeDamage(0.0f);
        }
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

    void EndCollect()
    {
        animator.SetBool("Collect", false);
        blowSelfObject.SetActive(false);
        EndTurn();
    }

    bool OberonHit()
    {
        attackThisPlayer = battleManager.players[1].playerReference;

        CalculateHit();

        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool FargasHit()
    {
        attackThisPlayer = battleManager.players[0].playerReference;

        CalculateHit();

        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool ArcelusHit()
    {
        attackThisPlayer = battleManager.players[3].playerReference;

        CalculateHit();

        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool FreaHit()
    {
        attackThisPlayer = battleManager.players[2].playerReference;

        CalculateHit();

        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void GroundSmashSkill()
    {
        int randomRow = Random.Range(0, 1);
        uiBTL.UpdateActivityText("Thunder Struck");
        if (randomRow == 0)
        {
            
         
            if (OberonHit())
            {
                AttackOberon(eAttack * 1.5f);
                objPooler.SpawnFromPool("LightingBolt", battleManager.players[1].playerReference.gameObject.transform.position, battleManager.players[1].playerReference.gameObject.transform.rotation);
            }
            else
            {

            }

            if (FreaHit()) 
            {
                AttackFrea(eAttack * 1.5f);
                objPooler.SpawnFromPool("LightingBolt", battleManager.players[2].playerReference.gameObject.transform.position, battleManager.players[2].playerReference.gameObject.transform.rotation);
            }
            else
            {

            }
            
        }

        else if (randomRow == 1)
        {
          

            if (FargasHit())
            {
                AttackFargas(eAttack * 1.5f);
                objPooler.SpawnFromPool("LightingBolt", battleManager.players[0].playerReference.gameObject.transform.position, battleManager.players[0].playerReference.gameObject.transform.rotation);
                
            }
            else
            {

            }

            if (ArcelusHit()) 
            {
                AttackArcelus(eAttack * 1.5f);
                objPooler.SpawnFromPool("LightingBolt", battleManager.players[3].playerReference.gameObject.transform.position, battleManager.players[3].playerReference.gameObject.transform.rotation);
                
            }
            else
            {

            }

        }
        audioManager.PlayThisEffect("Lighting");
    }

    void BlowSelf()
    {
        AttackWholeField(blowStrength);
        uiBTL.UpdateActivityText("Explode Self");

        for (int i = 0; i < battleManager.players.Length; ++i)
        {
            if (battleManager.players[i].playerReference != null && !battleManager.players[i].playerReference.dead)
            {
                objPooler.SpawnFromPool("BlowEffect", battleManager.players[i].playerReference.gameObject.transform.position, battleManager.players[i].playerReference.gameObject.transform.rotation);
            }
        }

        for (int i = 0; i < battleManager.enemies.Length; ++i)
        {
            if (battleManager.enemies[i].enemyReference != null && !battleManager.enemies[i].enemyReference.dead)
            {
                objPooler.SpawnFromPool("BlowEffect", battleManager.enemies[i].enemyReference.gameObject.transform.position, battleManager.enemies[i].enemyReference.gameObject.transform.rotation);
            }
        }
        audioManager.PlayThisEffect("Blow");

        blow = false;
        countDownToBlow = 0;
    }

    void BiteSkill()
    {
        uiBTL.UpdateActivityText("Iron Tusk");
        audioManager.PlayThisEffect("Iron Tusk");
        float attackMod;
        attackThisPlayer = battleManager.players[PickRandomNumber(battleManager.players[0].playerReference.playerIndex, battleManager.players[1].playerReference.playerIndex, battleManager.players[2].playerReference.playerIndex, battleManager.players[3].playerReference.playerIndex)].playerReference;
        if (attackThisPlayer.currentHP <= 0)
        {
            BiteSkill();
        }

        Mathf.CeilToInt(attackMod = (eAttack * .5f));
        CalculateHit();
        if(hit)
        {
            attackThisPlayer.TakeDamage(eAttack + attackMod);
            objPooler.SpawnFromPool("BoarBite", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
        }
        else
        {
    
        } 
    }

    void RaiseDefSkill()
    {
        uiBTL.UpdateActivityText("Shields Up");
        float dMod;
        if (waitTime == waitTimeAtStart)
        {
            audioManager.PlayThisEffect("buff");
            dMod = Random.value;
            BuffStats("Defense", dMod, waitTime);
        }

        if (waitTime <= 0)
        {
            animator.SetBool("SkillInUse", false);
            eDefence = enemyStartingDefence;
            currentState = EnemyState.idle;
            waitTime = waitTimeAtStart;
        }
      
    }

    void HealAllSkill()
    {
        uiBTL.UpdateActivityText("Heal All");
        int healthMod = 100;
       

        for (int i = 0; i < battleManager.enemies.Length; ++i)
        {
            audioManager.PlayThisEffect("buff");
            if (battleManager.enemies[i].enemyReference != null && this != battleManager.enemies[i].enemyReference)
            {
                if (!battleManager.enemies[i].enemyReference.dead)
                {
                    enemyToHeal = battleManager.enemies[i].enemyReference;
                    enemyToHeal.currentHP += healthMod;
                    enemyToHeal.HP.fillAmount = enemyToHeal.currentHP;

                    if (enemyToHeal.currentHP > enemyToHeal.maxHP)
                    {
                        enemyToHeal.currentHP = enemyToHeal.maxHP;
                    }
                }

                battleManager.enemies[i].enemyReference.healthObject.SetActive(true);
            }
        }

        if(enemyAttack == EnemyAttackType.Heal_Support)
        {
            currentState = EnemyState.idle;
            waitQTurns = waitTimeAtStart;
            waitTime = waitTimeAtStart;
            animator.SetBool("SkillInUse1", false);
            EndTurn();
        }
    }

    void EarthSmashSkill()
    {
        uiBTL.UpdateActivityText("Earthquake");
        audioManager.PlayThisEffect("EarthSmash");
        eAttack += Random.Range(5, 10);

        objPooler.SpawnFromPool("EarthSmashRocks", battleManager.players[0].playerReference.gameObject.transform.position, battleManager.players[0].playerReference.gameObject.transform.rotation);
        objPooler.SpawnFromPool("EarthSmashRocks", battleManager.players[1].playerReference.gameObject.transform.position, battleManager.players[1].playerReference.gameObject.transform.rotation);
        AttackFargas();
        AttackOberon();

        Invoke("EarthSmashBackEffect", .4f);
        Invoke("AttackFrea", .5f);
        Invoke("AttackArcelus", .5f);
        
        eAttack = enemyStartingAtk;
    }

    //change to wind debuff in the future 
    void BallRollSkill()
    {
        uiBTL.UpdateActivityText("Cave Winds");
        bool obHit = OberonHit();
        bool freaHit = FreaHit();
        bool arcHit = ArcelusHit();
        bool farHit = FargasHit();
        audioManager.PlayThisEffect("BatWind");
        
            if (obHit)
            {
                AttackOberon();
                OBatWindFx();
            }
            else
            {

            }

            if (freaHit && obHit)
            {
                eAttack += 2;
                Invoke("AttackFrea", 0.4f);
                Invoke("FeaBatWindFx", 0.4f);
            }

            else if(freaHit && !obHit)
            {
                AttackFrea();
                FeaBatWindFx();
            }

            else
            {

            }
           
            if(arcHit && freaHit && obHit)
            {
                eAttack += 3;
                Invoke("AttackArcelus", 0.6f);
                Invoke("ArcBatWindFx", 0.6f);
            }

            else if(arcHit && freaHit && !obHit)
            {
                eAttack += 2;
                Invoke("AttackArcelus", 0.4f);
                Invoke("ArcBatWindFx", 0.4f);
            }

            else if(arcHit && !freaHit && obHit)
            {
                eAttack += 2;
                Invoke("AttackArcelus", 0.4f);
                Invoke("ArcBatWindFx", 0.4f);
            }
            else if(arcHit && !freaHit && !obHit)
            {
                AttackArcelus();
                ArcBatWindFx();
            }
            else
            {

            }

            if (farHit && arcHit && freaHit && obHit)
            {
                eAttack += 4;
                Invoke("AttackFargas", 0.8f);
                Invoke(" FarBatWindFx", 0.8f);
            }

            else if (farHit && arcHit && freaHit && !obHit)
            {
                Invoke("AttackFargas", 0.6f);
                Invoke(" FarBatWindFx", 0.6f);
            }
            else if (farHit && arcHit && !freaHit && !obHit)
            {
                Invoke("AttackFargas", 0.4f);
                Invoke("FarBatWindFx", 0.4f);
            }
            else if (farHit && !arcHit && !freaHit && !obHit)
            {
                AttackFargas();
                FarBatWindFx();
            }
            else if(farHit && !arcHit && !freaHit && obHit)
            {
                Invoke("AttackFargas", 0.4f);
                Invoke("FarBatWindFx", 0.4f);
            }
            else if(farHit && !arcHit && freaHit && obHit)
            {
                Invoke("AttackFargas", 0.6f);
                Invoke("FarBatWindFx", 0.6f);
            }
            else if(farHit && arcHit && !freaHit && obHit)
            {
                Invoke("AttackFargas", 0.6f);
                Invoke("FarBatWindFx", 0.6f);
            }
            else
            {
             
            }

            

        

       
        eAttack = enemyStartingAtk;
    }

    void SliceAndDiceSkill()
    {
        uiBTL.UpdateActivityText("Slice and Dice");

        if (hitAmount == 1)
        {
            CalculateHit();
            if (hit)
            {
                attackThisPlayer.TakeDamage(eAttack + (eAttack * .1f));
                objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                audioManager.PlayThisEffect("SliceAndDice");
            }
            else
            {
                
            }
            EndSkill();
        }
        
        else if(hitAmount == 2)
        {
            if (timeAttackingForSlice == 1)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .1f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if(timeAttackingForSlice == 2)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .2f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                EndSkill();
                timeAttackingForSlice = 1;
                eAttack = enemyStartingAtk;
            }
        }

        else if (hitAmount == 3)
        {
            if (timeAttackingForSlice == 1)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .1f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if (timeAttackingForSlice == 2)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .2f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                
                timeAttackingForSlice++;
            }

            else if (timeAttackingForSlice == 3)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .3f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                EndSkill();
                timeAttackingForSlice = 1;
            }
        }

        else if(hitAmount == 4)
        {
            if (timeAttackingForSlice == 1)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .1f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if (timeAttackingForSlice == 2)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .2f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if (timeAttackingForSlice == 3)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .3f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if (timeAttackingForSlice == 4)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .4f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                EndSkill();
                timeAttackingForSlice = 1;
            }
        }

        else if (hitAmount == 4)
        {
            CalculateHit();
            if (timeAttackingForSlice == 1)
            {
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .1f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if (timeAttackingForSlice == 2)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .2f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if (timeAttackingForSlice == 3)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack*.3f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if (timeAttackingForSlice == 4)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(eAttack + (eAttack * .4f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                timeAttackingForSlice++;
            }

            else if(timeAttackingForSlice == 5)
            {
                CalculateHit();
                if (hit)
                {
                    attackThisPlayer.TakeDamage(+(eAttack * .5f));
                    objPooler.SpawnFromPool("SliceAndDice", attackThisPlayer.gameObject.transform.position, attackThisPlayer.gameObject.transform.rotation);
                    audioManager.PlayThisEffect("SliceAndDice");
                }
                else
                {
                    
                }
                EndSkill();
                timeAttackingForSlice = 1;
            }
        }
        
    }

    void MultipleStatSkill()
    {
        uiBTL.UpdateActivityText("Spore Spray");
        int statIncrease = PickRandomNumber(10, 15);

        if (enemyToHeal.enemyClass == EnemyClassType.DPS)
        {
            enemyToHeal.BuffStats("Attack", statIncrease * .06f, 4);
            enemyToHeal.BuffStats("Agility", statIncrease * .04f, 4);
        }

        else if (enemyToHeal.enemyClass == EnemyClassType.Tank)
        {
            enemyToHeal.BuffStats("Attack", statIncrease * .03f, 4);
            enemyToHeal.BuffStats("Defense", statIncrease * .07f, 4);
        }

        else if (enemyToHeal.enemyClass == EnemyClassType.Support)
        {
            enemyToHeal.BuffStats("Attack", statIncrease * .04f, 4); 
            enemyToHeal.BuffStats("Agility", statIncrease * .06f, 4);
        }

        EndSkill();
    }

    void EarthSmashBackEffect()
    {
        objPooler.SpawnFromPool("EarthSmashRocks", battleManager.players[2].playerReference.gameObject.transform.position, battleManager.players[2].playerReference.gameObject.transform.rotation);
        objPooler.SpawnFromPool("EarthSmashRocks", battleManager.players[3].playerReference.gameObject.transform.position, battleManager.players[3].playerReference.gameObject.transform.rotation);
    }

    void OBatWindFx()
    {
        objPooler.SpawnFromPool("BatWind", battleManager.players[1].playerReference.gameObject.transform.position, battleManager.players[1].playerReference.gameObject.transform.rotation);

    }

    void FarBatWindFx()
    {
        objPooler.SpawnFromPool("BatWind", battleManager.players[0].playerReference.gameObject.transform.position, battleManager.players[0].playerReference.gameObject.transform.rotation);

    }

    void FeaBatWindFx()
    {
        objPooler.SpawnFromPool("BatWind", battleManager.players[2].playerReference.gameObject.transform.position, battleManager.players[2].playerReference.gameObject.transform.rotation);

    }

    void ArcBatWindFx()
    {
        objPooler.SpawnFromPool("BatWind", battleManager.players[3].playerReference.gameObject.transform.position, battleManager.players[3].playerReference.gameObject.transform.rotation);

    }

    protected void Death()
    {
        if (!dead)
        {
            uiBTL.UpdateActivityText("Dead");
            currentState = EnemyState.idle;
            spriteRenderer.enabled = false;
            enemyCanvas.SetActive(false);
            dead = true;
            gameObject.SetActive(false);
            uiBTL.EnemyIsDead(enemyIndexInBattleManager);

            //Only end the turn after the damage has been taken
            //If the enemy is chained, only the primary target can end the turn
            //All to avoid turn skips
            if ((currentStatusAilment0 == EnemyStatusAilment.chained || currentStatusAilment1 == EnemyStatusAilment.chained) && primaryChainedTarget)
            {
                primaryChainedTarget = false;
                uiBTL.EndTurn();
            }
            else if (currentStatusAilment0 != EnemyStatusAilment.chained && currentStatusAilment1 != EnemyStatusAilment.chained)
            {
                uiBTL.EndTurn();
            }
        }
    }

    void Waiting()
    {
        if (waitTime == waitTimeAtStart && playedWaitingText)
        {
            uiBTL.UpdateActivityText("Waiting " + waitTime + " Turns");
            if(enemyName == EnemyName.Boar)
            {
                BoarWaitingSound();
                Invoke("BoarWaitingSound",.16f);
                Invoke("BoarWaitingSound",.3f);


            }
            playedWaitingText = false;
        }
    }

    //used for invoking 
    void BoarWaitingSound()
    {
        audioManager.PlayThisEffect("Boar Waiting");
    }
    void HealEnemy()
    {
        audioManager.PlayThisEffect("buff");
        int healthMod = Random.Range(5, 20);// how much health should be applied to the enemies currentHP
        if (enemyToHeal.currentHP + healthMod >= enemyToHeal.maxHP)
        {
            enemyToHeal.currentHP = enemyToHeal.maxHP;
            battleManager.enemies[enemyToHeal.enemyIndexInBattleManager].currentHP = enemyToHeal.currentHP;
            enemyToHeal.HP.fillAmount = enemyToHeal.currentHP;
        }

        else
        {
            enemyToHeal.currentHP += healthMod;
            battleManager.enemies[enemyToHeal.enemyIndexInBattleManager].currentHP = enemyToHeal.currentHP;
            enemyToHeal.HP.fillAmount = enemyToHeal.currentHP /enemyToHeal.maxHP;
        }

        enemyToHeal.healthObject.SetActive(true);
        animator.SetBool("Heal", false);
        enemyStatNeeded.Clear();
        EndTurn();
    }

    //An enemy tied to a player should get healed when the player is healed. Called from the tied player 
    public virtual void HealDueToTied(float healAmount)
    { 
        currentHP += healAmount;
        if (currentHP >= maxHP)
            currentHP = maxHP;
        HP.fillAmount = currentHP / maxHP; 
        healthObject.gameObject.SetActive(true); 
        tiedTimer--; //Tied timer decreases when damaged or healed 
        if (tiedTimer <= 0) 
        { 
            tiedTimer = 0; 
            if(tieThisPlayer!=null)
            tieThisPlayer.Untie(); 
            chain.gameObject.SetActive(false); 
        } 
    }

    public virtual void Heal(float healAmount)
    {
        currentHP += healAmount;
		if (currentHP > maxHP) currentHP = maxHP;
        HP.fillAmount = currentHP / maxHP;
        healthObject.gameObject.SetActive(true);
    }
}

