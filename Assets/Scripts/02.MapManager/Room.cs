using UnityEngine;
public class Room
{

    public RoomType Type;
    public RoomData Data;
    public bool isVisited;
    public Room()
    {
        Type = RoomType.Void;
        Data = null;
        isVisited = false;
    }
}