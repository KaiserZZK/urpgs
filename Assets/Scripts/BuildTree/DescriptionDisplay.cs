using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionDisplay : MonoBehaviour
{

    private static DescriptionDisplay instance;

    [SerializeField]
    private Camera uiCamera;

    private Text descriptionText;
    private RectTransform backgroundRectTransform;

    private void Awake()
    {
        instance = this;

        backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        descriptionText = transform.Find("Text").GetComponent<Text>();

        ShowDescription("random test text!~");

        HideDescription();
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent.GetComponent<RectTransform>(),
            Input.mousePosition,
            uiCamera,
            out localPoint
        );

        transform.localPosition = localPoint;
    }

    private void ShowDescription(string descriptionString)
    {
        gameObject.SetActive(true);

        descriptionText.text = descriptionString;
        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(
            descriptionText.preferredWidth + textPaddingSize * 2f,
            descriptionText.preferredHeight + textPaddingSize * 2f
        );

        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void HideDescription()
    {
        gameObject.SetActive(false);
    }

    public static void ShowDescirptionStatic(string descriptionString)
    {
        instance.ShowDescription(descriptionString);
    }

    public static void HideDescriptionStatic()
    {
        instance.HideDescription();
    }

}