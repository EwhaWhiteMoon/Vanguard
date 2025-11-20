using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomData : ScriptableObject
{
    public RoomType Type;

    [Header("Tables")]
    public int PrizeTable;
    public int EnemyTable;

    [Header("Visuals")]
    public Sprite minimapSprite;

    [Header("Flags")]
    public bool canContainEnemies = true;
    public bool canContainLoot = true;
}
