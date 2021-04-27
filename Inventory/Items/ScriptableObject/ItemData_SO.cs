using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Useable,Weapon,Armor}
[CreateAssetMenu(fileName ="New Item",menuName ="Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;//图标
    public int itemAmount;//本身堆叠的数量
    [TextArea]
    public string description = "";//物品描述
    public bool stackable;//可堆叠

    [Header("Useable")]
    public UseableItemData_SO useableData;
    [Header("Weapon")]
    public GameObject weaponPrefab;
    public AttackData_SO weaponData;
    public AnimatorOverrideController weaponAnimator;
}
