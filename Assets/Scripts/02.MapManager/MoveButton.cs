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

    public void UpdateButtons()
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

        //한윤구 추가
        var CurrentRoom = map.GetCurrentRoom();
        int floor = MySceneManagement.CurrentFloor; //현재 층 번호 가져오기

        // BGM 변경
        SoundManager.Instance.StopBGM();
        Debug.Log("[MoveButton] Current room type: " + CurrentRoom.Type);

        switch (CurrentRoom.Type)
        {
            case RoomType.EventRoom:
                SoundManager.Instance.PlayEventRoomBGM();
                break;

            case RoomType.BossRoom:
                SoundManager.Instance.PlayBossBGM(floor);
                break;

            case RoomType.CombatRoom:
            default:
                SoundManager.Instance.PlayFloorBGM(floor);
                break;
        }

        // 전투 상태 설정
        if (CurrentRoom.Type == RoomType.CombatRoom || CurrentRoom.Type == RoomType.BossRoom)
        {
            GameManager.Instance.GameState = GameState.Combat;
        }

        UpdateButtons();

        //if (map.GetCurrentRoom().Type == RoomType.CombatRoom || map.GetCurrentRoom().Type == RoomType.BossRoom)
        //{
        //    GameManager.Instance.GameState = GameState.Combat;
        //}

        //UpdateButtons();
    }
    //한윤구 추가
    private IEnumerator PlayBossRoomDelayed(int floor)
    {
        yield return null; // 한 프레임 대기 (다른 BGM 호출이 끝난 뒤 실행)
        SoundManager.Instance.PlayBossBGM(floor);
    }

    private void Start()
    {
        //StartCoroutine(LateInit());
    }

    IEnumerator LateInit()
    {
        yield return null;
        UpdateButtons();
    }
}
