using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    [Header("Game Elements")]
    public DungeonManager dungeonManager;
    public Dungeon dungeon;

    [Header("UI Elements")]
    public TextMeshProUGUI alertText;
    public Slider alertBar;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI hermetismText;
    public TextMeshProUGUI poiseText;
    public TextMeshProUGUI coherenceText;
    public TextMeshProUGUI dissonanceText;

    void Update()
    {
        // Update alert bar
        float alertProportion = (float)dungeon.alert / (float)dungeon.alertMax;
        alertBar.value = alertProportion;
        // Fill color
        if (alertProportion <= 0.20f)
        {
            alertBar.fillRect.GetComponent<Image>().color = Color.green;
        }
        else if (alertProportion >= 0.80f)
        {
            alertBar.fillRect.GetComponent<Image>().color = Color.red;
        }
        else
        {
            alertBar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.green, Color.red, (alertProportion - 0.20f) / 0.60f);
        }

        // Update time text
        timeText.text = "System.time: " + dungeon.time;

        // Update player stats
        DungeonPlayer player = dungeon.entities[0].GetComponent<DungeonPlayer>();
        hermetismText.text = player.visionRange.ToString();
        poiseText.text = player.detectedRange.ToString();
        coherenceText.text = player.coherence.ToString();
        dissonanceText.text = player.dissonance.ToString();
    }
}
