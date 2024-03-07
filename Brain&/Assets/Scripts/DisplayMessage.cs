using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayMessage : MonoBehaviour
{
    [SerializeField] GameObject display;
    GameObject currentDisplay;
    public static DisplayMessage instance;
    [SerializeField] AnimationCurve anim;

    Color good = new Color(1f, 0.9686275f, .5f), bad = new Color(.8f, 0, 0);

    void Awake()
    {
        instance = this;
        //anim = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0),
           // new Keyframe(.25f, .1f), new Keyframe(.5f, .25f), new Keyframe(.8f, .5f), new Keyframe(1, 1)});
    }

    public void DisplayAMessage(string message, bool isGood)
    {
        StopAllCoroutines();
        if(currentDisplay != null) { Destroy(currentDisplay); }

        currentDisplay = Instantiate(display, GameObject.FindGameObjectWithTag("Canvas").transform);
        Image[] img = currentDisplay.GetComponentsInChildren<Image>();
        TextMeshProUGUI text = currentDisplay.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;
        
        StartCoroutine(Decolor(img, text, isGood));
    }

    IEnumerator Decolor(Image[] img, TextMeshProUGUI text, bool isGood)
    {
        float timer = 1.5f;
        float multiplier = 1 / timer;

        Color[] imgColor = new Color[img.Length];
        Color[] imgAlpha = new Color[img.Length];
        for (int x = 0; x < img.Length; x++)
        {
            imgColor[x] = x == 0 ? (isGood ? good : bad) : img[x].color;
            imgAlpha[x] = new Color(img[x].color.r, img[x].color.g, img[x].color.b, 0);
        }

        Color textColor = text.color;
        Color textAlpha = new Color(text.color.r, text.color.g, text.color.b, 0);

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            for(int x = 0; x < img.Length; x++)
            {
                img[x].color = Color.Lerp(imgColor[x], imgAlpha[x], anim.Evaluate(1 - timer * multiplier));
            }
            text.color = Color.Lerp(textColor, textAlpha, anim.Evaluate(1 - timer * multiplier));
            yield return null;
        }

        if (currentDisplay != null) { Destroy(currentDisplay); }
    }
}
