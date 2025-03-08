using System;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]  
public class RulesBySource
{
    public string sourceName;  
    public List<string> rules;  
}

public class GameManager : MonoBehaviour
{
    // Global GameManager singleton accessible using GameManager.instance
    public static GameManager instance;

    // Input
    public Controls controls;

    // Current dungeon (null if no dungeon is active)
    public Dungeon dungeon;

    // Sequential progression
    public bool collectedNotebook = false;
    public bool checkedTerminal = false;

    public bool isFullScreenCgActive = false;
    public GameObject activeFullScreenCgCanvas;
    
    // Rules
    [SerializeField] private GameObject notebookUI;
    public GameObject notebookNotification;
    [SerializeField] private List<RulesBySource> rulesBySources = new List<RulesBySource>();
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (notebookUI != null)
        {
            DontDestroyOnLoad(notebookUI); // Persist the notebook UI separately
        }
    }

    void Start()
    {
        controls = new Controls();
        controls.Enable();

        // StartGame();
    }

    public void StartGame()
    {
        dungeon = Instantiate(Resources.Load("Prefabs/Dungeon")).GetComponent<Dungeon>();
        dungeon.name = "Dungeon";
    }

    public void UncoverNotebookTabName(string source)
    {
        Transform textTransform;
        switch (source)
        {
            case "Institute":
                textTransform = notebookUI.transform.Find("Menu/Tabs/Institute/InstituteTabText");
                if (textTransform != null)
                {
                    TMP_Text tmpText = textTransform.GetComponent<TMP_Text>();
                    if (tmpText != null && tmpText.text == "???")
                    {
                        tmpText.text = "Institute";
                    }
                }
                break;
            case "EIDOS":
                textTransform = notebookUI.transform.Find("Menu/Tabs/EIDOS/EIDOSTabText");
                if (textTransform != null)
                {
                    TMP_Text tmpText = textTransform.GetComponent<TMP_Text>();
                    if (tmpText != null && tmpText.text == "???")
                    {
                        tmpText.text = "EIDOS";
                    }
                }
                break;
            case "Self":
                textTransform = notebookUI.transform.Find("Menu/Tabs/Self/SelfTabText");
                if (textTransform != null)
                {
                    TMP_Text tmpText = textTransform.GetComponent<TMP_Text>();
                    if (tmpText != null && tmpText.text == "???")
                    {
                        tmpText.text = "Self";
                    }
                }
                break;
        }
    }

    public void AddRuleToNotebook(string source, int startIndex, int endIndex)
    {
        Transform textTransform;
        switch (source)
        {
            case "Institute":
                textTransform = notebookUI.transform.Find("Menu/Pages/InstitutePage/InstituteTextMask/InstituteText");

                if (textTransform != null)
                {
                    Text legacyText = textTransform.GetComponent<Text>(); // Using legacy Text component
                    if (legacyText != null)
                    {
                        // If text is empty, set the initial sentence
                        if (string.IsNullOrWhiteSpace(legacyText.text))
                        {
                            legacyText.text = "A list of rules gathered from notifications by the Institute...\n\n";
                        }

                        // Find rules from the dictionary
                        List<string> rules = rulesBySources[0].rules;

                        // Add the rules to the legacy text component
                        for (int i = startIndex; i < endIndex; i++)
                        {
                            legacyText.text += rules[i] + "\n\n";
                        }
                    }
                }
                break;
        }
    }
}
