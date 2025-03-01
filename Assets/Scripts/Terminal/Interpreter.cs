using Scripts.BuildTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    List<string> response = new List<string>();
    private BuildTreeManager _playerBuildManager;

    private void Awake()
    {
        _playerBuildManager = FindObjectOfType<BuildTreeManager>();
    }

    public List<string> Interpret(string userInput)
    {
        response.Clear();

        string[] args = userInput.Split();

        if (args[0] == "help")
        {
            response.Add("If you want to use the terminal, type \"boop\" ");

            return response; 
        }
        if (userInput == "Open the pod bay door, Hal")
        {
            response.Add("...");
            return response;
        }
        if (userInput == "Hal do you read me?")
        {
            response.Add("......");
            return response;
        }
        if (userInput.StartsWith("Component"))
        {
            // Respond with the detailed component and slot information
            response.Add(userInput);
            return response;
        }
        if (args[0] == "*****update")
        {
            if (_playerBuildManager.Strength >= 40)
            {
                Debug.Log("Impressive!!");
                response.Add("Impressive!");
                return response;
            }
            response.Add("Component dropped into the slot!");
            // Debug.Log("Response: " + response[0]);
            return response;
        }
        else
        {
            response.Add("Command not recognized. Type help for a list of available commands.");
            return response;
        }
    }

}
