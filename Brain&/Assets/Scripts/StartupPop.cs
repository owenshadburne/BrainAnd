using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartupPop : MonoBehaviour
{
    [SerializeField] GameObject[] objects;

    List<Image> img;
    Color[] imgBase, imgAlpha;

    List<TextMeshProUGUI> text;
    Color[] textBase, textAlpha;

    List<SpriteRenderer> sr;
    Color[] spriteBase, spriteAlpha;

    AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] bool activateOnStartup, _fadeIn;
    [SerializeField] float _delay, _timeOfFade;

    void Awake()
    {
        img = new List<Image>();
        text = new List<TextMeshProUGUI>();
        sr = new List<SpriteRenderer>();

        foreach(GameObject o in objects)
        {
            img.AddRange(o.GetComponentsInChildren<Image>(true));
            text.AddRange(o.GetComponentsInChildren<TextMeshProUGUI>(true));
            sr.AddRange(o.GetComponentsInChildren<SpriteRenderer>(true));
        }

        imgBase = new Color[img.Count];
        imgAlpha = new Color[img.Count];
        for (int x = 0; x < img.Count; x++)
        {
            imgBase[x] = img[x].color;
            imgAlpha[x] = new Color(imgBase[x].r, imgBase[x].g, imgBase[x].b, 0);
        }

        textBase = new Color[text.Count];
        textAlpha = new Color[text.Count];
        for (int x = 0; x < text.Count; x++)
        {
            textBase[x] = text[x].color;
            textAlpha[x] = new Color(textBase[x].r, textBase[x].g, textBase[x].b, 0);
        }

        spriteBase = new Color[sr.Count];
        spriteAlpha = new Color[sr.Count];
        for (int x = 0; x < sr.Count; x++)
        {
            spriteBase[x] = sr[x].color;
            spriteAlpha[x] = new Color(spriteBase[x].r, spriteBase[x].g, spriteBase[x].b, 0);
        }

        if (activateOnStartup) { Activate(_fadeIn, _delay, _timeOfFade); }
    }

    public void Activate(bool fadeIn, float delay, float timeOfFade)
    {
        SetAll(!fadeIn);
        if(fadeIn) { StartCoroutine(FadeIn(delay, timeOfFade)); }
        else { StartCoroutine(FadeOut(delay, timeOfFade)); }
    }
    IEnumerator FadeIn(float delay, float timeOfFade)
    {
        yield return new WaitForSeconds(delay);

        float timer = timeOfFade;
        float multiplier = 1 / timeOfFade;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            for (int x = 0; x < img.Count; x++)
            {
                img[x].color = Color.Lerp(imgAlpha[x], imgBase[x], curve.Evaluate(1 - (timer * multiplier)));
            }
            for (int x = 0; x < text.Count; x++)
            {
                text[x].color = Color.Lerp(textAlpha[x], textBase[x], curve.Evaluate(1 - (timer * multiplier)));
            }
            for (int x = 0; x < sr.Count; x++)
            {
                sr[x].color = Color.Lerp(spriteAlpha[x], spriteBase[x], curve.Evaluate(1 - (timer * multiplier)));
            }
            yield return null;
        }
    }
    IEnumerator FadeOut(float delay, float timeOfFade)
    {
        yield return new WaitForSeconds(delay);

        float timer = timeOfFade;
        float multiplier = 1 / timeOfFade;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            for (int x = 0; x < img.Count; x++)
            {
                img[x].color = Color.Lerp(imgBase[x], imgAlpha[x], curve.Evaluate(1 - (timer * multiplier)));
            }
            for (int x = 0; x < text.Count; x++)
            {
                text[x].color = Color.Lerp(textBase[x], textAlpha[x], curve.Evaluate(1 - (timer * multiplier)));
            }
            for (int x = 0; x < sr.Count; x++)
            {
                sr[x].color = Color.Lerp(spriteBase[x], spriteAlpha[x], curve.Evaluate(1 - (timer * multiplier)));
            }
            yield return null;
        }
    }

    void SetAll(bool visible)
    {
        for(int x = 0; x < img.Count; x++)
        {
            img[x].color = visible ? imgBase[x] : imgAlpha[x];
        }
        for (int x = 0; x < text.Count; x++)
        {
            text[x].color = visible ? textBase[x] : textAlpha[x];
        }
        for (int x = 0; x < sr.Count; x++)
        {
            sr[x].color = visible ? spriteBase[x] : spriteAlpha[x];
        }
    }
}
