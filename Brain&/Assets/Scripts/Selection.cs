using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Selection : MonoBehaviour
{
    CommandBar commandBar;
    Button button;
    TextMeshProUGUI text, useText;
    bool alreadyEnabled = true, setLimit = false;

    ColorBlock selected;
    int hue = 0, saturation = 100, visability = 100;

    int _uses;

    void Awake()
    {
        commandBar = GameObject.FindGameObjectWithTag("CommandBar").GetComponent<CommandBar>();
        button = GetComponent<Button>();
        TextMeshProUGUI[] temp = GetComponentsInChildren<TextMeshProUGUI>();
        text = temp[0];
        useText = temp[1];

        button.onClick.AddListener(() => OnClick());

        SetColorBlock();
    }

    void SetColorBlock()
    {
        selected.normalColor = Color.HSVToRGB(hue, saturation, visability);
        selected.highlightedColor = Color.HSVToRGB(hue, saturation, visability - 4);
        selected.pressedColor = Color.HSVToRGB(hue, saturation, visability - 22);
        selected.selectedColor = Color.HSVToRGB(hue, saturation, visability - 4);
        selected.colorMultiplier = 1;
    }

    void OnClick()
    {
        commandBar.Add(char.Parse(text.text));
        Uses--;
        UpdateButton();
    }

    public int Uses
    {
        get { return _uses; }
        set { _uses = value; UpdateButton(); }
    }
    public void SetUses(int total)
    {
        Uses = total;
        setLimit = true;
        UpdateButton();
    }
    public void SetUses(string total)
    {
        //print(name + " uses updated to " + total);
        try
        {
            SetUses(int.Parse(total));
        }
        catch
        {
            if(char.Parse(total.Substring(0, 1)) == 'i')
            {
                setLimit = false;
                UpdateButton();
            }
            else
            {
                Debug.LogError("[" + total + "] not a valid amount");
            }
        }
    }

    public void UpdateButton()
    {
        if(setLimit)
        {
            useText.text = Uses.ToString();
            if (Uses > 0) { Enable(true); }
            else { Enable(false); }
        }
        else
        {
            useText.text = ""; //"\u221E";
            Enable(true);
        }
    }
    void Enable(bool isEnabled)
    {
        if(isEnabled && !alreadyEnabled)
        {
            button.onClick.AddListener(() => OnClick());
            alreadyEnabled = true;
            useText.enabled = true;
        }
        else if(!isEnabled && alreadyEnabled)
        {
            alreadyEnabled = false;
            useText.enabled = false;
            button.onClick.RemoveAllListeners();
        }

        button.colors = isEnabled ? ColorBlock.defaultColorBlock : selected;
    }
}
