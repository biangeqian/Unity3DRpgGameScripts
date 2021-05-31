using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
