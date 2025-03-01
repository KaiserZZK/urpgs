using System.Collections;
using System.Collections.Generic;
using Scripts.BuildTree;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelManager : MonoBehaviour
{
    private BuildTreeManager _playerBuildManager;
    public BuildTreeManager PlayerBuildManager => _playerBuildManager;

    private UIDocument _uiDocument;
    public UIDocument UIDocument => _uiDocument;

    private void Awake()
    {
        _playerBuildManager = FindObjectOfType<BuildTreeManager>();
        _uiDocument = GetComponent<UIDocument>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
