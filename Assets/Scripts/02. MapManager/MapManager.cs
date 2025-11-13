using UnityEngine;

public class MapManager
{
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

    public void InitMap(int width, int height)
    {
        playerPos = new Vector2Int(width / 2, height / 2); // 일단 중심에서..
        
        Map = new Room[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Map[i, j] = new Room();
            }
        }
        
        // TODO : map generate algorithm
    }
    
    public Room GetCurrentRoom()
    {
        return Map[playerPos.x, playerPos.y];
    }

    public void movePlayer(int x, int y)
    {
        playerPos = new Vector2Int(x, y);
    }
}