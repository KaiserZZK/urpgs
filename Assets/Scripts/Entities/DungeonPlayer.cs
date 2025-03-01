using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DungeonPlayer : DungeonEntity
{
    [Header("Stats")]
    public int visionRange = 10;
    public int detectedRange = 10;
    public int coherence = 150;
    public int dissonance = 30;
}
