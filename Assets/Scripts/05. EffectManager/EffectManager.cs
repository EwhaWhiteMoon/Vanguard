using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전역 이펙트 매니저
/// - 유닛 이벤트 기반으로 피격 / 사망 시 자동 이펙트 재생
/// - MonoSingleton으로 전체 접근 가능
/// </summary>
/// 

/// <summary>
/// 사용 예시
/// 
/// 1. UnitFactory.cs
///     EffectManager.Instance.RegisterUnit(unit, behaviour.transform);
/// 
/// 2. Unit이 사망 시 → 자동으로 "Death" 이펙트 재생
/// 
/// 3. Unit이 피격 시 → 자동으로 "Hit" 이펙트 재생
/// </summary>
/// 

[DisallowMultipleComponent]
public class EffectManager : MonoSingleton<EffectManager>, IEffectManager
{
    [Header("Effect Prefabs")]
    [SerializeField] private GameObject hitEffectPrefab;    // 피격 시 이펙트 프리팹
    [SerializeField] private GameObject deathEffectPrefab;  // 사망 시 이펙트 프리팹

    private readonly List<GameObject> activeEffects = new(); // 활성화된 이펙트 목록

    /// <summary>특정 위치에 이펙트를 생성하고 일정 시간 후 자동 파괴</summary>
    public void PlayEffect(string effectName, Vector3 position)
    {

        GameObject prefab = effectName switch
        {
            "Hit" => hitEffectPrefab,
            "Death" => deathEffectPrefab,
            _ => null
        };
        if (prefab == null) return;

        GameObject fx = Object.Instantiate(prefab, position, Quaternion.identity);
        activeEffects.Add(fx);
        Object.Destroy(fx, 1.5f); // 1.5초 후 자동 제거
    }

    /// <summary>모든 활성화된 이펙트를 제거</summary>
    public void ClearAllEffects()
    {
        foreach (var fx in activeEffects)
        {
            if (fx != null) Object.Destroy(fx);
        }
        activeEffects.Clear();
    }

    /*

    /// <summary> 유닛의 OnDamaged, OnDied 이벤트에 이펙트를 등록 </summary>
    public void RegisterUnit(Unit unit, Transform visualRoot)
    {
        if (unit == null || visualRoot == null) return;
        unit.OnDamaged += (_, _) => PlayEffect("Hit", visualRoot.position);
        unit.OnDied += (_) => PlayEffect("Death", visualRoot.position);
    }

    /// <summary>유닛 이벤트 등록을 해제</summary>
    public void UnregisterUnit(Unit unit)
    {
        if (unit == null) return;
        unit.OnDamaged -= (_, _) => { };
        unit.OnDied -= (_) => { };
    }
    */

}
