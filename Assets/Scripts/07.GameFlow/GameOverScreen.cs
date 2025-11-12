using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public void OnGameStateChange(GameState state)
    {
        gameObject.SetActive(state == GameState.GameOver);
    }
    
    //TODO:게임을 어케 첨으로 잘 돌리는 그런 메커니즘 탑재 바람...
    private void Awake()
    {
        GameManager.Instance.OnGameStateChange += OnGameStateChange;
        gameObject.SetActive(false); // 구독하고, 일단 필요해질때까지 꺼두기.
        OnGameStateChange(GameManager.Instance.GameState); //지금 State에 맞게 한번 호출해줘야함.
    }
}