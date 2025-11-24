using UnityEditor.Search;
using UnityEngine;

public class NextFloorDoor : MonoSingleton<NextFloorDoor>
{
    public GameObject doorPrefab;
    private GameObject doorObj;
    private Vector3 doorPosition;

    void Awake()
    {
        if (doorPrefab != null)
        {
            Debug.Log("문 만들었음.");
            doorObj = Instantiate(doorPrefab, Vector3.zero, Quaternion.identity);
            doorObj.SetActive(false);
        }
    }

    public void ShowNextFloorDoor()
    {
        Debug.Log($"show 넥스트 도어, {doorObj}, {doorPosition}");
        doorObj.transform.position = doorPosition;
        doorObj.SetActive(true);
    }

    public void HideNextFloorDoor()
    {
        doorObj.SetActive(false);
    }

    public void SetDoorPosition(Vector3 pos)
    {
        Debug.Log("set door pos");
        doorPosition = new Vector3(pos.x, pos.y, pos.z);
    }
}
