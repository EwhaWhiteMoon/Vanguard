using UnityEngine;

public class StartScreen : MonoBehaviour
{
    //한윤구 추가 
    //bgm 불러오기
    public void OnGameStateChange(GameState state)
    {
        bool isStartMenu = state == GameState.StartMenu;
        gameObject.SetActive(state == GameState.StartMenu); // Combat이라면 살아남

        if (isStartMenu)
        {
            SoundManager.Instance.PlayMenuBGM();
        }
        else
        {
            SoundManager.Instance.StopBGM();

            var mapManager = FindFirstObjectByType<MapManager>();
            int currentFloor = 1;

            if (mapManager != null)
            {
                // mapStatsPool과 currentStats를 직접 비교해서 인덱스 구하기
                var field = typeof(MapManager).GetField("mapStatsPool", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var statsArray = field?.GetValue(mapManager) as Object[];

                var currentField = typeof(MapManager).GetField("currentStats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var currentStats = currentField?.GetValue(mapManager);

                if (statsArray != null && currentStats != null)
                {
                    for (int i = 0; i < statsArray.Length; i++)
                    {
                        if (statsArray[i] == currentStats)
                        {
                            currentFloor = i + 1;
                            break;
                        }
                    }
                }
            }


            SoundManager.Instance.PlayFloorBGM(currentFloor);
        }
        
    
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