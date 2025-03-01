[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string content;
    public bool hasOptions;
    public DialogueOption[] options;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public string speaker;
    public string dialogueLines;
}

[System.Serializable]
public class Dialogue
{
    public DialogueLine[] lines;
}

// add another class specifically for dialogue lines in option field