using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialFormatter : MonoBehaviour
{
    public static DialFormatter instance;
    [SerializeField] Transform[] tiles;
    TextMeshProUGUI[] values, references, pointers;

    private void Awake()
    {
        instance = this;

        values = new TextMeshProUGUI[tiles.Length];
        references = new TextMeshProUGUI[tiles.Length];
        pointers = new TextMeshProUGUI[tiles.Length];
        for (int x = 0; x < tiles.Length; x++) 
        {
            values[x] = tiles[x].GetChild(0).GetComponent<TextMeshProUGUI>();
            references[x] = tiles[x].GetChild(1).GetComponent<TextMeshProUGUI>();
            pointers[x] = tiles[x].GetChild(2).GetComponent<TextMeshProUGUI>();
        }
    }

    public void UpdateDisplay(string info)
    {
        string[] seperate = info.Split(' ');

        for(int x = 0; x < values.Length; x++)
        {
            if(seperate[x].Contains("&"))
            {
                references[x].text = "&";
                seperate[x] = seperate[x].Substring(1);
            }
            else
            {
                references[x].text = "";
            }

            if(seperate[x].Contains("*"))
            {
                pointers[x].text = "*";
                seperate[x] = seperate[x].Substring(1);
            }
            else
            {
                pointers[x].text = "";
            }

            values[x].text = seperate[x];
        }
    }

}
