using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RangeAttribute = UnityEngine.RangeAttribute;

public class MapManager : MonoBehaviour
{
    [Header("Map Stats")]
    public MapStats[] mapStatsPool;
    private MapStats currentStats;

    [Header("Room Data by Type")]
    public RoomData emptyData;
    public RoomData eventData;
    public RoomData combatData;
    public RoomData mysteryData;
    public RoomData bossData;

    private int width = 7;
    private int height = 7;
    public int WIDTH => width;
    public int HEIGHT => height;
    [Range(7, 30)] public int minRooms = 7;
    [Range(10, 30)] public int maxRooms = 10;

    public Room[,] Map { get; private set; }
    public Vector2Int playerPos { get; private set; }

    private int currentRoomCount = 0;

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private List<Vector2Int> endRooms = new List<Vector2Int>();
    private HashSet<Vector2Int> assignedRooms = new HashSet<Vector2Int>();
    private MiniMap miniMap;

    private void Awake()
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

        ChooseRandomMapStats();
        width = currentStats.Width;
        height = currentStats.Height;

        playerPos = new Vector2Int(width / 2, height / 2);

        Map = new Room[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Map[i, j] = new Room();
            }
        }

        bool success = false;

        while (!success)
        {
            success = GenerateMap();
        }

        AssignRoomTypes();
        AssignRoomData();
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

    private void ChooseRandomMapStats()
    {
        currentStats = mapStatsPool[Random.Range(0, mapStatsPool.Length)];
    }

    private void AssignRoomTypes()
    {
        if (assignedRooms.Count == 0) return;

        Vector2Int start = playerPos;
        Map[start.x, start.y].Type = RoomType.Empty;

        Vector2Int boss = FindFarthestRoomFrom(start, endRooms);
        Map[boss.x, boss.y].Type = RoomType.BossRoom;

        List<Vector2Int> pool = assignedRooms.Where(r => r != start && r != boss).ToList();

        Shuffle(pool);

        int index = 0;

        // Assign empty rooms
        for (int i = 0; i < currentStats.EmptyNum && index < pool.Count; i++)
            Map[pool[index++].x, pool[index - 1].y].Type = RoomType.Empty;

        // Assign event rooms
        for (int i = 0; i < currentStats.EventNum && index < pool.Count; i++)
            Map[pool[index++].x, pool[index - 1].y].Type = RoomType.EventRoom;

        // Assign combat rooms
        for (int i = 0; i < currentStats.CombatNum && index < pool.Count; i++)
            Map[pool[index++].x, pool[index - 1].y].Type = RoomType.CombatRoom;

        // Assign mystery rooms
        for (int i = 0; i < currentStats.MysteryNum && index < pool.Count; i++)
            Map[pool[index++].x, pool[index - 1].y].Type = RoomType.MysteryRoom;

        // Remaining rooms → default room type 
        while (index < pool.Count)
        {
            var pos = pool[index++];
            Map[pos.x, pos.y].Type = RoomType.CombatRoom; 
        }
    }

    private void AssignRoomData()
    {
        for (int x = 0; x < width;  x++)
        {
            for(int y = 0; y < height; y++)
            {
                Room room = Map[x, y];
                switch(room.Type)
                {
                    case RoomType.Empty:
                        room.Data = emptyData;
                        break;
                    case RoomType.EventRoom:
                        room.Data = eventData;
                        break;
                    case RoomType.MysteryRoom:
                        room.Data = mysteryData;
                        break;
                    case RoomType.CombatRoom:
                        room.Data = combatData;
                        break;
                    case RoomType.BossRoom:
                        room.Data = bossData;
                        break;
                    default:
                        room.Data = null;
                        break;
                }
            }
        }
    }

    private bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private bool VisitCell(Vector2Int pos)
    {
        if (!InBounds(pos) || Map[pos.x, pos.y].Type != RoomType.Void || GetNeighborCount(pos) > 1 || currentRoomCount >= maxRooms || Random.value < 0.5f)
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
            if (InBounds(check) && Map[check.x, check.y].Type != RoomType.Void)
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
                if (InBounds(next) && Map[next.x, next.y].Type != RoomType.Void && !visited.Contains(next))
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

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i <list.Count; i++)
        {
            int rand = Random.Range(0, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
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
                    case RoomType.Void:
                        c = '.';
                        break;
                    case RoomType.Empty:
                        c = 'E';
                        break;
                    case RoomType.EventRoom:
                        c = 'S';
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

    public bool CanMove(Vector2Int dir)
    {
        Vector2Int next = playerPos + dir;

        if (!InBounds(next)) return false;
        if (Map[next.x, next.y].Type == RoomType.Void) return false;

        return true;
    }

    public Room GetCurrentRoom()
    {
        return Map[playerPos.x, playerPos.y];
    }

    public void movePlayer(int x, int y)
    {
        playerPos = new Vector2Int(x, y);
        Map[x, y].isVisited = true;

        miniMap.HighlightPlayerRoom();

        Debug.Log($"Player moved to: ({playerPos.x}, {playerPos.y})");
    }

    public void movePlayer(Vector2Int dir)
    {
        movePlayer(playerPos.x + dir.x, playerPos.y + dir.y);
    }
}