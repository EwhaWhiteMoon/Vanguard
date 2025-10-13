// 작성자 : 김도건
// 마지막 수정 : 2025.10.11.
// 게임의 현재 상태를 나타내는 Enum

public enum GameState
{
    Loading, // 게임 시작 화면 띄우기 전 상태. 실제 사용 x
    StartMenu, // 게임 시작 전 화면
    PauseMenu, // 게임 정지 시 화면
    BeforeCombat, // 전투 이전 화면
    Combat, // 전투 중 화면
    AfterCombat, // 전투 이후 화면
    Event, // 이벤트 진행 중 화면
    Map, // 맵 이동 중 화면
}