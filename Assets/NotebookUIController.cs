using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookUIController : MonoBehaviour
{
   public GameObject notebookCanvas;
   public GameObject icon;
   private bool keepIconInitiallyHidden = true;
    void Start()
    {
        notebookCanvas.SetActive(false);
        icon.SetActive(false);
    }
    
    public void Update(){
        if (GameManager.instance.collectedNotebook)
        {
            if (keepIconInitiallyHidden)
            {
                icon.SetActive(true);
                keepIconInitiallyHidden = false;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                icon.SetActive(!icon.activeSelf);
                notebookCanvas.SetActive(!notebookCanvas.activeSelf);
            }
        }
    }
    
}
