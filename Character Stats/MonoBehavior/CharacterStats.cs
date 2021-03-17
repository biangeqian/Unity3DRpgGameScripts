﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    //更新血量UI
    public event Action<int, int> UpdateHealthBarOnAttack;
    //模板数据
    public CharacterData_SO templateData;

    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get{if (characterData != null)return characterData.maxHealth;else return 0;}
        set{characterData.maxHealth = value;}
    }
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    //受到攻击
    public void TakeDamage(CharacterStats attacker,CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence,0);
        defender.CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        //暴击时触发挨打的人的受击动画
        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }

        //更新血量UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //结算经验值

    }
    //重载take damage
    public void TakeDamage(int damage,CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 0);
        defender.CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            //Debug.Log("暴击!" + coreDamage);
        }
        return (int)coreDamage;
    }
    #endregion
}
