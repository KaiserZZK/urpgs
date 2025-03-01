using Scripts.BuildTree;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OriginalSlot : MonoBehaviour, IDropHandler
{
    private BuildTreeManager _playerBuildManager;
    private Image targetImage;

    private void Awake()
    {
        _playerBuildManager = FindObjectOfType<BuildTreeManager>();
        targetImage = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            RectTransform dropTargetRectTransform = GetComponent<RectTransform>();
            RectTransform draggedRectTransform = eventData.pointerDrag.GetComponent<RectTransform>();

            // Get the DragDrop component from the dragged object
            DragDrop dragDropComponent = eventData.pointerDrag.GetComponent<DragDrop>();

            if (dragDropComponent != null)
            {
                // Snap the dragged object to the position of the original slot (world space)
                draggedRectTransform.position = dropTargetRectTransform.position;

                // Decrement the stats when the component is returned to its original spot
                _playerBuildManager.RemoveComponent(dragDropComponent.component);

                // Clear any slot reference in the DragDrop script
                dragDropComponent.SetCurrentSlot(null);
                Debug.Log($"Component {dragDropComponent.component.name} returned to the original slot. Attributes decremented.");
            }
        }
    }
}

