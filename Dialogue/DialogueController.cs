using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;
    bool canTalk=false;
    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player")&&currentData!=null)
        {
            canTalk=true;
        }
    }
    void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            canTalk=false;
        }
    }
    void Update() 
    {
        if(Keyboard.current.eKey.wasPressedThisFrame&&canTalk)
        {
            OpenDialogue();
        }
    }
    void OpenDialogue()
    {
        //打开UI面板
        //传输对话内容信息
        DialogueUI.Instance.UpdateDialogueData(currentData);
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }
}
