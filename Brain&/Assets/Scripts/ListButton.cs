using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListButton : MonoBehaviour
{
    private void Start()
    {
        Button b = GetComponent<Button>();
        b.onClick.AddListener(() => ListOnClick());
    }

    void ListOnClick()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (text.text == "Playground") { Restrictions.level = 0; }
        else { Restrictions.level = int.Parse(text.text); }
        LevelSelection.instance.Refresh();
        //print("Level changed to " + Restrictions.level);
    }
}
