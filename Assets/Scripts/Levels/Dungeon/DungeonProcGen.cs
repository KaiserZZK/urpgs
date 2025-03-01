using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

sealed class DungeonProcGen : MonoBehaviour
{
    public static Dictionary<Vector2, DungeonTileType> GenerateDungeon(
        int width, int height,
        int roomWidthHeightMin, int roomWidthHeightMax, int maxRooms,
        List<DungeonRoom> rooms)
    {
        Dictionary<Vector2, DungeonTileType> dungeonLayout = new Dictionary<Vector2, DungeonTileType>();
        // Fill with wall tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dungeonLayout.Add(new Vector2(x, y), DungeonTileType.Wall);
            }
        }

        // Generate BSP
        BSPLeaf root = new BSPLeaf(0, 0, width, height);
        List<BSPLeaf> leaves = new List<BSPLeaf>(); // Actual "leaves" of the tree which will not be split any further, these are the rooms

        // DFS leaves to split
        List<BSPLeaf> leavesToSplit = new List<BSPLeaf>();
        leavesToSplit.Add(root);
        while (leavesToSplit.Count > 0)
        {
            BSPLeaf leaf = leavesToSplit[0];
            leavesToSplit.RemoveAt(0);
            if (leaf.width > leaf.maxSize || leaf.height > leaf.maxSize || Random.value > 0.25f)
            {
                if (leaf.Split())
                {
                    leavesToSplit.Add(leaf.leftChild);
                    leavesToSplit.Add(leaf.rightChild);
                }
                else
                {
                    leaves.Add(leaf);
                }
            }
        }

        // Place rooms in leaves
        root.MakeDungeonRooms(rooms, roomWidthHeightMin);

        // Draw rooms
        foreach (DungeonRoom room in rooms)
        {
            // Build it
            for (int x = room.x; x < room.x + room.width; x++)
            {
                for (int y = room.y; y < room.y + room.height; y++)
                {
                    dungeonLayout[new Vector2(x, y)] = DungeonTileType.Empty;
                }
            }
        }

        // Connect each unconnected room to the closest connected room 
        // and 2 random rooms out of the 3 closest rooms (which can themselves already be connected)
        List<DungeonRoom> connectedRooms = new List<DungeonRoom>();
        connectedRooms.Add(rooms[0]); // Start with the first room (arbitrary choice)
        foreach (DungeonRoom room in rooms)
        {
            // Connect each room to the closest connected room
            if (connectedRooms.Contains(room))
            {
                continue;
            }
            DungeonRoom closestConnectedRoom = GetKClosestDungeonRooms(room, 1, connectedRooms)[0];
            MakeCorridoorBetweenRooms(dungeonLayout, room, closestConnectedRoom);
            connectedRooms.Add(room);

            // Then connect each room to 2 random rooms out of the 3 closest rooms
            List<DungeonRoom> closestRooms = GetKClosestDungeonRooms(room, 3, rooms);
            closestRooms = closestRooms.OrderBy(x => Random.value).Take(2).ToList();
            foreach (DungeonRoom closestRoom in closestRooms)
            {
                MakeCorridoorBetweenRooms(dungeonLayout, room, closestRoom);
            }
        }

        // Pick random room for portal
        DungeonRoom portalRoom = rooms[Random.Range(0, rooms.Count)];
        Vector2Int portalPos = new Vector2Int(
            Random.Range(portalRoom.x, portalRoom.x + portalRoom.width),
            Random.Range(portalRoom.y, portalRoom.y + portalRoom.height)
        );
        dungeonLayout[new Vector2(portalPos.x, portalPos.y)] = DungeonTileType.Portal;

        return dungeonLayout;
    }

    private static void MakeCorridoorBetweenRooms(Dictionary<Vector2, DungeonTileType> dungeonLayout, DungeonRoom roomA, DungeonRoom roomB)
    {
        Vector2Int roomCenter = roomA.Center();
        Vector2Int closestRoomCenter = roomB.Center();
        int x = roomCenter.x;
        int y = roomCenter.y;
        while (x != closestRoomCenter.x)
        {
            dungeonLayout[new Vector2(x, y)] = DungeonTileType.Empty;
            x += x < closestRoomCenter.x ? 1 : -1;
        }
        while (y != closestRoomCenter.y)
        {
            dungeonLayout[new Vector2(x, y)] = DungeonTileType.Empty;
            y += y < closestRoomCenter.y ? 1 : -1;
        }
    }

    public static List<DungeonRoom> GetKClosestDungeonRooms(DungeonRoom room, int k, List<DungeonRoom> rooms)
    {
        List<DungeonRoom> closestRooms = new List<DungeonRoom>();
        foreach (DungeonRoom otherRoom in rooms)
        {
            if (otherRoom == room)
            {
                continue;
            }
            if (closestRooms.Count < k)
            {
                closestRooms.Add(otherRoom);
            }
            else
            {
                // Find the farthest room in closestRooms
                DungeonRoom farthestRoom = closestRooms[0];
                int farthestDistance = DungeonUtils.DistanceBetweenPoints(room.Center(), farthestRoom.Center());
                foreach (DungeonRoom closestRoom in closestRooms)
                {
                    int distance = DungeonUtils.DistanceBetweenPoints(room.Center(), closestRoom.Center());
                    if (distance > farthestDistance)
                    {
                        farthestRoom = closestRoom;
                        farthestDistance = distance;
                    }
                }
                // Replace farthest room with otherRoom if otherRoom is closer
                int otherRoomDistance = DungeonUtils.DistanceBetweenPoints(room.Center(), otherRoom.Center());
                if (otherRoomDistance < farthestDistance)
                {
                    closestRooms.Remove(farthestRoom);
                    closestRooms.Add(otherRoom);
                }
            }
        }
        return closestRooms;
    }

    class BSPLeaf
    {
        public int minSize = 10;
        public int maxSize = 30;
        public int x;
        public int y;
        public int width;
        public int height;
        public DungeonRoom room;

        public BSPLeaf leftChild;
        public BSPLeaf rightChild;

        public BSPLeaf(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool Split()
        {
            if (leftChild != null || rightChild != null)
            {
                return false; // Already split
            }

            bool splitHorizontally = Random.value > 0.5f;
            if (width > height && width / height >= 1.25f)
            {
                splitHorizontally = false;
            }
            else if (height > width && height / width >= 1.25f)
            {
                splitHorizontally = true;
            }

            int max = (splitHorizontally ? height : width) - minSize;
            if (max <= minSize)
            {
                return false; // Area too small to split
            }

            int split = Random.Range(minSize, max);
            if (splitHorizontally)
            {
                leftChild = new BSPLeaf(x, y, width, split);
                rightChild = new BSPLeaf(x, y + split, width, height - split);
            }
            else
            {
                leftChild = new BSPLeaf(x, y, split, height);
                rightChild = new BSPLeaf(x + split, y, width - split, height);
            }

            return true;
        }

        public void MakeDungeonRooms(List<DungeonRoom> rooms, int roomSizeMin)
        {
            if (leftChild != null || rightChild != null)
            {
                if (leftChild != null)
                {
                    leftChild.MakeDungeonRooms(rooms, roomSizeMin);
                }
                if (rightChild != null)
                {
                    rightChild.MakeDungeonRooms(rooms, roomSizeMin);
                }
            }
            else
            {
                // If leaf, make room
                int roomWidth = Random.Range(roomSizeMin, width - 1);
                int roomHeight = Random.Range(roomSizeMin, height - 1);
                int roomX = Random.Range(1, width - roomWidth - 1);
                int roomY = Random.Range(1, height - roomHeight - 1);
                DungeonRoom newRoom = new DungeonRoom(x + roomX, y + roomY, roomWidth, roomHeight);
                room = newRoom;
                rooms.Add(newRoom);
            }
        }
    }
}

[System.Serializable]
public class DungeonRoom
{
    public int x;
    public int y;
    public int width;
    public int height;

    public DungeonRoom(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public Vector2Int Center()
    {
        int centerX = x + width / 2;
        int centerY = y + height / 2;
        return new Vector2Int(centerX, centerY);
    }

    public Bounds GetBounds()
    {
        return new Bounds(new Vector3(x, y, 0), new Vector3(width, height, 0));
    }

    public BoundsInt GetBoundsInt()
    {
        return new BoundsInt(new Vector3Int(x, y, 0), new Vector3Int(width, height, 0));
    }

    public bool OverlapsExistingRooms(List<DungeonRoom> otherRooms)
    {
        foreach (DungeonRoom otherRoom in otherRooms)
        {
            if (GetBounds().Intersects(otherRoom.GetBounds()))
            {
                return true;
            }
        }
        return false;
    }
}