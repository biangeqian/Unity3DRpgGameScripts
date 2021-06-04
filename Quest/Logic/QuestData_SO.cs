using System.Net.NetworkInformation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName ="New Quest",menuName ="Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name;//物品名或怪物名
        public int requireAmount;//需求数量
        public int currentAmount;//当前已完成属性
    }
    public string questName;//任务名,唯一
    [TextArea]
    public string description;
    public bool isStarted;
    public bool isComplete;
    public bool isFinished;//不可重复接取
    
    public List<QuestRequire> questRequires=new List<QuestRequire>();
    public List<InventoryItem> rewards=new List<InventoryItem>();

    public void CheckQuestProcess()
    {
        var finishRequires=questRequires.Where(r=>r.requireAmount<=r.currentAmount);
        isComplete=finishRequires.Count()==questRequires.Count;

        if(isComplete)
        {
            Debug.Log("任务完成");
        }
    }
    public void GiveRewards()
    {
        foreach(var reward in rewards)
        {
            if(reward.amount<0)
            {
                int requireCount=Mathf.Abs(reward.amount);
                if(InventoryManager.Instance.QuestItemInBag(reward.itemData)!=null)
                {
                    if(InventoryManager.Instance.QuestItemInBag(reward.itemData).amount<=requireCount)
                    {
                        requireCount-=InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount=0;

                        if(InventoryManager.Instance.QuestItemInAction(reward.itemData)!=null)
                        {
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount-=requireCount;
                        }
                    }
                    else
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount-=requireCount;
                    }
                }
                else
                {
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount-=requireCount;
                }
            }
            else
            {
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData,reward.amount);
            }
            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }
    //当前任务需求的所有东西
    public List<string> RequireTargetName()
    {
        List<string> targetNameList=new List<string>();
        foreach(var require in questRequires)
        {
            targetNameList.Add(require.name);
        }
        return targetNameList;
    }
}
