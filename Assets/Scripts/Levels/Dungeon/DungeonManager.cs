using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonManager : MonoBehaviour
{
    [Header("Components")]
    public Dungeon dungeon;
    public Tilemap knowledgeMap; // Currently revealed tiles (Includes unexplored)
    public Tilemap entityMap; // Entities

    [Header("MapGen Settings")]
    public int width = 200; // Tiles
    public int height = 100; // Tiles

    [Header("Tilesets")]
    public Dictionary<DungeonTileType, Tile> tileDict = new Dictionary<DungeonTileType, Tile>();
    public Dictionary<DungeonTileType, DungeonTileType> visibleTileDict = new Dictionary<DungeonTileType, DungeonTileType>();

    [Header("Map Data")]
    public Dictionary<Vector2, DungeonTileType> dungeonLayout = new Dictionary<Vector2, DungeonTileType>(); // Full map layout
    public List<DungeonRoom> rooms = new List<DungeonRoom>();

    [Header("Rendering")]
    public HashSet<Vector2> visibleTiles = new HashSet<Vector2>(); // Tiles that are currently visible

    public void SetUpGame()
    {
        LoadTileset(); // Load tile prefabs into tileDict

        // Generate dungeon
        dungeonLayout = DungeonProcGen.GenerateDungeon(width, height, 5, 20, 100, rooms);

        // Randomly choose a room to spawn the player in after ProcGen
        DungeonRoom playerRoom = rooms[Random.Range(0, rooms.Count)];
        Vector2Int playerPos = new Vector2Int(
            Random.Range(playerRoom.x, playerRoom.x + playerRoom.width),
            Random.Range(playerRoom.y, playerRoom.y + playerRoom.height)
        );
        DungeonPlayer player = Instantiate(Resources.Load("Prefabs/DungeonPlayer"), transform).GetComponent<DungeonPlayer>();
        player.name = "DungeonPlayer";
        player.transform.position = new Vector2(playerPos.x, playerPos.y);
        dungeon.CenterCameraOnPlayer();

        // Draw dungeon
        knowledgeMap.ClearAllTiles();
        foreach (KeyValuePair<Vector2, DungeonTileType> kvp in dungeonLayout)
        {
            Vector3Int pos = new Vector3Int((int)kvp.Key.x, (int)kvp.Key.y, 0);

            // Every tile is unexplored for now
            int unexplored_variant = Random.Range(0, 2);
            if (unexplored_variant == 0)
            {
                knowledgeMap.SetTile(pos, tileDict[DungeonTileType.Unexplored_0]);
            }
            else
            {
                knowledgeMap.SetTile(pos, tileDict[DungeonTileType.Unexplored_1]);
            }
            // DEV: reveal all map
            // knowledgeMap.SetTile(pos, tileDict[kvp.Value]);
        }
        RevealMapAroundPoint(playerPos, player.visionRange);
        PlayerVisionAroundPoint(playerPos, player.visionRange);

        RefreshTiles();
    }

    public void RefreshTiles()
    {
        // Clear entityMap and redraw
        entityMap.ClearAllTiles();
        foreach (DungeonEntity entity in dungeon.entities)
        {
            entityMap.SetTile(new Vector3Int((int)entity.transform.position.x, (int)entity.transform.position.y, 0), entity.tile);
        }
    }

    // Load tile prefabs into tileDict
    void LoadTileset()
    {
        // Load tiles that are used in the dungeon
        tileDict.Add(DungeonTileType.Empty, Resources.Load<Tile>("Tilesets/RDE_8x8 Tiles/empty"));
        tileDict.Add(DungeonTileType.Wall, Resources.Load<Tile>("Tilesets/RDE_8x8 Tiles/wall"));
        tileDict.Add(DungeonTileType.Portal, Resources.Load<Tile>("Tilesets/RDE_8x8 Tiles/portal"));

        // Load tiles that are used in the knowledge map
        tileDict.Add(DungeonTileType.Unexplored_0, Resources.Load<Tile>("Tilesets/RDE_8x8 Tiles/unexplored_0"));
        tileDict.Add(DungeonTileType.Unexplored_1, Resources.Load<Tile>("Tilesets/RDE_8x8 Tiles/unexplored_1"));

        visibleTileDict.Add(DungeonTileType.Wall, DungeonTileType.Wall_Visible);
        tileDict.Add(DungeonTileType.Wall_Visible, Resources.Load<Tile>("Tilesets/RDE_8x8 Tiles/wall_visible"));
        visibleTileDict.Add(DungeonTileType.Portal, DungeonTileType.Portal_Visible);
        tileDict.Add(DungeonTileType.Portal_Visible, Resources.Load<Tile>("Tilesets/RDE_8x8 Tiles/portal_visible"));
    }

    public bool IsValidMove(Vector2Int end)
    {
        if (dungeonLayout.ContainsKey(end) && dungeonLayout[end] == DungeonTileType.Empty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RevealMapPoint(Vector2Int pos)
    {
        Tile mapTileAtPoint = tileDict[dungeonLayout[pos]];
        knowledgeMap.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), mapTileAtPoint);
    }
    public void RevealMapAroundPoint(Vector2Int pos, float radius)
    {
        foreach (KeyValuePair<Vector2, DungeonTileType> kvp in dungeonLayout)
        {
            if (Vector2.Distance(pos, kvp.Key) <= radius)
            {
                RevealMapPoint(new Vector2Int((int)kvp.Key.x, (int)kvp.Key.y));
            }
        }
    }

    public void PlayerVisionAroundPoint(Vector2Int point, int visionRange)
    {
        // First, reset the "vision map" by setting all tiles in current visibleTiles to their normal variant (if they have one)
        foreach (Vector2 pos in visibleTiles)
        {
            DungeonTileType tileType = dungeonLayout[pos];
            if (visibleTileDict.ContainsKey(tileType))
            {
                knowledgeMap.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), tileDict[tileType]);
            }
        }

        // Now, update visibleTiles by replacing it with a new set of tiles within visionRange
        visibleTiles = new HashSet<Vector2>();
        for (int x = point.x - visionRange; x <= point.x + visionRange; x++)
        {
            for (int y = point.y - visionRange; y <= point.y + visionRange; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (dungeonLayout.ContainsKey(pos) && Vector2.Distance(point, pos) <= visionRange)
                {
                    visibleTiles.Add(pos);
                }
            }
        }

        // Finally, update the "vision map" by setting all tiles in the new visibleTiles to their _visible variant (if they have one)
        foreach (Vector2 pos in visibleTiles)
        {
            DungeonTileType tileType = dungeonLayout[pos];
            if (visibleTileDict.ContainsKey(tileType))
            {
                knowledgeMap.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), tileDict[visibleTileDict[tileType]]);
            }
        }
    }
}
