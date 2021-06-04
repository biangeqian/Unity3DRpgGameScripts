using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    [System.Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;
        public bool IsStarted{get{return questData.isStarted;}set{questData.isStarted=value;}}
        public bool IsComplete{get{return questData.isComplete;}set{questData.isComplete=value;}}
        public bool IsFinished{get{return questData.isFinished;}set{questData.isFinished=value;}}

    }
    public List<QuestTask> tasks=new List<QuestTask>();

    void Start() 
    {
        LoadQuestManager();
    }
    public void SaveQuestManager()
    {
        PlayerPrefs.SetInt("QuestCount",tasks.Count);
        for(int i=0;i<tasks.Count;i++)
        {
            SaveManager.Instance.Save(tasks[i].questData,"task"+i);
        }
        Debug.Log("保存");
    }
    public void LoadQuestManager()
    {
        var questCount=PlayerPrefs.GetInt("QuestTask");
        for(int i=0;i<questCount;i++)
        {
            var newQuest=ScriptableObject.CreateInstance<QuestData_SO>();
            SaveManager.Instance.Load(newQuest,"task"+i);
            tasks.Add(new QuestTask{questData=newQuest});
        }
        Debug.Log("加载");
    }

    //敌人死亡/拾取物品时调用
    public void UpdateQuestProgress(string requirename,int amount)
    {
        foreach(var task in tasks)
        {
            if(task.IsFinished) continue;
            var matchTask=task.questData.questRequires.Find(r=>r.name==requirename);
            if(matchTask!=null)
            {
                matchTask.currentAmount+=amount;
                task.questData.CheckQuestProcess();
            }  
        }
    }

    public bool HaveQuest(QuestData_SO data)
    {
        if(data!=null)
            return tasks.Any(q=>q.questData.questName==data.questName);
        else return false;
    }
    public QuestTask GetTask(QuestData_SO data)
    {
        return tasks.Find(q=>q.questData.questName==data.questName);
    }
}
