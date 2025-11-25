// 작성자 : 김도건
// 마지막 수정 : 2025.10.13
// 게임 전반 진행을 담당하는 클래스.

using System;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameState gameState = GameState.Loading;

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
            Debug.Log("Loading Done...");
            GameState = GameState.StartMenu;
        }
    }
}
