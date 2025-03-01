using Scripts.BuildTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EmptySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Canvas canvas;
    private BuildTreeManager _playerBuildManager;
    private Interpreter _interpreter;
    private Image targetImage;

    // Static list to track all slots and their contents
    private static List<EmptySlot> allSlots = new List<EmptySlot>();

    // Reference to the DragDrop component currently in this slot
    private DragDrop currentDragDropComponent;


    private void Awake()
    {
        _playerBuildManager = FindObjectOfType<BuildTreeManager>();
        _interpreter = FindObjectOfType<Interpreter>();

        // Get the Image component of the current drop target
        targetImage = GetComponent<Image>();
        allSlots.Add(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            RectTransform dropTargetRectTransform = GetComponent<RectTransform>();
            RectTransform draggedRectTransform = eventData.pointerDrag.GetComponent<RectTransform>();

            // Snap the dragged object to the center of the drop target
            draggedRectTransform.anchoredPosition = dropTargetRectTransform.anchoredPosition;

            // Get the DragDrop component from the dragged object
            DragDrop dragDropComponent = eventData.pointerDrag.GetComponent<DragDrop>();

            // If the component is moved to a new slot, update the BuildTreeManager
            if (dragDropComponent != null)
            {
                _playerBuildManager.AddComponent(dragDropComponent.component, transform.GetSiblingIndex());

                // Update the current slot reference in the DragDrop script
                dragDropComponent.SetCurrentSlot(this);
                currentDragDropComponent = dragDropComponent; // Update the component in this slot
                UpdateSlotColors();

                if (_interpreter != null)
                {
                    string message = $"Component {dragDropComponent.component.name} has been added to slot {transform.GetSiblingIndex()}";
                    List<string> response = _interpreter.Interpret(message);  // Send message to the Interpreter
                    FindObjectOfType<TerminalManager>().AddDirectoryLine("---Action---");
                    FindObjectOfType<TerminalManager>().AddInterpreterLine(response);
                }
            }
            // List<string> response = _interpreter.Interpret("*****update");

            // Update the terminal display
            // FindObjectOfType<TerminalManager>().AddDirectoryLine("---Action---");
            // FindObjectOfType<TerminalManager>().AddInterpreterLine(response);
        }
    }

    private void UpdateSlotColors()
    {
        foreach (EmptySlot slot in allSlots)
        {
            if (slot.currentDragDropComponent != null)
            {
                slot.targetImage.color = Color.green; // Set to green if the slot has a component
            }
            else
            {
                slot.targetImage.color = Color.white; // Reset to white if the slot is empty
            }
        }
    }

    public void RemoveComponentFromSlot()
    {
        currentDragDropComponent = null; // Clear the reference to the removed component
        UpdateSlotColors(); // Update slot colors after removal
    }

    private void OnDestroy()
    {
        // Unregister this slot from the static list when destroyed
        allSlots.Remove(this);
    }

}
