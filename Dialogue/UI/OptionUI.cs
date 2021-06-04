using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;
    private Button thisButton;
    private DialoguePiece currentPiece;
    private string nextPieceID;
    private bool takeQuest;
    void Awake() 
    {
        thisButton=GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);
    }
    public void UpdateOption(DialoguePiece piece,DialogueOption option) 
    {
        currentPiece=piece;
        optionText.text=option.text;
        nextPieceID=option.targetID;
        takeQuest=option.takeQuest;
    }
    public void OnOptionClicked()
    {
        if(currentPiece.quest!=null)
        {
            var newTask=new QuestManager.QuestTask
            {
                questData=Instantiate(currentPiece.quest)
            };
            if(takeQuest)//接受任务
            {
                //添加到任务列表
                //判断任务是否已经存在
                if(QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //判断是否完成,给予奖励
                    if(QuestManager.Instance.GetTask(newTask.questData).IsComplete)
                    {
                        newTask.questData.GiveRewards();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished=true;
                    }
                }
                else
                {
                    //接受任务
                    newTask.IsStarted=true;
                    //检查背包物品
                    foreach(var requireItem in newTask.questData.questRequires)
                    {
                        int sum=InventoryManager.Instance.CheckQuestItemInBag(requireItem.name);
                        requireItem.currentAmount=sum;
                    }
                    newTask.questData.CheckQuestProcess();
                    QuestManager.Instance.tasks.Add(newTask);
                    //QuestManager.Instance.GetTask(newTask.questData).IsStarted=true;
                    

                }
            }
        }
        if(nextPieceID=="")
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            return;
        }
        else
        {
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);
        }
    }
}
