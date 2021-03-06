using System.IO.IsolatedStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    public GameObject questPanel;
    public ItemTooltip tooltip;
    bool isOpen;

    [Header("Quest Name")]
    public RectTransform questListTransform;
    public QuestNameButton questNameButton;

    [Header("Text Content")]
    public Text questContentText;

    [Header("Requirement")]
    public RectTransform requireTransform;
    public QuestRequirement requirement;

    [Header("Reawrd Panel")]
    public RectTransform rewardTransform;
    public ItemUI rewardUI;

    void Update() 
    {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isOpen=!isOpen;
            questPanel.SetActive(isOpen);
            questContentText.text=string.Empty;
            //显示面板内容
            SetupQuestList();

            if(!isOpen)
            {
                tooltip.gameObject.SetActive(false);
            }
        }
    }
    public void SetupQuestList()
    {
        foreach(Transform item in questListTransform)
        {
            Destroy(item.gameObject);
        }
        foreach(Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }
        foreach(Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }

        foreach(var task in QuestManager.Instance.tasks)
        {
            var newTask=Instantiate(questNameButton,questListTransform);
            newTask.SetupNameButton(task.questData);
            //newTask.questContentText=questContentText;
        }
    }

    public void SetupRequireList(QuestData_SO questData)
    {
        questContentText.text=questData.description;
        foreach(Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }
        foreach(var require in questData.questRequires)
        {
            var q=Instantiate(requirement,requireTransform);
            q.SetupRequirement(require.name,require.requireAmount,require.currentAmount);
        }
    }
    public void SetupRewardItem(ItemData_SO itemData,int amount)
    {
        var item=Instantiate(rewardUI,rewardTransform);
        item.SetupItemUI(itemData,amount);
    }
}
