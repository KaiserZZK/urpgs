using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CompDescManager: MonoBehaviour
{
    public static CompDescManager _instance;

    public TextMeshProUGUI textComponent;

    private void Awake()
    {
        // Ensure this is always only 1 instance of the description box
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowDescription(string description)
    {
        gameObject.SetActive(true);
        textComponent.text = description;
        Debug.Log(transform.position);
    }

    public void HideDescription()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }

}