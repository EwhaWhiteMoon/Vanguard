using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 전역 이펙트(파티클, 프리팹) 관리 매니저
/// - 지정된 이름의 프리팹을 씬 상에 생성하고 자동 제거.
/// - 시각적 피드백(폭발, 피격 효과 등)을 담당.
/// - MonoSingleton<T> 기반으로 전역 접근 가능.
/// </summary>
public class EffectManager : MonoSingleton<EffectManager>, IEffectManager
{
    [Header("Effect Prefabs")]
    [SerializeField] private List<GameObject> effectPrefabs = new(); // 미리 등록된 이펙트 프리팹 목록

    // 이름으로 빠르게 검색할 수 있도록 Dictionary로 캐싱
    private readonly Dictionary<string, GameObject> _prefabMap = new();
    private readonly List<GameObject> _activeEffects = new(); // 현재 활성화된 이펙트 목록

    /// <summary>
    /// Start 시점에 effectPrefabs 리스트를 딕셔너리에 등록.
    /// </summary>
    private void Start()
    {
        foreach (var fx in effectPrefabs)
        {
            if (!_prefabMap.ContainsKey(fx.name))
                _prefabMap.Add(fx.name, fx);
        }
    }

    /// <summary>
    /// 지정된 이름의 이펙트를 특정 위치와 회전값으로 재생.
    /// </summary>
    public void PlayEffect(string effectName, Vector3 position, Quaternion rotation = default)
    {
        if (!_prefabMap.TryGetValue(effectName, out var prefab))
        {
            Debug.LogWarning($"[EffectManager] Effect '{effectName}' not found!");
            return;
        }

        var instance = Instantiate(prefab, position, rotation);
        _activeEffects.Add(instance);

        // ParticleSystem이 있으면 재생 후 자동 제거
        if (instance.TryGetComponent(out ParticleSystem ps))
            Destroy(instance, ps.main.duration + ps.main.startLifetime.constantMax);
        else
            Destroy(instance, 3f); // 기본 3초 후 제거
    }

    /// <summary>
    /// 특정 이름의 이펙트를 찾아 제거.
    /// </summary>
    public void StopEffect(string effectName)
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            var fx = _activeEffects[i];
            if (fx == null) { _activeEffects.RemoveAt(i); continue; }

            if (fx.name.Contains(effectName))
            {
                Destroy(fx);
                _activeEffects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 모든 활성화된 이펙트를 제거.
    /// </summary>
    public void ClearAllEffects()
    {
        foreach (var fx in _activeEffects)
        {
            if (fx != null) Destroy(fx);
        }
        _activeEffects.Clear();
    }

    /// <summary>
    /// 런타임 중 새로운 이펙트 프리팹을 등록.
    /// </summary>
    public void RegisterEffect(GameObject prefab)
    {
        if (prefab == null) return;
        if (!_prefabMap.ContainsKey(prefab.name))
            _prefabMap.Add(prefab.name, prefab);
    }
}
