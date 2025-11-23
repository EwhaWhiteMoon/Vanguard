using UnityEngine;

public class RoomVisualManager : MonoBehaviour
{
    public Transform roomParent;
    private GameObject activeRoom;

    public void ShowRoom(Room room)
    {
        if (activeRoom != null)
        {
            Destroy(activeRoom);
        }

        var layouts = room.Data.roomLayouts;

        if(layouts == null || layouts.Count == 0)
        {
            Debug.LogWarning("No layouts prefabs found!");
            return;
        }

        GameObject prefab = layouts[Random.Range(0, layouts.Count)];
        activeRoom = Instantiate(prefab, roomParent.position, Quaternion.identity);
    }
}
