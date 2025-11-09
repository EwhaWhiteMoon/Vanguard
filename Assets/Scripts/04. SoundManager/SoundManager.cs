using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 전역 사운드 관리 매니저
/// - BGM 및 SFX(AudioClip) 재생 및 제어 담당.
/// - MonoSingleton<T> 기반으로 어느 씬에서도 접근 가능.
/// - BGM/SFX를 각각 별도의 AudioSource로 관리.
/// </summary>
public class SoundManager : MonoSingleton<SoundManager>, ISoundManager
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; // 배경음 재생용 AudioSource
    [SerializeField] private AudioSource sfxSource; // 효과음 재생용 AudioSource

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> audioClips = new(); // 인스펙터에서 등록 가능한 사운드 목록

    // 클립 이름으로 빠르게 찾기 위한 Dictionary
    private readonly Dictionary<string, AudioClip> _clipMap = new();

    /// <summary>
    /// Start 시점에 등록된 모든 AudioClip을 딕셔너리에 저장.
    /// </summary>
    private void Start()
    {
        foreach (var clip in audioClips)
        {
            if (!_clipMap.ContainsKey(clip.name))
                _clipMap.Add(clip.name, clip);
        }
    }

    /// <summary>
    /// 지정된 이름의 배경음(BGM)을 재생.
    /// </summary>
    public void PlayBGM(string clipName, float volume = 1f, bool loop = true)
    {
        if (!_clipMap.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"[SoundManager] BGM '{clipName}' not found!");
            return;
        }

        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    /// <summary>
    /// 지정된 이름의 효과음(SFX)을 재생.
    /// </summary>
    public void PlaySFX(string clipName, float volume = 1f)
    {
        if (_clipMap.TryGetValue(clipName, out var clip))
            sfxSource.PlayOneShot(clip, volume);
        else
            Debug.LogWarning($"[SoundManager] SFX '{clipName}' not found!");
    }

    /// <summary>
    /// 현재 재생 중인 BGM을 중지.
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    /// <summary>
    /// 전체 마스터 볼륨(전체 오디오)에 영향을 줌.
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// BGM 전용 볼륨 조정.
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// SFX 전용 볼륨 조정.
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 런타임 중 새로운 오디오 클립을 등록.
    /// </summary>
    public void RegisterClip(AudioClip clip)
    {
        if (clip == null) return;
        if (!_clipMap.ContainsKey(clip.name))
            _clipMap.Add(clip.name, clip);
    }
}
