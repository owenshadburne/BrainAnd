using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    public static Memory instance;
    Ring[] rings;
    int currentRing;

    Ring referenceStart;
    Ring pointerStart, pointerEnd;

    Display display;

    void Awake()
    {
        instance = this;
        display = GetComponent<Display>();
        Reset();
    }

    public void Increment()
    {
        rings[currentRing].Increment();

        if(pointerStart != null && pointerEnd != null)
        {
            if(rings[currentRing].isPointerStart())
            {
                pointerEnd.IncrementPointer();
            }
            else if(rings[currentRing].isPointerEnd())
            {
                pointerStart.IncrementPointer();
            }
        }
    }
    public void Decrement()
    {
        rings[currentRing].Decrement();

        if (pointerStart != null && pointerEnd != null)
        {
            if (rings[currentRing].isPointerStart())
            {
                pointerEnd.DecrementPointer();
            }
            else if (rings[currentRing].isPointerEnd())
            {
                pointerStart.DecrementPointer();
            }
        }
    }
    public void MoveLeft()
    {
        currentRing--;
        if(currentRing < 0)
        {
            currentRing = rings.Length - 1;
        }
    }
    public void MoveRight()
    {
        currentRing++;
        if(currentRing > rings.Length - 1)
        {
            currentRing = 0;
        }
    }
    public void GetInput(int val)
    {
        rings[currentRing].SetValue((sbyte)val);
    }
    public string Output()
    {
        string result = "";

        foreach(Ring r in rings)
        {
            result += r.GetValue() + ".";
        }

        return result.Substring(0, result.Length - 1);
    }
    public void Reference()
    {
        if(referenceStart == null)
        {
            rings[currentRing].BeginReference();
            referenceStart = rings[currentRing];
        }
        else
        {
            rings[currentRing].SetValue(referenceStart.EndReference());
            referenceStart = null;
        }
    }
    public void Pointer()
    {
        if(pointerStart == null)
        {
            rings[currentRing].BeginPointer();
            pointerStart = rings[currentRing];
        }
        else if(pointerEnd == null)
        {
            if(rings[currentRing].SetPointer())
            {
                pointerEnd = rings[currentRing];
            }
        }
        else
        {
            pointerStart.EndPointer();
            pointerEnd.EndPointer();
            pointerStart = null;
            pointerEnd = null;
            Pointer();
        }
    }
    public void MoveUp()
    {
        rings[currentRing].MoveUp();
    }
    public void MoveDown()
    {
        rings[currentRing].MoveDown();
    }
    public int CurrentRingIndex()
    {
        return currentRing;
    }
    public bool isCurrentRing(Ring r)
    {
        return r == rings[currentRing];
    }

    public bool CanEndLoop()
    {
        return rings[currentRing].GetValue() == 0;
    }

    public string GetPrintOrder()
    {
        string result = "";

        foreach(Ring r in rings)
        {
            result += r.GetPrint() + " ";
        }

        return result;
    }
    public string GetPrintOrderRaw()
    {
        string result = "";

        foreach (Ring r in rings)
        {
            result += r.GetRaw() + " ";
        }

        return result;
    }

    public void Reset()
    {
        rings = new Ring[] { new Ring(0), new Ring(1), new Ring(2) };
        currentRing = 0;

        referenceStart = null;
        pointerStart = null;
        pointerEnd = null;

        display.Reset();
    }
}
