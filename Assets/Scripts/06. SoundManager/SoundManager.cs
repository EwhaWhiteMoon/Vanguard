using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전역 사운드 매니저
/// - MonoSingleton으로 전체 접근 가능
/// - 메뉴, 층, 방, 이벤트 BGM 관리
/// - 유닛의 OnDamaged/OnDied 이벤트 기반 SFX 자동 재생
/// </summary>
/// 

/// <summary>
/// 사용 예시
/// 
/// 1. GameManager.cs
///     SoundManager.Instance.PlayMenuBGM();  // 메뉴 진입 시
/// 
/// 2. MapManager.cs
///     SoundManager.Instance.PlayBGM("Adventure"); // 층 진입
///     SoundManager.Instance.SwitchRoomBGM();      // 방 이동
///     SoundManager.Instance.PlayEventRoomBGM();   // 이벤트 방
///
/// 3. UnitFactory.cs
///     SoundManager.Instance.RegisterUnit(unit);   // 유닛 이벤트 등록
/// </summary>

[DisallowMultipleComponent]
public class SoundManager : MonoSingleton<SoundManager>, ISoundManager
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;   // BGM 재생 전용 오디오 소스
    [SerializeField] private AudioSource sfxSource;   // SFX 재생 전용 오디오 소스

    [Header("Menu Music")]
    [SerializeField] private AudioClip menuBGM;       // 메뉴 화면용 BGM

    [Header("SFX Clips")]
    [SerializeField] private AudioClip hitClip;       // 유닛 피격 효과음
    [SerializeField] private AudioClip deathClip;     // 유닛 사망 효과음

    [Header("BGM Themes (층별)")]
    [SerializeField] private List<BGMTheme> bgmThemes; // 층별 테마 리스트
    private BGMTheme currentTheme;                     // 현재 활성화된 테마

    [Header("Event Room BGM")]
    [SerializeField] private List<AudioClip> eventRoomBGMs; // 이벤트 방 전용 BGM 리스트

    
    // BGM 관련
    /// <summary>메뉴 BGM을 재생</summary>
    public void PlayMenuBGM()
    {
        if (menuBGM == null) return;
        bgmSource.clip = menuBGM;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>층(테마) 이름으로 BGM 테마를 찾아 재생</summary>
    public void PlayBGM(string themeName)
    {
        currentTheme = bgmThemes.Find(t => t.ThemeName == themeName);
        if (currentTheme == null)
        {
            Debug.LogWarning($"[SoundManager] Theme '{themeName}' not found!");
            return;
        }
        PlayRandomBGMFromTheme();
    }

    /// <summary>현재 테마 안에서 랜덤한 방 BGM을 재생</summary>
    public void SwitchRoomBGM()
    {
        PlayRandomBGMFromTheme();
    }

    /// <summary>이벤트 방 전용 BGM을 재생</summary>
    public void PlayEventRoomBGM()
    {
        if (eventRoomBGMs.Count == 0) return;
        var clip = eventRoomBGMs[Random.Range(0, eventRoomBGMs.Count)];
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>현재 재생 중인 BGM을 정지</summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }


    // 🔊 SFX 관련
    /// <summary>효과음 이름으로 해당 SFX를 재생</summary>
    public void PlaySFX(string soundName)
    {
        AudioClip clip = soundName switch
        {
            "Hit" => hitClip,
            "Death" => deathClip,
            _ => null
        };
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }


    // 볼륨 설정 관련

    public void SetMasterVolume(float value) => AudioListener.volume = value;
    public void SetBGMVolume(float value) => bgmSource.volume = value;
    public void SetSFXVolume(float value) => sfxSource.volume = value;

    // 내부 기능
    private void PlayRandomBGMFromTheme()
    {
        if (currentTheme == null || currentTheme.BGMs.Count == 0) return;
        var clip = currentTheme.BGMs[Random.Range(0, currentTheme.BGMs.Count)];
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    
}

/// <summary>
/// 층별 테마 데이터 구조
/// </summary>
[System.Serializable]
public class BGMTheme
{
    public string ThemeName;       // ex: "Adventure", "Happiness"
    public List<AudioClip> BGMs;   // 각 방마다 다른 곡들
}


