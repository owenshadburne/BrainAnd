using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Display : MonoBehaviour
{
    [SerializeField] GameObject inner, middle, outer;
    int mpos = 3, opos = 4;

    bool resetAngle;
    Quaternion finalAngle;

    [SerializeField] ScrollRect scrollRect;
    Button previousButton;
    ColorBlock selected = new ColorBlock();
    int hue = 0, saturation = 100, visability = 100;

    [SerializeField] Transform selector;
    SpriteRenderer selectorSprite;
    float[] selectorPos = new float[] { -8.15f, -6.65f, -5.125f };
    Vector3 defaultPos = new Vector3(-8.13f, 5.5f, 0);
    Color alpha = new Color(0, 0, 0, 0), activeColor = new Color(1, 0, 0, .5f);
    AnimationCurve curve;

    [SerializeField] Transform outContent;
    [SerializeField] ScrollRect outRect;
    [SerializeField] GameObject fakeButton;

    private void Awake()
    {
        selected.normalColor = Color.HSVToRGB(hue, saturation, visability);
        selected.highlightedColor = Color.HSVToRGB(hue, saturation, visability - 4);
        selected.pressedColor = Color.HSVToRGB(hue, saturation, visability - 22);
        selected.selectedColor = Color.HSVToRGB(hue, saturation, visability - 4);
        selected.colorMultiplier = 1;

        curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        selectorSprite = selector.GetComponent<SpriteRenderer>();
    }

    public void SpinCircle(float index, bool isUp, float timeOfSpin)
    {
        float angle = 360;
        GameObject obj = null;
        switch(index)
        {
            case 0:
                angle /= 6;
                obj = inner;
                break;
            case 1:
                angle /= 10;
                obj = middle;
                if (mpos >= 6 || mpos <= 0) { resetAngle = true; mpos = 3; }
                mpos += isUp ? -1 : 1;
                print(mpos);
                break;
            case 2:
                angle /= 14;
                obj = outer;
                if (opos >= 8 || opos <= 0) { resetAngle = true; opos = 4; }
                opos += isUp ? -1 : 1;
                print(opos);
                break;
        }
        if(isUp)
        {
            angle *= -1;
        }

        StartCoroutine(Spin(obj.transform, angle, timeOfSpin));
    }
    IEnumerator Spin(Transform obj, float angle, float timeOfSpin)
    {
        if(resetAngle) { obj.rotation = Quaternion.identity; resetAngle = false; }

        float timer = timeOfSpin;
        float multiplier = 1 / timeOfSpin;

        Quaternion startAngle = obj.rotation;
        finalAngle = Quaternion.Euler(0, 0, obj.transform.rotation.eulerAngles.z + angle);

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            obj.rotation = Quaternion.Lerp(startAngle, finalAngle, curve.Evaluate(1 - (timer * multiplier)));
            yield return null;
        }
    }    

    public void CenterCommand(Button obj)
    {
        if(obj == null) { if(previousButton != null) { previousButton.colors = ColorBlock.defaultColorBlock; } return; }
        if (previousButton != null) { previousButton.colors = ColorBlock.defaultColorBlock; }
        obj.colors = selected;
        previousButton = obj;
        scrollRect.content.localPosition = GetSnapToPositionToBringChildIntoView(scrollRect, obj.GetComponent<RectTransform>());
    }
    public static Vector2 GetSnapToPositionToBringChildIntoView(ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }

    public void MoveSelector(int index, float timeOfMove)
    {
        StartCoroutine(Move(selectorPos[index], timeOfMove));
    }
    IEnumerator Move(float pos, float timeOfMove)
    {
        Vector3 startPos = selector.position;
        Vector3 endPos = new Vector3(pos, startPos.y, startPos.z);

        float timer = timeOfMove;
        float multiplier = 1 / timeOfMove;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            selector.position = Vector3.Lerp(startPos, endPos, curve.Evaluate(1 - timer * multiplier));
            yield return null;
        }
    }
    public void ActivateSelector()
    {
        //selectorSprite.color = activeColor;
    }

    public void Output(string _output)
    {
        TextMeshProUGUI[] texts = outContent.GetComponentsInChildren<TextMeshProUGUI>();
        string[] content = _output.Split('.');

        GameObject lastObj = null;
        for(int x = content.Length - (content.Length - texts.Length); x < content.Length; x++)
        {
            GameObject temp = Instantiate(fakeButton, outContent);
            lastObj = temp;
            TextMeshProUGUI text = temp.GetComponentInChildren<TextMeshProUGUI>();
            text.text = content[x];
        }

        if(lastObj != null) { outRect.content.localPosition = GetSnapToPositionToBringChildIntoView(outRect, lastObj.GetComponent<RectTransform>()); }
    }

    public void Reset()
    {
        ResetOut();
        inner.transform.rotation = Quaternion.identity;
        middle.transform.rotation = Quaternion.identity;
        outer.transform.rotation = Quaternion.identity;

        mpos = 3;
        opos = 4;

        if (previousButton != null) { previousButton.colors = ColorBlock.defaultColorBlock; }
        selector.position = defaultPos;
        //if (selectorSprite != null) { selectorSprite.color = alpha; }
        //else { selector.GetComponent<SpriteRenderer>().color = alpha; }
    }
    void ResetOut()
    {
        for (int x = 0; x < outContent.childCount; x++)
        {
            Destroy(outContent.GetChild(x).gameObject);
        }
    }
}
