using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Scripts.BuildTree;

public class DragDrop :
    MonoBehaviour,
    IPointerDownHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IDragHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] public ScriptableComponent component;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 originalPosition; // Track the original position for resetting when dropped outside a slot
    private EmptySlot currentSlot;    // Keep track of the current slot the component is in
    private bool isPlacedOnSlot = false;  // Boolean flag to check if the component is on a slot

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = transform.position; // Save the starting position
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;

        // Save the original position in world space for resetting later
        originalPosition = transform.position;

        if (isPlacedOnSlot && currentSlot != null)
        {
            var buildTreeManager = FindObjectOfType<BuildTreeManager>();
            buildTreeManager.RemoveComponent(component);
            currentSlot.RemoveComponentFromSlot(); // Inform the slot that the component is removed
            currentSlot = null; // Clear current slot tracking
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (currentSlot == null)
        {
            // Check if dropped on an OriginalSlot
            OriginalSlot originalSlot = eventData.pointerEnter?.GetComponent<OriginalSlot>();
            if (originalSlot != null)
            {
                Debug.Log("Dropped on the original slot.");

                // Convert world position to local position relative to the original parent
                rectTransform.position = originalSlot.transform.position;

                // Trigger the OnDrop logic for decrementing stats
                originalSlot.OnDrop(eventData);
            }
            else
            {
                // Dropped outside any valid slot, reset position
                Debug.Log("Dropped outside a slot, resetting position.");

                // Reset using anchoredPosition for UI elements
                transform.position = originalPosition;
                isPlacedOnSlot = false;
            }
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Component selected.");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DescriptionDisplay.ShowDescirptionStatic(component.ComponentDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescriptionDisplay.HideDescriptionStatic();
    }

    // Set the current slot when the component is successfully dropped into a slot
    public void SetCurrentSlot(EmptySlot slot)
    {
        currentSlot = slot;
        isPlacedOnSlot = true;
    }

    // Get the current slot for other scripts to access
    public EmptySlot GetCurrentSlot()
    {
        return currentSlot;
    }

}
