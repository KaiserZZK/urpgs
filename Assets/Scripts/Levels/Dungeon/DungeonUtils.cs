using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonUtils
{
    public static int DistanceBetweenPoints(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
