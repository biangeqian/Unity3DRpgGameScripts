using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
    CanvasGroup CanvasGroup;
    public float fadeInDuration;
    public float fadeOutDuration;
    void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }
    public IEnumerator FadeOutIn()
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }
    public IEnumerator FadeOut(float time)
    {
        while (CanvasGroup.alpha < 1)
        {
            CanvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }
    public IEnumerator FadeIn(float time)
    {
        while (CanvasGroup.alpha !=0)
        {
            CanvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        Destroy(gameObject);
    }
}
