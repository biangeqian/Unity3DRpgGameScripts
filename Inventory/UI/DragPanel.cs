using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour,IDragHandler,IPointerDownHandler
{
    RectTransform rectTransform;
    Canvas canvas;
    void Awake() {
        rectTransform=GetComponent<RectTransform>();
        canvas=InventoryManager.Instance.GetComponent<Canvas>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        //注意匹配屏幕分辨率
        rectTransform.anchoredPosition+=eventData.delta/canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //2即DragCanvas的上一个,如果添加新的Canvas就要修改这个数字
        rectTransform.SetSiblingIndex(2);
    }
}
