using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public TMP_InputField terminalInput;
    public GameObject userInputLine;

    public ScrollRect sr;
    public GameObject msgList;

    Interpreter interpreter;

    private void Start()
    {
        interpreter = GetComponent<Interpreter>();
    }

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            // Store user input
            string userInput = terminalInput.text;
            // Clear input field
            ClearInputField();
            // Instantiate game object with directory prefix
            AddDirectoryLine(userInput);

            // Add the interpretation lines
            int lines = AddInterpreterLine(interpreter.Interpret(userInput));

            // Scroll to the bottom of ScrollRect
            ScrollToBottom(lines);

            // Move user line to the end
            userInputLine.transform.SetAsLastSibling();
            // Refocus input field
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    private void ClearInputField()
    {
        terminalInput.text = "";
    }

    public void AddDirectoryLine(string userInput)
    {
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35.0f);

        GameObject msg = Instantiate(directoryLine, msgList.transform);

        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

        msg.GetComponentsInChildren<Text>()[1].text = userInput;
    }

    public int AddInterpreterLine(List<string> interpretation)
    {
        for (int i = 0; i < interpretation.Count; i++)
        {
            // Instantiate the reponse line
            GameObject res = Instantiate(responseLine, msgList.transform);

            // Set it to the end of all messages 
            res.transform.SetAsLastSibling();

            // Get the size of message list, then resize 
            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

            // Set the text of this response line to be whatever the interpreter string is
            res.GetComponentInChildren<Text>().text = interpretation[i];
            Debug.Log("Adding response: " + interpretation[i]);
        }

        return interpretation.Count;
    }

    void ScrollToBottom(int lines)
    {
        if (lines > 4)
        {
            sr.velocity = new Vector2(0, 450);
        }
        else
        {
            sr.verticalNormalizedPosition = 0;
        }
    }

}
