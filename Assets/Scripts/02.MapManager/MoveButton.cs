using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    public Button btnUp;
    public Button btnDown;
    public Button btnLeft;
    public Button btnRight;

    public MapManager map;
    private void Awake()
    {
        if(map == null)
        {
            map = FindFirstObjectByType<MapManager>();
        }
    }

    void UpdateButtons()
    {
        btnUp.gameObject.SetActive(map.CanMove(Vector2Int.up));
        btnDown.gameObject.SetActive(map.CanMove(Vector2Int.down));
        btnLeft.gameObject.SetActive(map.CanMove(Vector2Int.left));
        btnRight.gameObject.SetActive(map.CanMove(Vector2Int.right));
    }

    public void OnMoveUp() { Move(Vector2Int.up); }
    public void OnMoveDown() { Move(Vector2Int.down); }
    public void OnMoveLeft() { Move(Vector2Int.left); }
    public void OnMoveRight() { Move(Vector2Int.right); }

    void Move(Vector2Int dir)
    {
        if (!map.CanMove(dir)) return;

        Vector2Int next = map.playerPos + dir;
        map.movePlayer(next.x, next.y);

        UpdateButtons();
    }

    private void Start()
    {
        StartCoroutine(LateInit());
    }

    IEnumerator LateInit()
    {
        yield return null; 
        UpdateButtons();
    }
}
