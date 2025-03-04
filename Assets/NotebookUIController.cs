using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookUIController : MonoBehaviour
{
   public GameObject notebookCanvas;
    void Start()
    {
        notebookCanvas.SetActive(false);
    }
    
    public void Update(){
        if (GameManager.instance.collectedNotebook && Input.GetKeyDown(KeyCode.R))
        {
            notebookCanvas.SetActive(!notebookCanvas.activeSelf);
        }
    }
    
}
