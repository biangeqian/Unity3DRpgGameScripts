using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { BAG,WEAPON,ARMOR,ACTION}
public class SlotHolder : MonoBehaviour,IPointerClickHandler
{
    public SlotType slotType;
    public ItemUI itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount%2==0){
            if(itemUI.GetItem()!=null){
                Debug.Log("使用物品");
                UseItem();
            } 
        }
    }
    public void UseItem(){
        if(itemUI.GetItem().itemType==ItemType.Useable&&itemUI.Bag.items[itemUI.Index].amount>0){
            GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableData.healthPoint);
            itemUI.Bag.items[itemUI.Index].amount-=1;
        }
        UpdateItem();
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                //切换武器
                if(itemUI.Bag.items[itemUI.Index].itemData!=null)
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else{
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }
                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
        }
        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetupItemUI(item.itemData, item.amount);
    }
}
