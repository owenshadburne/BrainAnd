using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    [SerializeField] CommandBar commandBar;
    Display display;
    const int loopMax = 255;

    bool debugMessages = false;
    bool abort;
    public static bool running;

    float defaultDelayTime = .5f, delayTime;
    Stack<int> loopPos;

    void Start()
    {
        display = GetComponent<Display>();
        UpdateDisplay();
    }

    public void CompileCode(char[] code)
    {
        delayTime = defaultDelayTime;
        display.ActivateSelector();
        StartCoroutine(StaggeredCode(code));
    }
    IEnumerator StaggeredCode(char[] code)
    {
        Memory inst = Memory.instance;

        loopPos = new Stack<int>();
        int currentLoops = 0;
        abort = false;
        for (int x = 0; x < code.Length; x++)
        {
            //print("Code Segment: " + x);
            if (currentLoops > loopMax)
            {
                Debug.LogWarning("Maximum Amount of Loops Exceeded");
                DisplayMessage.instance.DisplayAMessage("Maximum Amount of Loops Exceeded", false);
                abort = true;
            }
            if (abort)
            {
                break;
            }

            if (code[x] == 93 && loopPos.Count > 0)
            {
                if (inst.CanEndLoop())
                {
                    loopPos.Pop();
                    if (debugMessages) { print("Loop End"); }
                }
                else
                {
                    x = loopPos.Peek();
                    currentLoops++;
                    if (debugMessages) { print("Resart Loop"); }
                }
            }
            
            if (code[x] == 91)
            {
                if(inst.CanEndLoop())
                {
                    int temp = GetNextEndBracePos(code, x);
                    if(temp > -1)
                    {
                        x = temp;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    loopPos.Push(x + 1);
                    currentLoops = 0;
                    if (debugMessages) { print("Loop Start"); }
                }
            }
            else
            {
                Command(code[x], inst);
            }

            display.CenterCommand(commandBar.GetButton(x));
            yield return new WaitForSeconds(delayTime);
        }

        if (!abort) { Restrictions.instance.TrialOver(); }
        else { abort = false; Restrictions.instance.SoftReset(); }
        display.CenterCommand(null);
    }
    int GetNextEndBracePos(char[] code, int startBracePos)
    {
        for(int x = startBracePos; x < code.Length; x++)
        {
            if(code[x] == 93)
            {
                return x;
            }
        }

        return -1;
    }

    public void Command(char command, Memory inst)
    {
        switch((int)command)
        {
            case 43: // +
                inst.Increment();
                if (debugMessages) { print(inst.GetPrintOrder()); }
                break;
            case 45: // -
                inst.Decrement();
                if (debugMessages) { print(inst.GetPrintOrder()); }
                break;
            case 60: // <
                inst.MoveLeft();
                display.MoveSelector(inst.CurrentRingIndex(), delayTime);
                if (debugMessages) { print("Move Left"); }
                break;
            case 62: // >
                inst.MoveRight();
                display.MoveSelector(inst.CurrentRingIndex(), delayTime);
                if (debugMessages) { print("Move Right"); }
                break;
            case 44: // ,
                int temp = Restrictions.instance.GetInput();
                if(temp > int.MinValue) { inst.GetInput(temp); }
                break;
            case 46: // .
                Restrictions.instance.StoreOutput(inst.Output());
                break;
            case 38: // &
                inst.Reference();
                if (debugMessages) { print(inst.GetPrintOrder()); }
                break;
            case 42: // *
                inst.Pointer();
                if (debugMessages) { print(inst.GetPrintOrder()); }
                break;
            case 47: // /
                inst.MoveUp();
                display.SpinCircle(inst.CurrentRingIndex(), true, delayTime);
                if (debugMessages) { print("Move Up"); }
                break;
            case 92: // \
                inst.MoveDown();
                display.SpinCircle(inst.CurrentRingIndex(), false, delayTime);
                if (debugMessages) { print("Move Down"); }
                break;
        }

        UpdateDisplay();
    }

    public void Abort()
    {
        abort = true;
    }
    public void SpeedUp()
    {
        if(delayTime > .0001f)
        {
            delayTime /= 2;
            if(delayTime == 0)
            {
                delayTime = .0001f;
            }
        }
    }

    public void UpdateDisplay()
    {
        DialFormatter.instance.UpdateDisplay(Memory.instance.GetPrintOrderRaw());
    }
}
