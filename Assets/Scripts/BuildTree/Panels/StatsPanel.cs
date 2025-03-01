using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatsPanel : MonoBehaviour
{
    private Label _stabilityLabel, _adaptabilityLabel, _strengthLabel;

    private PanelManager _panelManager;
    
    private void Awake()
    {
        _panelManager = GetComponent<PanelManager>();
        if (_panelManager is null)
        {
            Debug.LogError("PanelManager not found on StatsPanel");
        }
    }

    private void Start()
    {
        _panelManager.PlayerBuildManager.OnEmptySlotsChanged += PopulateLabelText;
        GatherLabelReferences();
        PopulateLabelText();
    }

    private void GatherLabelReferences()
    {
        _stabilityLabel = _panelManager.UIDocument.rootVisualElement.Q<Label>("current_stability");
        _adaptabilityLabel = _panelManager.UIDocument.rootVisualElement.Q<Label>("current_adaptability");
        _strengthLabel = _panelManager.UIDocument.rootVisualElement.Q<Label>("current_strength");
    }

    private void PopulateLabelText()
    {
        _stabilityLabel.text = _panelManager.PlayerBuildManager.Stability.ToString();
        _adaptabilityLabel.text = _panelManager.PlayerBuildManager.Adaptability.ToString();
        _strengthLabel.text = _panelManager.PlayerBuildManager.Strength.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        PopulateLabelText();
    }
}
