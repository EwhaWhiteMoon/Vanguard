using UnityEngine;

/// <summary>
/// 사운드 재생 및 관리 기능을 제공하는 인터페이스
/// </summary>

public interface ISoundManager
{
    ///<summary>
    ///배경음악 재생
    /// </summary>
    /// <param name = "ClipName"> 오디오 클립이름</param>
    /// <param name="volume">볼륨 (0~1)</param>
    /// <param name="loop">반복 여부</param>
    void PlayBGM(string clipName, float volume = 1f, bool loop = true);
    
    
    /// <summary>
    /// 효과음(SFX) 재생.
    /// </summary>
    /// <param name="clipName">오디오 클립 이름</param>
    /// <param name="volume">볼륨 (0~1)</param>
    void PlaySFX(string ClipName, float volume = 1f);


    /// <summary>
    /// 현재 재생 중인 BGM 정지.
    /// </summary>
    void StopBGM();


    /// <summary>
    /// 전체 음량 제어.
    /// </summary>
    void SetMasterVolume(float volume);


    /// <summary>
    /// BGM 볼륨 조정.
    /// </summary>
    void SetBGMVolume(float volume);


    /// <summary>
    /// SFX 볼륨 조정.
    /// </summary>
    void SetSFXVolume(float volume);


    /// <summary>
    /// 오디오 클립 미리 등록.
    /// </summary>
    /// <param name="clip">AudioClip</param>
    void RegisterClip(AudioClip clip);
}
