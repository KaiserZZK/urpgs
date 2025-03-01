using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonTile
{
    public Tile unityTile; // Tilemap tile as defined in Unity

    // State
    public DungeonTileType tileType;
}

public enum DungeonTileType
{
    Unexplored_0,
    Unexplored_1,
    Empty,
    Player,
    Wall,
    Wall_Visible,
    Portal,
    Portal_Visible,
}