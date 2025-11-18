using UnityEngine;
public class Room
{

    public RoomType Type;
    public int PrizeTable;
    public int EnemyTable;
    public bool isVisited;
    public Room()
    {
        Type = RoomType.Void;
        PrizeTable = 0;
        EnemyTable = 0;
        isVisited = false;
    }
}