using UnityEngine;

public class MapManager
{
    public Room[,] Map { get; private set; }
    public Vector2Int playerPos { get; private set; } = new Vector2Int(0, 0);

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