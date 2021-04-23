using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData{
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }
    [Header("Inventory Data")]
    public InventoryDatat_SO inventoryData;
    public InventoryDatat_SO actionData;
    public InventoryDatat_SO equipmentData;

    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;
    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;
    bool isOpen=false;
    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;

    void Start()
    {
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }
    void Update() {
        if(Keyboard.current.bKey.wasPressedThisFrame){
            isOpen=!isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }
        UpdateStatsText(GameManager.Instance.playerStats.MaxHealth,GameManager.Instance.playerStats.attackData.minDamage,
        GameManager.Instance.playerStats.attackData.maxDamage);
    }
    public void UpdateStatsText(int health,int min,int max){
        healthText.text=health.ToString();
        attackText.text=min+" - "+max;
    }

    #region 检查拖拽物品是否在每一个Slot范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        for(int i=0;i<inventoryUI.slotHolders.Length;i++)
        {
            RectTransform t=inventoryUI.slotHolders[i].transform as RectTransform;
            if(RectTransformUtility.RectangleContainsScreenPoint(t,position)){
                return true;
            }
        }
        return false;
    }
    public bool CheckInActionUI(Vector3 position)
    {
        for(int i=0;i<actionUI.slotHolders.Length;i++)
        {
            RectTransform t=actionUI.slotHolders[i].transform as RectTransform;
            if(RectTransformUtility.RectangleContainsScreenPoint(t,position)){
                return true;
            }
        }
        return false;
    }
    public bool CheckInEquipmentUI(Vector3 position)
    {
        for(int i=0;i<equipmentUI.slotHolders.Length;i++)
        {
            RectTransform t=equipmentUI.slotHolders[i].transform as RectTransform;
            if(RectTransformUtility.RectangleContainsScreenPoint(t,position)){
                return true;
            }
        }
        return false;
    }
    #endregion
}
