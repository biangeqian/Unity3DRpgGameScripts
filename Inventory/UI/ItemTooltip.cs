using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText;
    public Text itemInfoText;
    RectTransform rectTransform;
    Vector3 mousePos;
    void Awake() 
    {
        rectTransform=GetComponent<RectTransform>();
    }
    public void SetupTooltip(ItemData_SO item,Vector2 pos)
    {
        itemNameText.text=item.itemName;
        itemInfoText.text=item.description;
        mousePos=pos;
    }
    void OnEnable()
    {
        UpdatePosition();
    }
    void Update() 
    {
        UpdatePosition();
    }
    void UpdatePosition()
    {   Vector3[] corners=new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        float width=corners[3].x-corners[0].x;
        float height=corners[1].y-corners[0].y;
        if(mousePos.y<height)
        {
            rectTransform.position=mousePos+Vector3.up*height*0.6f;
        }
        else if(Screen.width-mousePos.x>width)
            rectTransform.position=mousePos+Vector3.right*width*0.6f;
        else
            rectTransform.position=mousePos+Vector3.left*width*0.6f;
        
    }
}
