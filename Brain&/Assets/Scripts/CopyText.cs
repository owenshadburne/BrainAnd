using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CopyText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI original;
    TextMeshProUGUI thisText;

    private void Start()
    {
        thisText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        thisText.text = original.text;
    }
}
