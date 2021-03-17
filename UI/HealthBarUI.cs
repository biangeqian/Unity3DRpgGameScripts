using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrebab;
    public Transform barPoint;
    //是否长久可见
    public bool alwaysVisible;
    //可见时间
    public float visibleTime;
    private float timeLeft;

    Image healthSlider;
    //UI的位置
    Transform UIbar;
    //摄像机位置
    Transform cam;
    CharacterStats currentStats;
    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }
    void OnEnable()
    {
        cam = Camera.main.transform;
        foreach(Canvas canvas in FindObjectsOfType<Canvas>())
        {
            //目前只有一个RenderMode.WorldSpace
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrebab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }
    private void UpdateHealthBar(int currentHealth,int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
    //在上一帧渲染后执行
    void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;
            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }

}
