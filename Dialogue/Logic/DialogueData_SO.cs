using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Dialogue",menuName ="Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces=new List<DialoguePiece>();
    public Dictionary<string,DialoguePiece> dialogueIndex=new Dictionary<string, DialoguePiece>();
    //仅在编辑器中执行
#if UNITY_EDITOR
    //数据发生改变时执行
    void OnValidate() 
    {
        dialogueIndex.Clear();
        foreach(var piece in dialoguePieces)
        {
            if(!dialogueIndex.ContainsKey(piece.ID))
            {
                dialogueIndex.Add(piece.ID,piece);
            }
        }
    }
#else
    void Awake() 
    {
        dialogueIndex.Clear();
        foreach(var piece in dialoguePieces)
        {
            if(!dialogueIndex.ContainsKey(piece.ID))
            {
                dialogueIndex.Add(piece.ID,piece);
            }
        }
    }
#endif
    //这里实际上是返回了对话里包含的最后一个任务
    public QuestData_SO GetQuest()
    {
        QuestData_SO currentQuest=null;
        foreach(var piece in dialoguePieces)
        {
            if(piece.quest!=null)
            {
                currentQuest=piece.quest;
            }
        }
        return currentQuest;
    }
}
