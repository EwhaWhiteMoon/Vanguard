using UnityEngine;

/// <summary>
/// 시각적 효과(이펙트) 관리 인터페이스.
/// - 유닛 피격 / 사망 이펙트 재생
/// - 이펙트 제어 및 초기화 기능 포함
/// </summary>
public interface IEffectManager
{
    /// <summary>지정된 이름의 이펙트를 특정 위치에서 재생</summary>
    void PlayEffect(string effectName, Vector3 position);

    /// <summary>현재 활성화된 모든 이펙트를 제거</summary>
    void ClearAllEffects();

    /// <summary>유닛 이벤트(OnDamaged, OnDied)를 이펙트 시스템에 등록</summary>
    void RegisterUnit(Unit unit, Transform visualRoot);

    /// <summary>유닛 이벤트 등록을 해제</summary>
    void UnregisterUnit(Unit unit);
}
