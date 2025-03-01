using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonActions
{
    public static void Move(Dungeon dungeon, DungeonEntity entity, Vector2 direction)
    {
        entity.transform.position += (Vector3)direction;
    }

    public static void Wait(Dungeon dungeon, DungeonEntity entity)
    {
        Debug.Log(entity.name + " skipped their turn");
    }
}
