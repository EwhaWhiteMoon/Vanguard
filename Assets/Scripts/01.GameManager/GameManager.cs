// 작성자 : 김도건
// 마지막 수정 : 2025.10.13
// 게임 전반 진행을 담당하는 클래스.

using System;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameState gameState = GameState.Loading;
    [SerializeField] private bool isBossCleared = false;

    public bool IsBossCleared
    {
        get { return isBossCleared; }
        set { isBossCleared = value; }
    }

    public GameState GameState
    {
        get => gameState;
        set
        {
            gameState = value;
            OnGameStateChange?.Invoke(value);
        }
    }

    public event Action<GameState> OnGameStateChange; // GameState가 변경되면 호출할 함수

    private void Start()
    {
        GameState = GameState.Loading;
    }

    private void Update()
    {
        if (GameState == GameState.Loading)
        {

            FindFirstObjectByType<MapManager>().InitMap();
            FindFirstObjectByType<MiniMap>().DrawMiniMap();
            FindFirstObjectByType<ItemBonusManager>().ResetBonuses();
            FindFirstObjectByType<CombatManager>().ResetRoaster();
            Debug.Log("Loading Done...");
            GameState = GameState.StartMenu;
        }
    }

    public void ResetGame()
    {
        // 런 단위 데이터 초기화 (골드, 인벤토리/시너지 등)
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.ResetGold();
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetAll();
        }

        GameState = GameState.Loading;
    }
}
