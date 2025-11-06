using UnityEngine;

public interface IEffectManager
{
    /// <summary>
    /// 특정 효과 재생 (예: 파티클, 프리팹, 애니메이션)
    /// </summary>
    void PlayEffect(string effectName, UnityEngine.Vector3 position);
    

    /// <summary>
    /// 특정 효과를 일정 시간 후 제거
    /// </summary>
    void StopEffect(string effectName);


    /// <summary>
    /// 모든 활성 효과 제거
    /// </summary>
    void ClearAllEffects();
}
