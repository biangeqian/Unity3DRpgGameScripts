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
}
