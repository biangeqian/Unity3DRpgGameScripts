using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName="Character Stats/Data" )]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    //最大血量
    public int maxHealth;
    //当前血量
    public int currentHealth;
    //基础防御
    public int baseDefence;
    //当前防御
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    //升级后提升百分比
    public float levelBuff;

    public float LevelMultiplier { get { return 1 + (currentLevel - 1) * levelBuff; } }


    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
        {
            LevelUp();
        }
    }
    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1,0,maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);
        maxHealth = (int)(maxHealth * (1 + levelBuff));
        currentHealth = maxHealth;
        Debug.Log("LEVEL UP!" + currentLevel + "Max Health:" + maxHealth);
    }
}
