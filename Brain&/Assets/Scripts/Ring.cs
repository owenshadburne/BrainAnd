using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring
{
    sbyte[] values;
    int position;

    int referencePos = -1;
    int pointerStartPos = -1, pointerEndPos = -1;

    public Ring(int allignment)
    {
        switch (allignment)
        {
            case 0:
                values = new sbyte[2];
                break;
            case 1:
                values = new sbyte[3];
                break;
            case 2:
                values = new sbyte[4];
                break;
        }
        position = 0;
    }

    public void Increment()
    {
        values[position]++;
    }
    public void Decrement()
    {
        values[position]--;
    }
    public void BeginReference()
    {
        referencePos = position;
    }
    public sbyte EndReference()
    {
        int temp = referencePos;
        referencePos = -1;
        return values[temp];
    }
    public void BeginPointer()
    {
        pointerStartPos = position;
    }
    public bool SetPointer()
    {
        if(pointerStartPos == position)
        {
            return false;
        }

        pointerEndPos = position;
        return true;
    }
    public void IncrementPointer()
    {
        if(pointerStartPos > -1 && (!Memory.instance.isCurrentRing(this) || position != pointerStartPos))
        {
            values[pointerStartPos]++;
        }
        else if(pointerEndPos > -1 && (!Memory.instance.isCurrentRing(this) || position != pointerEndPos))
        {
            values[pointerEndPos]++;
        }
    }
    public void DecrementPointer()
    {
        if (pointerStartPos > -1 && (!Memory.instance.isCurrentRing(this) || position != pointerStartPos))
        {
            values[pointerStartPos]--;
        }
        else if (pointerEndPos > -1 && (!Memory.instance.isCurrentRing(this) || position != pointerEndPos))
        {
            values[pointerEndPos]--;
        }
    }
    public void EndPointer()
    {
        pointerStartPos = -1;
        pointerEndPos = -1;
    }

    public bool isPointerStart() { return position == pointerStartPos; }
    public bool isPointerEnd() { return position == pointerEndPos; }

    public void MoveUp()
    {
        position--;
        if (position < 0)
        {
            position = values.Length - 1;
        }
    }

    public void MoveDown()
    {
        position++;
        if (position > values.Length - 1)
        {
            position = 0;
        }
    }

    public void SetValue(sbyte val)
    {
        values[position] = val;
    }

    public sbyte GetValue()
    {
        return values[position];
    }

    public string GetPrint()
    {
        string result = "[";
        
        for(int x = 0; x < values.Length; x++)
        {
            result += x == referencePos ? "&" : "";
            result += (x == pointerStartPos || x == pointerEndPos) ? "*" : "";
            result += values[x] + (x < values.Length - 1 ? ", " : "]");
        }

        return result;
    }

    public string GetRaw()
    {
        string result = "";
        for (int x = 0; x < values.Length; x++)
        {
            result += x == referencePos ? "&" : "";
            result += (x == pointerStartPos || x == pointerEndPos) ? "*" : "";
            //result += x == position ? ">" : "";
            result += values[x] + (x < values.Length - 1 ? " " : "");
        }
        return result;
    }
}
