using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Code written by: Your moma and I
//Date: Who the fuck knows

public class Enemy : MonoBehaviour
{
    public int enemyIndexInBattleManager;
    private BattleManager battleManager;
    private UIBTL uiBTL;
    private  EnemySkills eSkills;
    public float eMana;
    public float eAttack;
    public float eAgility;
    public float eDefence;
    public float eStrength;
    public float eSpeed;
    public int[] showMeWhatYouGot;
    public float eBaseLevel;
    public int eCurrentLevel;
    public string eName;
    public float eRange;
    public List<float> playerStatNeeded;
    public List<float> enemyStatNeeded;
    public float eCritical;
    public Sprite qImage;
    Player attackThisPlayer;
    int statMod;
    Enemy enemyToHeal;
    Player[] attackSomePlayers = new Player[4];

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
    public bool usedSkill;
    public bool skillAttacksTwo;

    private GameObject demoEffect;
    private ObjectPooler objPooler;

    public EnemyClassType enemyClass;
    public EnemyAttackType enemyAttack;
    public EnemyName enemyName;
    [SerializeField] AllEnemySkills canUseSkill;

    private void Start()
    {
        GiveNamesAndSkills();
        battleManager = BattleManager.instance;
        objPooler = ObjectPooler.instance;
        uiBTL = UIBTL.instance;
        eSkills = EnemySkills.Instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
        animator = gameObject.GetComponent<Animator>();

        haveAddedMyself = false;
        hit = false;
        dead = false;
        usedSkill = false;
        skillAttacksTwo = false;
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
            HealEnemy();
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
        battleManager.AddEnemy(enemyIndexInBattleManager, Mathf.RoundToInt(eAgility), Mathf.RoundToInt(eStrength), Mathf.RoundToInt(eCritical), Mathf.RoundToInt(eSpeed), Mathf.RoundToInt(currentHP),Mathf.RoundToInt(maxHP) ,this, name);
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
                    if (skillChance > 40)
                    {
                        HealEnemy();
                    }

                    else
                    {
                        HealEnemy();
                    }
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

                case EnemyAttackType.Demo:

                    DumbAttack();

                    break;
            }
        }

        else
        {
            EnemySkills.usedSkill = false;
            uiBTL.EndTurn();
        }
    }

    //Calculate whether the attack is a hit or a miss
    private void CalculateHit()
    {
        //if (/*!skillAttacksTwo*/)
      //  {
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

    private void DemoAttackEffect()
    {
        demoEffect = objPooler.SpawnFromPool("DemoAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
    }

    //Called from the animator once the attack anaimation ends
    private void CompleteAttack()
    {
        float critMod = 1.2f;
        if (hit)
        {
            if (skillAttacksTwo) 
            {
                for (int i = 0; i < attackSomePlayers.Length; ++i)
                {
                    objPooler.SpawnFromPool("EnemyNormalAttack", attackSomePlayers[i].gameObject.transform.position, gameObject.transform.rotation);
                }
            }

            else
            {
                objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
            }
            
            if (CalculateCrit() <= eCritical)
            {
                Debug.Log("Critical Hit from Enemy");
                attackThisPlayer.TakeDamage(eAttack * critMod);
            }

            else
            {
                if (!usedSkill)
                {
                    if (!skillAttacksTwo)
                    {
                        Debug.Log("Critical Hit from Enemy");
                        attackThisPlayer.TakeDamage(eAttack);
                    }
                }

                else
                {
                    if (skillAttacksTwo)
                    {
                        for (int i = 0; i < attackSomePlayers.Length; ++i)
                        {
                            attackSomePlayers[i].TakeDamage(eAttack);
                        }
                    }

                    else
                    {
                        attackThisPlayer.TakeDamage(eAttack);
                    }
                }
            }
        }

        else
        {
            Debug.Log("Enemy has missed");
        }

        animator.SetBool("Attack", false);
        uiBTL.EndTurn();
    }

    void DumbAttack()
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
        StatNeeded(PlayerStatReference.Health);
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

    public void SkillAttack(int[] whoToAttack)
    {
        usedSkill = true;
        skillAttacksTwo = true;
        for (int i = 0; i < battleManager.players.Length; i++) 
        {
            for (int j = 0; j < whoToAttack.Length; j++)
            {
                if (battleManager.players[i].playerReference.playerIndex == whoToAttack[j])
                {
                    attackSomePlayers[i] = battleManager.players[i].playerReference;
                    CalculateHit();
                    animator.SetBool("Attack", true);
                }
            }
        }
    }

    void SkillAttack(int whoToAttack)
    {

    }

    void HealEnemy()
    {
        int[] healthHolder = new int[battleManager.enemies.Length];// why i did this i will never know change it when not too lazy
        float lowestHealth; //holds ref the lowest health in enemyStat List
        int healthMod = Random.Range(5, 20);// how much health should be applied to the enemies currentHP
        float chanceOfHealth = Random.value; //  how low should  the health be before it is healed //CHANGE THIS FUCKING VARIABLE NAME ANDRE!!

        for (int i = 0; i < healthHolder.Length; ++i)
        {
            //add enemies currentHp to enemyStat List only if they arnt dead
            if (battleManager.enemies[i].currentHP > 0 && !dead)
            {
                enemyStatNeeded.Add(battleManager.enemies[i].currentHP);
            }
        }

        lowestHealth = Mathf.Min(enemyStatNeeded.ToArray()); 

        for (int i = 0; i <enemyStatNeeded.Count; ++i)
        {
            //if enemyStat is not the lowestHealth Remove it 
            if(enemyStatNeeded[i] != lowestHealth)
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
        if (enemyToHeal.currentHP <= (enemyToHeal.maxHP * chanceOfHealth + .1f))
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

        print(enemyToHeal.enemyIndexInBattleManager);
        enemyStatNeeded.Clear(); 
    }

    void SupportHeal()
    {
        int statMod = Random.Range(5, 20); // temp values 
        float statToMod = Random.value + .1f;
        

        for (int i = 0; i < battleManager.enemies.Length; ++i)
        {

        }

    }

    // returns the stat needed for the enemies that attack based on player stats 
    void StatNeeded(PlayerStatReference pStatNeeded)
    {
        float pStatsRefForCheck = 0; //  i know shitty name 
                                     //returns the lowest HP of the party 
        if (pStatNeeded == PlayerStatReference.Health)
        {
            foreach (BattleManager.PlayerInformtion stat in battleManager.players)
            {
                //checks if player current hp from battlemanager is above 0 and if player exist if that's the case add to list
                if (stat.playerReference != null && stat.currentHP > 0)
                {
                    playerStatNeeded.Add(stat.currentHP);
                    //return;
                }
            }
            //sort the list DUH
            playerStatNeeded.Sort();

            pStatsRefForCheck += Mathf.Min(playerStatNeeded.ToArray());

            for (int i = 0; i < playerStatNeeded.Count; i++)
            {
                if (playerStatNeeded[i] != pStatsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                    print("Removed" + battleManager.players[i].name);
                }

                else if (playerStatNeeded[i] == pStatsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded.Count - 1);
                }
            }
        }


        else if (pStatNeeded == PlayerStatReference.Agility)
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

            pStatsRefForCheck = Mathf.Max(playerStatNeeded.ToArray());

            for (int i = 0; i < playerStatNeeded.Count; i++)
            {
                if (playerStatNeeded[i] != pStatsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                    print("Removed" + battleManager.players[i].name);
                }

                else if (playerStatNeeded[i] == pStatsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded.Count - 1);
                }
            }
        }

        else if (pStatNeeded == PlayerStatReference.Attack)
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

            pStatsRefForCheck += Mathf.Max(playerStatNeeded.ToArray());

            for (int i = 0; i < playerStatNeeded.Count; i++)
            {
                if (playerStatNeeded[i] != pStatsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                    print("Removed" + battleManager.players[i].name);
                }

                else if (playerStatNeeded[i] == pStatsRefForCheck)
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

            pStatsRefForCheck += Mathf.Min(playerStatNeeded.ToArray());
            for (int i = 0; i < playerStatNeeded.Count; i++)
            {
                if (playerStatNeeded[i] != pStatsRefForCheck)
                {
                    playerStatNeeded.Remove(playerStatNeeded[i]);
                    print("Removed" + battleManager.players[i].name);
                }

                else if (playerStatNeeded[i] == pStatsRefForCheck)
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

        if (currentHP <= 0.0f)
        {
            Death();
        }
    }

    public void EndHitAnimation()
    {
        animator.SetBool("Hit", false);
    }

    public void GiveNamesAndSkills()
    {
        int randomskill = Random.Range(0, 100);
        switch (enemyName)
        {
            case EnemyName.Bat:
                eName = "The Bat";
                break;

            case EnemyName.Boar:
                eName = "The Boar";
                break;

            case EnemyName.Dino:
                eName = "The Dino";
                break;

            case EnemyName.Dragon:
                eName = "The Dragon";
                break;

            case EnemyName.Ghost:
                eName = "The Ghost";
                break;

            case EnemyName.Giant:
                eName = "The Giant";

                if (randomskill <= 50)
                {
                    canUseSkill = AllEnemySkills.Ground_Smash;
                }

                else
                {
                    canUseSkill = AllEnemySkills.Ground_Smash;
                }

                break;

            case EnemyName.Mimic:
                eName = "The Mimic";
                break;

            case EnemyName.Mushroom:
                eName = "The Most Dangerous Mushroom";
                break;

            case EnemyName.Octodad:
                eName = "The Octodad";
                break;

            case EnemyName.Reptile:
                eName = "King Krool";
                break;

            case EnemyName.Slime:
                eName = "The Slime";
                break;

            case EnemyName.Snake:
                eName = "The Ekans";
                break;

            case EnemyName.Yeti:
                eName = "The Abominable Snowman";
                break;

            case EnemyName.Solider:
                eName = "The Soilder";
                break;

            case EnemyName.Lieutenant:
                eName = "The Lieutentant";
                break;
        }
    }

    //assigns skills functionalilty for skills that effect more than one in the current battle
    //called in enemyTurn
     void MakeSkillsWork(AllEnemySkills ChosenSkill)
     {
        switch (ChosenSkill)
        {
            case AllEnemySkills.Ground_Smash:
                SkillAttack(eSkills.GroundSmashSkill());
                break;
        }
     }

    private void Death()
    {
        spriteRenderer.enabled = false;
        enemyCanvas.SetActive(false);
        dead = true;
        uiBTL.EnemyIsDead(enemyIndexInBattleManager);
    }
}