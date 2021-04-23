using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionButton : MonoBehaviour
{
    public KeyCode ActionKey;
    private SlotHolder currentSlotHolder;
    void Awake() {
        currentSlotHolder=GetComponent<SlotHolder>();
    }
    void Update() {
        switch(ActionKey){
            case KeyCode.Alpha1:
                if(Keyboard.current.digit1Key.wasPressedThisFrame&&currentSlotHolder.itemUI.GetItem()){
                    currentSlotHolder.UseItem();
                }
                break;
            case KeyCode.Alpha2:
                if(Keyboard.current.digit2Key.wasPressedThisFrame&&currentSlotHolder.itemUI.GetItem()){
                    currentSlotHolder.UseItem();
                }
                break;
            case KeyCode.Alpha3:
                if(Keyboard.current.digit3Key.wasPressedThisFrame&&currentSlotHolder.itemUI.GetItem()){
                    currentSlotHolder.UseItem();
                }
                break;
            case KeyCode.Alpha4:
                if(Keyboard.current.digit4Key.wasPressedThisFrame&&currentSlotHolder.itemUI.GetItem()){
                    currentSlotHolder.UseItem();
                }
                break;
            case KeyCode.Alpha5:
                if(Keyboard.current.digit5Key.wasPressedThisFrame&&currentSlotHolder.itemUI.GetItem()){
                    currentSlotHolder.UseItem();
                }
                break;
            case KeyCode.Alpha6:
                if(Keyboard.current.digit6Key.wasPressedThisFrame&&currentSlotHolder.itemUI.GetItem()){
                    currentSlotHolder.UseItem();
                }
                break;
            default:
                break;
        }
    }
}
