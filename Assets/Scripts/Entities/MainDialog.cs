using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class MainDialog : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI nextLineHint;

    public Transform optionsPanel;
    public GameObject optionButtonPrefab;
    public Dialogue defaultDialogue;
    public Dialogue[] conditionalDialogues;
    private Dialogue currentDialogue;
    private int index = 0;
    [SerializeField] private GameObject visualCue; // set up for the visual cue of interaction

    public float wordSpeed;
    public float fastWordSpeed;
    public bool playerIsClose;
    public LayerMask interactableLayer; // Specify the layer mask for interactable objects
    private Coroutine typingCoroutine;
    private bool isFastTyping = false; // for fast typing
    private bool optionsDisplayed = false; // flag to check if options are displayed

    public string interactionCondition; // specify condition
    public Image dialogueCgCanvas;
    public Sprite dialogueCgSpirte;

    private bool isCgScene ;
    private bool firstDialoguePlayed = false;

    public GameObject notebookInteractable;
    // TODO: bool for character move
    
    void Start()
    {
        dialogueText.text = "";
        speakerText.text = "";
        currentDialogue = defaultDialogue; // Start with the default dialogue
        isCgScene = SceneController.sceneInstance.cgScenes.Contains(SceneManager.GetActiveScene().name);
        
        if (isCgScene)
        {
            firstDialoguePlayed = true; // Skip the first manual E press
            TriggerConversation(); // Autoplay first dialogue line
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        // dialogue-related
        if (isCgScene || playerIsClose)
        {
            // press E to trigger, continue and exit conversation
            if (Input.GetKeyDown(KeyCode.E))
            {
                nextLineHint.gameObject.SetActive(true);
                if (optionsDisplayed)
                {
                    optionsDisplayed = false;
                    optionsPanel.gameObject.SetActive(false);
                    NextLine();
                }
                else
                {
                    if (isCgScene)
                    {
                        if (!firstDialoguePlayed)
                        {
                            firstDialoguePlayed = true; // Set flag so first press is skipped
                            TriggerConversation();
                        }
                        else
                        {
                            NextLine();
                        }
                    }
                    else
                    {
                        TriggerConversation();
                    }
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Q) && dialoguePanel.activeInHierarchy)
            {
                RemoveText();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                isFastTyping = true;
            }

        }
        
    }

    void TriggerConversation()
    {
        if (!dialoguePanel.activeInHierarchy)
        {
            CheckInteractionCondition();
            dialoguePanel.SetActive(true);
            
            // TODO @zk BONUS: make index-specific (CG only showing during specific sentences in dialogue)
            // Show CG if available
            if (dialogueCgCanvas != null && dialogueCgSpirte != null)
            {
                dialogueCgCanvas.gameObject.SetActive(true);
                dialogueCgCanvas.sprite = dialogueCgSpirte;
            
                // Set alpha to fully visible
                Color cgColor = dialogueCgCanvas.color;
                cgColor.a = 1f; 
                dialogueCgCanvas.color = cgColor;
            }
            
            typingCoroutine = StartCoroutine(Typing(currentDialogue.lines.Length > 1));
        }
        else if (dialogueText.text == currentDialogue.lines[index].content)
        {
            NextLine();
        }
    }

    // check for interaction condition function
    void CheckInteractionCondition()
    {
        switch (interactionCondition)
        {
            case "Notebook":
                MarkAsInteracted();
                break;
            case "Terminal":
                if (GameManager.instance.checkedTerminal)
                {
                    currentDialogue = conditionalDialogues[1];
                }
                else if (GameManager.instance.collectedNotebook)
                {
                    currentDialogue = conditionalDialogues[0];
                    MarkAsInteracted();
                }
                else
                {
                    currentDialogue = defaultDialogue;
                }
                break;
            case "Exit":
                if (GameManager.instance.checkedTerminal)
                {
                    // TODO @zk could add a known vs. unknown state as to where the door leads to
                    if (true)
                    {
                        currentDialogue = conditionalDialogues[0]; // Destination unknown
                    }
                }
                else
                {
                    currentDialogue = defaultDialogue;
                }
                break;
            // Add more cases for other interaction conditions
            default:
                currentDialogue = defaultDialogue;
                break;

        }
    }

    public void RemoveText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        dialogueText.text = "";
        speakerText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        optionsPanel.gameObject.SetActive(false); // Deactivate options panel
        isFastTyping = false;
        
        // Hide CG when dialogue is closed
        if (dialogueCgCanvas != null)
        {
            dialogueCgCanvas.gameObject.SetActive(false);
            // Set alpha to fully transparent
            Color cgColor = dialogueCgCanvas.color;
            cgColor.a = 0f;
            dialogueCgCanvas.color = cgColor;
        }
        
        if (GameManager.instance.collectedNotebook)
        {
            notebookInteractable.gameObject.SetActive(false);
            
        }
        
    }

    IEnumerator Typing(bool hasNextLine)
    {
        if (!hasNextLine)
        {
            nextLineHint.gameObject.SetActive(false);
        }
        
        dialogueText.text = "";
        speakerText.text = currentDialogue.lines[index].speaker;
        foreach (char letter in currentDialogue.lines[index].content.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(isFastTyping ? fastWordSpeed : wordSpeed);
        }
        isFastTyping = false;
        

        // if there's options, show options here:
        if (currentDialogue.lines[index].hasOptions) {
            DisplayOptions(currentDialogue.lines[index].options);
            optionsDisplayed = true;
        }
        MarkAsInteracted();
    }

    void DisplayOptions(DialogueOption[] options)
    {
        // Clear previous options
        foreach (Transform child in optionsPanel)
        {
            Destroy(child.gameObject);
        }

        optionsPanel.gameObject.SetActive(true);
        // Add new options
        foreach (DialogueOption option in options)
        {
            GameObject button = Instantiate(optionButtonPrefab, optionsPanel);
            button.GetComponentInChildren<TextMeshProUGUI>().text = option.optionText;

            DialogueOption localOption = option;
            button.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnOptionSelected(localOption)));
            // button.GetComponent<Button>().onClick.AddListener(() => OnOptionSelected(option));
            button.SetActive(true);
        }
    }

    IEnumerator OnOptionSelected(DialogueOption option)
    {
        dialogueText.text = "";
        speakerText.text = option.speaker;
        foreach (char letter in option.dialogueLines.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(isFastTyping ? fastWordSpeed : wordSpeed);
        }
        isFastTyping = false;
        
        // option-handling logics
        if (option.shouldChangeScene != "")
        {
            // TODO @zk fix scene transition effect
            SceneController.sceneInstance.GoSpecifiedScene(option.shouldChangeScene);
        }
        
        // defaulting behavior to do nothing & show next line 
        NextLine();
    }

    public void NextLine()
    {
        if (index < currentDialogue.lines.Length - 1)
        {
            bool hasNextLine = (index < currentDialogue.lines.Length - 2);
            index++;
            dialogueText.text = "";
            speakerText.text = "";
            optionsPanel.gameObject.SetActive(false);
            typingCoroutine = StartCoroutine(Typing(hasNextLine));
        }
        else
        {
            RemoveText();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isCgScene)
            {
                visualCue.SetActive(true);
            }
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            visualCue.SetActive(false);
            playerIsClose = false;
            RemoveText();
        }
    }

    public void MarkAsInteracted()
    {
        switch (interactionCondition)
        {
            case "Notebook":
                GameManager.instance.collectedNotebook = true;
                break;
            case "Terminal":
                GameManager.instance.checkedTerminal = true;
                break;
        }
    }
}
