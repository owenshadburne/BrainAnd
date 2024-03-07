using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteForcePlayerPrefs : MonoBehaviour
{
    private void Awake()
    {
        if(PlayerPrefs.GetString("FirstBoot") == "True")
        {
            //nothing
        }
        else
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("FirstBoot", "True");
            PlayerPrefs.Save();
        }
    }
}
