using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public void OnGameStateChange(GameState state)
    {
        gameObject.SetActive(state == GameState.StartMenu); // Combat이라면 살아남
    }

    public void StartGame()
    {
        GameManager.Instance.GameState = GameState.Combat;
    }
    
    private void Awake()
    {
        GameManager.Instance.OnGameStateChange += OnGameStateChange;
        gameObject.SetActive(false); // 구독하고, 일단 필요해질때까지 꺼두기.
        OnGameStateChange(GameManager.Instance.GameState); //지금 State에 맞게 한번 호출해줘야함.
    }
}