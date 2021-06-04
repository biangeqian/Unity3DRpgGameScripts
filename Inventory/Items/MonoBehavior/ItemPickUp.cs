using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //将物品添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            //检查是否有任务
            QuestManager.Instance.UpdateQuestProgress(itemData.itemName,itemData.itemAmount);
            Destroy(gameObject);
        }
        
    }
}
