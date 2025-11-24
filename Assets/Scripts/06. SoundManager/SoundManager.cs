using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전역 사운드 매니저
/// - MonoSingleton으로 전체 접근 가능
/// - 메뉴, 층, 방, 이벤트 BGM 관리
/// - BGM 전환 시 페이드인/페이드아웃 적용
/// </summary>

/// <summary>
/// 사용 예시
/// 
/// 1. GameManager.cs
///     SoundManager.Instance.PlayMenuBGM();  // 메뉴 진입 시
/// 
/// 2. MapManager.cs
///     SoundManager.Instance.PlayFloorBGM(1);      // 1층 진입 시
///     SoundManager.Instance.SwitchRoomBGM();      // 방 이동 시 (자동 페이드 전환)
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
    [SerializeField] private AudioClip stepClip;      // 유닛 이동시 발걸음 효과음
    [SerializeField] private AudioClip adcAttackClip; // 원거리 유닛 투사체 발사
    [SerializeField] private AudioClip getRewardClip; // 리워드 선택 효과음

    private List<AudioClip> currentFloorBGMs = new();
    private List<AudioClip> bossBGMs = new();
    private List<AudioClip> eventRoomBGMs = new();
    private int currentFloor = 1;
    private Coroutine fadeCoroutine;

    // ================================================================
    //  BGM 관련
    // ================================================================

    /// <summary>메뉴 BGM을 재생</summary>
    public void PlayMenuBGM()
    {
        if (menuBGM == null) return;
        StartFadeBGM(menuBGM, 1f);
    }

    /// <summary>특정 층의 랜덤한 BGM을 재생</summary>
    public void PlayFloorBGM(int floor)
    {
        currentFloor = floor;
        currentFloorBGMs = LoadBGMClips($"Sound/BGM/Floor{floor}");
        if (currentFloorBGMs.Count == 0)
        {
            Debug.LogWarning($"[SoundManager] No BGMs found for Floor {floor}");
            return;
        }
        PlayRandomBGMFromList(currentFloorBGMs);
    }

    /// <summary>현재 층의 테마 안에서 방별 랜덤 BGM을 재생</summary>
    public void SwitchRoomBGM()
    {
        if (currentFloorBGMs.Count == 0)
        {
            currentFloorBGMs = LoadBGMClips($"Sound/BGM/Floor{currentFloor}");
        }
        PlayRandomBGMFromList(currentFloorBGMs);
    }

    /// <summary>보스방 BGM을 재생</summary>
    public void PlayBossBGM(int floor)
    {
        bossBGMs = LoadBGMClips("Sound/BGM/BossRoom"); // 폴더 이름 일치
        if (bossBGMs == null || bossBGMs.Count == 0)
        {
            Debug.LogWarning("[SoundManager] Boss BGM folder empty!");
            return;
        }

        // floor이 리스트보다 크면 마지막 곡을 재생하도록 처리
        int index = Mathf.Clamp(floor - 1, 0, bossBGMs.Count - 1);
        var clip = bossBGMs[index];
        Debug.Log($"[SoundManager] Playing Boss BGM for floor {floor}: {clip.name}");
        StartFadeBGM(clip, 1f);

        Debug.Log($"[SoundManager] bgmSource clip: {bgmSource.clip}, volume: {bgmSource.volume}, isPlaying: {bgmSource.isPlaying}");
    }

    /// <summary>이벤트 방 전용 BGM을 재생</summary>
    public void PlayEventRoomBGM()
    {
        eventRoomBGMs = LoadBGMClips("Sound/BGM/EventRoom");
        if (eventRoomBGMs.Count == 0) return;
        StartFadeBGM(eventRoomBGMs[Random.Range(0, eventRoomBGMs.Count)], 1f);
    }

    /// <summary>현재 재생 중인 BGM을 정지</summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ================================================================
    //  SFX 관련
    // ================================================================
    public void PlaySFX(string sfxName)
    {
        AudioClip clip = sfxName switch
        {
            "Hit" => hitClip,
            "Death" => deathClip,
            "Step" => stepClip,
            "ADCAttack" => adcAttackClip,
            "GetReward" => getRewardClip,
            _ => null
        };
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // ================================================================
    //  볼륨 설정 관련
    // ================================================================
    public void SetMasterVolume(float value) => AudioListener.volume = value;
    public void SetBGMVolume(float value) => bgmSource.volume = value;
    public void SetSFXVolume(float value) => sfxSource.volume = value;

    private void Awake()
    {
        if (sfxSource != null) sfxSource.volume = 0.1f; // SFX 50%
    }

    // ================================================================
    //  내부 기능
    // ================================================================
    private void PlayRandomBGMFromList(List<AudioClip> list)
    {
        if (list == null || list.Count == 0) return;
        var clip = list[Random.Range(0, list.Count)];
        StartFadeBGM(clip, 1f);
    }

    private List<AudioClip> LoadBGMClips(string path)
    {
        var clips = new List<AudioClip>(Resources.LoadAll<AudioClip>(path));
        if (clips.Count == 0)
        {
            Debug.LogWarning($"[SoundManager] No clips found at path: {path}");
        }
        else
        {
            clips.Sort((a, b) => string.CompareOrdinal(a.name, b.name)); // Floor1_1 ~ Floor1_4 순서 유지
        }
        return clips;
    }

    // ================================================================
    //  페이드 기능
    // ================================================================
    private void StartFadeBGM(AudioClip newClip, float duration)
    {
        if (bgmSource.clip == newClip) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        fadeCoroutine = StartCoroutine(FadeBGM(newClip, duration));
    }

    private IEnumerator FadeBGM(AudioClip newClip, float duration)
    {
        if (bgmSource.clip == newClip)
            yield break;

        float startVolume = bgmSource.volume;
        float fadeTime = duration * 0.5f;

        // 페이드 아웃
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
        bgmSource.Stop();

        bgmSource.clip = newClip;
        yield return null;
        bgmSource.clip = newClip;
        bgmSource.volume = 0;
        bgmSource.Play();
        Debug.Log($"[SoundManager] Now playing new clip: {bgmSource.clip.name}, isPlaying: {bgmSource.isPlaying}");

        // 페이드 인
        float fadeInTime = duration * 0.5f;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = startVolume;
    }
}
