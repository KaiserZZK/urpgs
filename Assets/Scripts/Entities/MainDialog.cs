using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class MainDialog : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI nextLineHint;

    public Transform optionsPanel;
    public GameObject optionButtonPrefab;
    public Dialogue defaultDialogue;
    public Dialogue conditionalDialogue;
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
    // TODO: bool for character move

    void Start()
    {
        dialogueText.text = "";
        speakerText.text = "";
        currentDialogue = defaultDialogue; // Start with the default dialogue
    }

    // Update is called once per frame
    void Update()
    {
        
        // status (current objectives, known rules)-related
        if (Input.GetKeyDown(KeyCode.R))
        {
            // @zk to create actual UI later 
            Debug.Log("temp UI: =============");
            Debug.Log("current objectives: ##");
            Debug.Log("found rules: @@@@@@@@@");
        }
        
        // dialogue-related
        if (playerIsClose)
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
                    TriggerConversation();
                }
            }

            // press mouse on the object to trigger conversation;
            // use raycast to ensure mouse on object;
            // and make sure use mouse to continue and exit the conversation.
            // if (Input.GetMouseButtonDown(0))a
            // {
            //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //     RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, interactableLayer);

            //     if (hit.collider != null && hit.collider.gameObject == gameObject)
            //     {
            //         TriggerConversation();
            //     }
            //     else if (dialoguePanel.activeInHierarchy)
            //     {

            //         if (index < currentDialogue.lines.Length - 1)
            //         {
            //             TriggerConversation();
            //         }
            //         else
            //         {
            //             RemoveText();
            //         }

            //     }
            // }

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
            // case "Sink":
            //     if (GameManager.instance.interactedWithToaster)
            //     {
            //         currentDialogue = conditionalDialogue;
            //     }
            //     else
            //     {
            //         currentDialogue = defaultDialogue;
            //     }
            //     break;
            // case "Plant":
            //     if (GameManager.instance.interactedWithToaster && GameManager.instance.interactedWithPapers)
            //     {
            //         currentDialogue = conditionalDialogue;
            //     }
            //     else
            //     {
            //         currentDialogue = defaultDialogue;
            //     }
            //     break;
            case "Notebook":
                GameManager.instance.collectedNotebook = true;
                break;
            case "Terminal":
                if (GameManager.instance.collectedNotebook)
                {
                    currentDialogue = conditionalDialogue;
                }
                else
                {
                    currentDialogue = defaultDialogue;
                }

                break;
            case "Exit":
                // TODO @zk can add more meaningful conditions here for exit
                if (GameManager.instance.interactedWithToaster && GameManager.instance.interactedWithPapers)
                {
                    currentDialogue = conditionalDialogue;
                }
                else
                {
                    currentDialogue = defaultDialogue;
                }
                break;
            // Add more cases for other interaction conditions
            default:
                Debug.Log("default case triggerd");
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
            visualCue.SetActive(true);
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
            case "Toaster":
                GameManager.instance.interactedWithToaster = true;
                break;
            // Add more cases for other interactions
            case "Papers":
                GameManager.instance.interactedWithPapers = true;
                break;
        }
    }
}
