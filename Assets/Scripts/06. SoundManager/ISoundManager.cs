using UnityEngine;

/// <summary>
/// 사운드 시스템 인터페이스.
/// - 메뉴 / 층 / 방 / 이벤트 BGM 제어
/// - 효과음(SFX) 재생
/// - 유닛 이벤트 등록
/// </summary>
public interface ISoundManager
{
    /// <summary>메뉴 BGM을 재생</summary>
    void PlayMenuBGM();

    /// <summary>특정 층(테마)의 배경음악을 재생</summary>
    /// <param name="themeName">층(테마) 이름</param>
    void PlayBGM(string themeName);

    /// <summary>현재 층의 테마 안에서 방별 랜덤 BGM을 재생</summary>
    void SwitchRoomBGM();

    /// <summary>이벤트 방 전용 BGM을 재생</summary>
    void PlayEventRoomBGM();

    /// <summary>현재 재생 중인 BGM을 정지</summary>
    void StopBGM();

    /// <summary>특정 효과음을 재생</summary>
    /// <param name="soundName">효과음 이름 (예: "Hit", "Death")</param>
    void PlaySFX(string soundName);

    /// <summary>전체 볼륨을 설정</summary>
    void SetMasterVolume(float value);

    /// <summary>BGM 볼륨을 설정</summary>
    void SetBGMVolume(float value);

    /// <summary>SFX 볼륨을 설정</summary>
    void SetSFXVolume(float value);
}
