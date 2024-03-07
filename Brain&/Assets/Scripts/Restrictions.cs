using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class Restrictions : MonoBehaviour
{
    public static Restrictions instance;
    [SerializeField] Display display;

    [SerializeField] Transform selectionObj;
    Selection[] selection;
    [SerializeField] CommandBar commandBar;

    public static int level 
    { 
        get 
        { 
            return _level;
        } 
        set 
        {
            if (value != _level) { tutPos = 0; }
            _level = value; 
        } 
    }
    static int _level;
    public static int totalLevels;
    List<List<string>> levelInfo;

    [SerializeField] GameObject backButton, nextButton;
    TextMeshProUGUI tut, tutHeader;
    List<string> tutorialText;
    static int tutPos;

    string[] input, desiredOutput;
    List<string> currentOutput;
    int inputPos, trialPos;

    bool ignoreZeros;
    bool inputTaken = false;

    bool debugLog = false;

    private void Awake()
    {
        level = 1;
        instance = this;
        SetSelection();
        ReadFromFile();
    }
    void SetSelection()
    {
        selection = new Selection[selectionObj.childCount];
        for (int x = 0; x < selection.Length; x++)
        {
            selection[x] = selectionObj.GetChild(x).GetComponent<Selection>();
        }
    }

    void ReadFromFile()
    {
        var sr = Resources.Load<TextAsset>("LevelData");
        string[] lines = sr.text.Split("\n"[0]);

        levelInfo = new List<List<string>>();
        levelInfo.Add(new List<string>());
        int lev = 0;
        for(int x = 0; x < lines.Length; x++)
        {
            if (lines[x].Contains("@#$"))
            {
                lev++;
                levelInfo.Add(new List<string>());
                x++;
            }

            levelInfo[lev].Add(lines[x]);
        }
        levelInfo.RemoveAt(0);
        totalLevels = levelInfo.Count - 1;
        SetLevel();
    }

    public void SetLevel()
    {
        /*
         * Level Number
         * + - , . < > [ ] / \ & *
         * Max Number of Commands
         * Input
         * Expected Output, Ignore Zeros
         * Instructions
         * ...
         * [Tutorial]
         * ...
        */

        //Line 0 is Useless
        SetCommandLimits();
        SetInput();
        SetOutput();
        SetText();
    }
    void SetCommandLimits()
    {
        //Lines 1 - 2
        if (debugLog)
        {
            print("Commands: " + levelInfo[level][1]);
            print("Limiter: " + levelInfo[level][2]);
        }

        string[] cmd = levelInfo[level][1].Split(' ');
        for(int x = 0; x < cmd.Length; x++)
        {
            Selection s = selection[GetIndex(cmd[x].Substring(0, 1))];
            s.SetUses(cmd[x].Substring(1));
        }

        commandBar.SetCommandsLeft(levelInfo[level][2]);
    }
    void SetInput()
    {
        //Line 3
        if (debugLog) { print("Input: " + levelInfo[level][3]); }
        input = levelInfo[level][3].Split('=');
        inputPos = 0;
        trialPos = 0;
        inputTaken = false;
    }
    public int GetInput()
    {
        if(trialPos >= input.Length) { return int.MinValue; }

        if (debugLog) { print("Trial Pos: " + trialPos); }
        if(input[trialPos] != "null")
        {
            char.TryParse(input[trialPos].Substring(0, 1), out char r);
            if (r == 'r')
            {
                int rng = Random.Range(0, 10);
                DisplayMessage.instance.DisplayAMessage("Input: " + rng, true);
                return rng;
            }
            else if (input[trialPos].Contains("."))
            {
                string[] vals = input[trialPos].Split('.');
                if (inputPos >= vals.Length) { return int.MinValue; }
                DisplayMessage.instance.DisplayAMessage("Input: " + vals[inputPos], true);
                return int.Parse(vals[inputPos++]);
            }
            else
            {
                string val = input[trialPos];
                if(inputTaken) { return int.MinValue; }
                inputTaken = true;
                //print(val);
                DisplayMessage.instance.DisplayAMessage("Input: " + val, true);
                return int.Parse(val);
            }
        }
        return int.MinValue;
    }
    void SetOutput()
    {
        //Line 4
        ignoreZeros = bool.Parse(levelInfo[level][4].Substring(levelInfo[level][4].IndexOf(",") + 1));
        desiredOutput = levelInfo[level][4].Substring(0, levelInfo[level][4].IndexOf(",")).Split('=');
        if (debugLog)
        {
            print("Output: " + string.Join("=", desiredOutput));
            print("Ignore Zeros: " + ignoreZeros);
        }
        currentOutput = new List<string>();
    }
    public void StoreOutput(string output)
    {
        if(currentOutput.Count > 0 && trialPos < currentOutput.Count)
        {
            string val = currentOutput[trialPos];
            val += "." + output;
            currentOutput[trialPos] = val;
        }
        else
        {
            currentOutput.Add(output);
        }

        if(ignoreZeros) { CorrectOutput(); }

        display.Output(currentOutput[trialPos]);
        //print(currentOutput[trialPos]);
    }
    void CorrectOutput()
    {
        string[] vals = currentOutput[trialPos].Split('.');
        string newVal = "";

        print(vals.Length);
        bool zeroCorrect = true;
        for(int x = vals.Length - 3; x < vals.Length; x++)
        {
            if(x < 0) { break; }
            zeroCorrect = vals[x] == "0" && zeroCorrect;
        }
        print(zeroCorrect);

        for(int x = 0; x < vals.Length - 3; x++)
        {
            newVal += (newVal.Length == 0 ? "" : ".") + vals[x];
        }

        if(!zeroCorrect)
        {
            for (int x = vals.Length - 3; x < vals.Length; x++)
            {
                if(x < 0) { break; }
                if (vals[x] != "0")
                {
                    newVal += (newVal.Length == 0 ? "" : ".") + vals[x];
                }
            }
        }
        else
        {
            newVal += (newVal.Length == 0 ? "" : ".") + "0";
        }
        
        currentOutput[trialPos] = newVal;
    }
    public bool CheckOutput()
    {
        if(trialPos >= currentOutput.Count || currentOutput.Count == 0) { return false; }
        print("[" + currentOutput[trialPos] + "] [" + desiredOutput[trialPos] + "] " + trialPos);
        return currentOutput[trialPos] == desiredOutput[trialPos];
    }
    void SetText()
    {
        TextMeshProUGUI inst = GameObject.FindGameObjectWithTag("InstructionText").GetComponent<TextMeshProUGUI>();
        tut = GameObject.FindGameObjectWithTag("TutorialText").GetComponent<TextMeshProUGUI>();
        tutHeader = GameObject.FindGameObjectWithTag("TutorialHeader").GetComponent<TextMeshProUGUI>();

        inst.text = "";
        tut.text = "";
        tutorialText = new List<string>();

        //Line 5+
        for(int x = 5; x < levelInfo[level].Count; x++)
        {
            if (levelInfo[level][x].Substring(0, 1) == "!")
            {
                inst.text = levelInfo[level][x].Substring(1);
            }
            else
            {
                if(levelInfo[level][x].Contains("---"))
                {
                    if(!levelInfo[level][x].Contains("@")) { tutorialText.Add(levelInfo[level][x] + "\n"); }
                    else { tutorialText.Add("\n"); }
                }
                else
                {
                    if(tutorialText.Count == 0) { Debug.LogError("No lines to edit"); }
                    if (levelInfo[level][x].Contains("\\n"))
                    {
                        tutorialText[tutorialText.Count - 1] += levelInfo[level][x].Substring(0, levelInfo[level][x].IndexOf("\\n")) + " ";
                        tutorialText[tutorialText.Count - 1] += "\n";
                    }
                    else
                    {
                        tutorialText[tutorialText.Count - 1] += levelInfo[level][x].Substring(0, levelInfo[level][x].Length - 1) + " ";
                    }
                }
            }
        }

        SetupTutorial();
    }
    #region Tutorial Text
    void SetupTutorial()
    {
        if (tutorialText.Count == 0) { Debug.LogError("No tutorial"); }

        tutHeader.text = tutorialText[tutPos].Substring(0, tutorialText[tutPos].IndexOf("\n"));
        tut.text = tutorialText[tutPos].Substring(tutorialText[tutPos].IndexOf("\n") + 1);
        if (tutPos > 0) { backButton.SetActive(true); }
        else { backButton.SetActive(false); }
        if (tutPos < tutorialText.Count - 1) { nextButton.SetActive(true); }
        else { nextButton.SetActive(false); }
    }
    public void BackText()
    {
        tutPos -= 1;
        if(tutPos == 0) { backButton.SetActive(false); }
        if(tutPos < tutorialText.Count - 1) { nextButton.SetActive(true); }

        tutHeader.text = tutorialText[tutPos].Substring(0, tutorialText[tutPos].IndexOf("\n"));
        tut.text = tutorialText[tutPos].Substring(tutorialText[tutPos].IndexOf("\n") + 1);
    }
    public void NextText()
    {
        tutPos += 1;
        if (tutPos > 0) { backButton.SetActive(true); }
        if (tutPos == tutorialText.Count - 1) { nextButton.SetActive(false); }

        tutHeader.text = tutorialText[tutPos].Substring(0, tutorialText[tutPos].IndexOf("\n"));
        tut.text = tutorialText[tutPos].Substring(tutorialText[tutPos].IndexOf("\n") + 1);
    }
    #endregion

    public void TrialOver()
    {
        if (CheckOutput())
        {
            if (trialPos < input.Length - 1)
            {
                inputPos = 0;
                trialPos++;
                inputTaken = false;
                //print("Next Trial");
                DisplayMessage.instance.DisplayAMessage("Starting Next Trial", true);
            }
            else
            {
                //tell player of success
                DisplayMessage.instance.DisplayAMessage("Level Complete", true);
                PlayerPrefs.SetString(level.ToString(), "True");
                PlayerPrefs.Save();
                SoftReset();
                return;
            }

            Interpreter.running = false;
            commandBar.Compile();
        }
        else
        {
            if(currentOutput.Count > 0)
            {
                DisplayMessage.instance.DisplayAMessage("Incorrect Output", false);
            }
            else
            {
                DisplayMessage.instance.DisplayAMessage("No Output", false);
            }
            SoftReset();
            //Tell player of failure
        }
    }

    #region Playtime
    public void Deleted(int index)
    {
        selection[index].Uses++;
    }

    public static int GetIndex(Button b)
    {
        TextMeshProUGUI text = b.GetComponentInChildren<TextMeshProUGUI>();
        return GetIndex(text.text);
    }
    public static int GetIndex(string text)
    {
        switch ((int)char.Parse(text))
        {
            case 43: // +
                return 0;
            case 45: // -
                return 1;
            case 44: // ,
                return 2;
            case 46: // .
                return 3;
            case 60: // <
                return 4;
            case 62: // >
                return 5;
            case 91: // [
                return 6;
            case 93: // ]
                return 7;
            case 47: // /
                return 8;
            case 92: // \
                return 9;
            case 38: // &
                return 10;
            case 42: // *
                return 11;
        }

        return -1;
    }

    public void ResetRestrictions()
    {
        //print("Resrtictions Reset");
        Interpreter.running = false;
        SetLevel();
    }
    public void SoftReset()
    {
        Interpreter.running = false;
        SetInput();
        SetOutput();
    }
    #endregion
}
