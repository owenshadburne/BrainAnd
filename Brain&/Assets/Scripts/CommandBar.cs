using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandBar : MonoBehaviour
{
    Interpreter interpreter;

    [SerializeField] Button template;
    [SerializeField] ScrollRect scrollrect;
    List<Button> buttons;
    int position;
    Transform content;

    ColorBlock selected = new ColorBlock();
    int hue = 0, saturation = 100, visability = 100;

    [SerializeField] TextMeshProUGUI commandsLeftText;
    int commandsLeft;
    bool infCommands;

    void Start()
    {
        interpreter = GameObject.FindGameObjectWithTag("Interpreter").GetComponent<Interpreter>();

        buttons = new List<Button>();
        position = 0;
        content = GameObject.FindGameObjectWithTag("Content").transform;

        selected.normalColor = Color.HSVToRGB(hue, saturation, visability);
        selected.highlightedColor = Color.HSVToRGB(hue, saturation, visability - 4);
        selected.pressedColor = Color.HSVToRGB(hue, saturation, visability - 22);
        selected.selectedColor = Color.HSVToRGB(hue, saturation, visability - 4);
        selected.colorMultiplier = 1;
    }

    public void Add(char command)
    {
        if(position >= buttons.Count && (commandsLeft > 0 || infCommands))
        {
            Button temp = Instantiate(template, content);
            TextMeshProUGUI text = temp.GetComponentInChildren<TextMeshProUGUI>();
            buttons.Add(temp);

            int index = buttons.Count - 1;
            temp.onClick.AddListener(() => Select(index));
            
            text.text = "" + command;
            commandsLeft--;

            Canvas.ForceUpdateCanvases();
            scrollrect.horizontalNormalizedPosition = 1;
        }
        else if(position >= 0 && position < buttons.Count)
        {
            Edit(command);
        }

        ChangePosition(position + 1);
    }
    void Edit(char command)
    {
        Button temp = buttons[position];
        Restrictions.instance.Deleted(Restrictions.GetIndex(temp));

        TextMeshProUGUI text = temp.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "" + command;

        scrollrect.content.localPosition = Display.GetSnapToPositionToBringChildIntoView(scrollrect, temp.GetComponent<RectTransform>());
    }

    public void Select(int index)
    {
        //print(index);
        if(index == position)
        {
            ChangePosition(buttons.Count);
        }
        else
        {
            ChangePosition(index);
        }
    }

    void ChangePosition(int newPos)
    {
        if (position < buttons.Count) { buttons[position].colors = ColorBlock.defaultColorBlock; }
        if (newPos < buttons.Count) { buttons[newPos].colors = selected; }
        position = newPos;
    }

    public void Compile()
    {
        ChangePosition(buttons.Count);
        if(!Interpreter.running)
        {
            Interpreter.running = true;
            Memory.instance.Reset();

            char[] commands = new char[buttons.Count];

            int pos = 0;
            foreach (Button button in buttons)
            {
                commands[pos++] = char.Parse(button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            }

            interpreter.CompileCode(commands);
        }
    }

    public void Reset()
    {
        interpreter.Abort();

        foreach(Button b in buttons)
        {
            Destroy(b.gameObject);
        }

        buttons = new List<Button>();
        Memory.instance.Reset();
        interpreter.UpdateDisplay();
        Restrictions.instance.ResetRestrictions();
    }

    public void Delete()
    {
        if(!Interpreter.running)
        {
            if (position < 0 || position >= buttons.Count) { return; }
            Button b = buttons[position];
            Restrictions.instance.Deleted(Restrictions.GetIndex(b));
            Destroy(b.gameObject);
            buttons.RemoveAt(position);

            ChangePosition(position);
            UpdateIndex();
            commandsLeft++;
        }
    }
    void UpdateIndex()
    {
        for(int x = 0; x < buttons.Count; x++)
        {
            buttons[x].onClick.RemoveAllListeners();
            int index = x;
            buttons[x].onClick.AddListener(() => Select(index));
        }
    }

    public Button GetButton(int index)
    {
        return buttons[index];
    }

    public void SetCommandsLeft(string val)
    {
        try
        {
            commandsLeft = int.Parse(val);
            infCommands = false;
        }
        catch
        {
            infCommands = true;
        }
    }

    private void Update()
    {
        if (infCommands) { commandsLeftText.text = ""; }
        else { commandsLeftText.text = "Commands Left: " + commandsLeft; }
    }
}
