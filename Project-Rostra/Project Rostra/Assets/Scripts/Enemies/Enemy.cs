using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    public int enemyIndexInBattleManager;
    private BattleManager battleManager;
    public int agi;
    public int str;
    public int crit;
    public string name;
    public Sprite qImage;

    private SpriteRenderer spriteRenderer;
    private Color spriteColor;

    public Image HP;
    private float maxHP;
    private float currentHP;


    private void Start()
    {
        battleManager = BattleManager.instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;

        maxHP = currentHP = 100.0f;


    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            battleManager.AddEnemy(enemyIndexInBattleManager, agi, str, crit, this,name);
            Debug.Log("Added enemy to battlemanager");
        }
    }

    private void EndBattle()
    {
        battleManager.DeleteEnemyFromQueue(enemyIndexInBattleManager);
    }

    public void mynameis()
    {
        Debug.Log("Enemy " + name);
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

    //Temp function
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        HP.fillAmount -= (1- currentHP / maxHP);
    }


}
