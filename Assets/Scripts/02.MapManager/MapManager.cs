using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RangeAttribute = UnityEngine.RangeAttribute;

public class MapManager : MonoSingleton<MapManager>
{
    [Header("Map Size")]
    public int width = 10;
    public int height = 10;
    [Range(7, 30)] public int minRooms = 7;
    [Range(10, 30)] public int maxRooms = 10;

    public Room[,] Map { get; private set; }
    public Vector2Int playerPos { get; private set; }

    private int currentRoomCount = 0;

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private List<Vector2Int> endRooms = new List<Vector2Int>();
    private HashSet<Vector2Int> assignedRooms = new HashSet<Vector2Int>();
    private MiniMap miniMap;

    private void Start()
    {
        miniMap = FindFirstObjectByType<MiniMap>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitMap();
            miniMap.RefreshMiniMap();
        }
    }

    public void InitMap()
    {
        if (Map != null)
        {
            Map = null;
        }

        playerPos = new Vector2Int(width / 2, height / 2);

        Map = new Room[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Map[i, j] = new Room();
            }
        }

        const int maxAttempts = 20;
        int attemp = 0;
        bool success = false;

        while (!success && attemp < maxAttempts)
        {
            attemp++;
            success = GenerateMap();
        }

        if (!success)
        {
            Debug.Log("Failed to generate a valid map after " + maxAttempts + " attemps.");
        }

        AssignRoomTypes();
        PrintMapToConsole();
    }

    private bool GenerateMap()
    {
        roomQueue.Clear();
        endRooms.Clear();
        assignedRooms.Clear();
        currentRoomCount = 0;

        Vector2Int startRoom = playerPos;

        Map[startRoom.x, startRoom.y].Type = RoomType.EventRoom;
        roomQueue.Enqueue(startRoom);
        assignedRooms.Add(startRoom);
        currentRoomCount++;

        while (roomQueue.Count > 0)
        {
            Vector2Int current = roomQueue.Dequeue();
            bool created = false;

            foreach (var dir in GetDirections())
            {
                Vector2Int next = current + dir;
                if (VisitCell(next)) created = true;
            }

            if (!created)
            {
                endRooms.Add(current);
            }
        }

        if (currentRoomCount < minRooms)
        {
            return false;
        }
        return true;
    }

    private void AssignRoomTypes()
    {
        if (assignedRooms.Count == 0) return;

        Vector2Int start = playerPos;
        Map[start.x, start.y].Type = RoomType.EventRoom;

        Vector2Int boss = FindFarthestRoomFrom(start, endRooms);
        Map[boss.x, boss.y].Type = RoomType.BossRoom;

        foreach (var room in assignedRooms)
        {
            if (room == start || room == boss) continue;

            float r = Random.value;
            if (r < 0.6f)
            {
                Map[room.x, room.y].Type = RoomType.CombatRoom;
            }
            else if (r >= 0.6f && r < 0.8f)
            {
                Map[room.x, room.y].Type = RoomType.EventRoom;
            }
            else
            {
                Map[room.x, room.y].Type = RoomType.MysteryRoom;
            }
        }
    }

    private bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private bool VisitCell(Vector2Int pos)
    {
        if (!InBounds(pos) || Map[pos.x, pos.y].Type != RoomType.Empty || GetNeighborCount(pos) > 1 || currentRoomCount >= maxRooms || Random.value < 0.5f)
        {
            return false;
        }

        Map[pos.x, pos.y].Type = RoomType.EventRoom;
        currentRoomCount++;
        roomQueue.Enqueue(pos);
        assignedRooms.Add(pos);

        return true;
    }

    private int GetNeighborCount(Vector2Int pos)
    {
        int count = 0;
        foreach (var dir in GetDirections())
        {
            Vector2Int check = pos + dir;
            if (InBounds(check) && Map[check.x, check.y].Type != RoomType.Empty)
            {
                count++;
            }
        }
        return count;
    }

    private Vector2Int FindFarthestRoomFrom(Vector2Int start, List<Vector2Int> rooms)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        visited.Add(start);
        distance[start] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = current + dir;
                if (InBounds(next) && Map[next.x, next.y].Type != RoomType.Empty && !visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                    distance[next] = distance[current] + 1;
                }
            }
        }

        Vector2Int farthest = start;
        int maxDist = 0;
        foreach (var room in rooms)
        {
            if (distance.TryGetValue(room, out int d) && d > maxDist)
            {
                maxDist = d;
                farthest = room;
            }
        }

        return farthest;
    }


    private static readonly Vector2Int[] baseDirs =
{
    Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
};

    private IEnumerable<Vector2Int> GetDirections()
    {
        Vector2Int[] dirs = (Vector2Int[])baseDirs.Clone();

        for (int i = 0; i < dirs.Length; i++)
        {
            int rand = Random.Range(i, dirs.Length);
            (dirs[i], dirs[rand]) = (dirs[rand], dirs[i]);
        }

        return dirs;
    }

    private void PrintMapToConsole()
    {
        string mapString = "";

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                Room room = Map[x, y];
                char c = '.';

                switch (room.Type)
                {
                    case RoomType.Empty:
                        c = '.';
                        break;
                    case RoomType.EventRoom:
                        c = 'E';
                        break;
                    case RoomType.MysteryRoom:
                        c = 'M';
                        break;
                    case RoomType.CombatRoom:
                        c = 'C';
                        break;
                    case RoomType.BossRoom:
                        c = 'B';
                        break;
                }
                mapString += c + " ";
            }
            mapString += "\n";
        }
        Debug.Log(mapString);
    }

    public Room GetCurrentRoom()
    {
        return Map[playerPos.x, playerPos.y];
    }

    public void movePlayer(int x, int y)
    {
        playerPos = new Vector2Int(x, y);
        Map[x, y].isVisited = true;
    }
}