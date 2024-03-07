using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] CommandBar cmd;
    public static LevelSelection instance;

    [SerializeField] GameObject list, listButton, screenButton;
    GameObject canvas, activeList, activeSB;

    TextMeshProUGUI text;

    ColorBlock completed = new ColorBlock(), current = new ColorBlock();
    float hue = 0, saturation = 100, visability = 100;

    private void Start()
    {
        instance = this;

        canvas = GameObject.FindGameObjectWithTag("Canvas");
        text = GetComponentInChildren<TextMeshProUGUI>();

        ColorBlocks();
    }
    void ColorBlocks()
    {
        current.normalColor = Color.HSVToRGB(hue, saturation, visability);
        current.highlightedColor = Color.HSVToRGB(hue, saturation, visability - 4);
        current.pressedColor = Color.HSVToRGB(hue, saturation, visability - 22);
        current.selectedColor = Color.HSVToRGB(hue, saturation, visability - 4);
        current.colorMultiplier = 1;

        Color c = new Color(1f, 0.9686275f, .5f);
        completed.normalColor = c; //Color.HSVToRGB(hue, saturation, visability);
        completed.highlightedColor = c; // Color.HSVToRGB(hue, saturation, visability - 4);
        completed.pressedColor = c; // Color.HSVToRGB(hue, saturation, visability - 22);
        completed.selectedColor = c; // Color.HSVToRGB(hue, saturation, visability - 4);
        completed.colorMultiplier = 1;
    }

    private void Update()
    {
        text.text = "Lv. " + Restrictions.level;
    }

    public void OnClick()
    {
        activeSB = Instantiate(screenButton, canvas.transform, false);
        activeSB.GetComponent<Button>().onClick.AddListener(() => SemiRefresh());

        activeList = Instantiate(list, canvas.transform, false);
        Populate();
    }
    void Populate()
    {
        Transform content = GameObject.FindGameObjectWithTag("ListContent").transform;

        for(int x = 0; x < Restrictions.totalLevels + 1; x++)
        {
            string lv = x.ToString();
            GameObject temp = Instantiate(listButton, content);
            temp.name = "To level " + lv;

            Button b = temp.GetComponent<Button>();

            TextMeshProUGUI t = temp.GetComponentInChildren<TextMeshProUGUI>();
            if (lv == "0")
            {
                t.text = "Playground";
                t.fontSize -= 10;
            }
            else { t.text = lv; }

            if(x == Restrictions.level)
            {
                b.colors = current;
            }
            else if(PlayerPrefs.HasKey(lv) && PlayerPrefs.GetString(lv) == "True")
            {
                b.colors = completed;
            }
        }
    }
    public void Refresh()
    {
        if(activeList != null) { Destroy(activeList); }
        if(activeSB != null) { Destroy(activeSB); }
        cmd.Reset();
    }
    public void SemiRefresh()
    {
        if (activeList != null) { Destroy(activeList); }
        if (activeSB != null) { Destroy(activeSB); }
    }
}
