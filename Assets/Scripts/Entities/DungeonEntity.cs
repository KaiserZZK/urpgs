using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonEntity : MonoBehaviour
{
    public Tile tile; // Appearance
    public bool isSentient = false;

    void Awake()
    {
        if (GetComponent<DungeonPlayer>())
        {
            GameManager.instance.dungeon.entities.Insert(0, this);
        }
        else if (isSentient)
        {
            GameManager.instance.dungeon.entities.Add(this);
        }
    }
}
