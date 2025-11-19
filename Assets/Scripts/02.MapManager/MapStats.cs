using UnityEngine;

[CreateAssetMenu(fileName = "MapStats", menuName = "Scriptable Objects/MapStats")]
public class MapStats : ScriptableObject
{
    [Header("Map Size")]
    [SerializeField] private int width = 7;
    [SerializeField] private int height = 7;

    [Header("Number of Rooms")]
    [SerializeField] private int emptyNum = 1;
    [SerializeField] private int eventNum = 2;
    [SerializeField] private int combatNum = 3;
    [SerializeField] private int mysteryNum = 1;

    public int Width => width;
    public int Height => height;
    public int EventNum => eventNum;
    public int CombatNum => combatNum;
    public int MysteryNum => mysteryNum;
    public int EmptyNum => emptyNum;
    
}
