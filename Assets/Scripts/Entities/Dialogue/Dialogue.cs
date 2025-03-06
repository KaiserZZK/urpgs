using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string content;
    public bool hasOptions;
    public int cgIndex;
    public bool markAtThisLine;
    public DialogueOption[] options;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public string speaker;
    public string dialogueLines;
    public string shouldChangeScene;
    // TODO @zk implement option-nested dialogue (use cases e.g. terminal CG)
    // public bool shouldDisplayFullScreenCg;
}

[System.Serializable]
public class Dialogue
{
    public DialogueLine[] lines;
}

// add another class specifically for dialogue lines in option field