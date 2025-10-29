using System.Collections.Generic;
using UnityEngine;

public class SynergyManager : MonoBehaviour
{
    [SerializeField] private SynergyDefinition[] definitions;

    private readonly Dictionary<UnitClass, SynergyDefinition> _map = new();
    private readonly Dictionary<UnitClass, int> _counts = new();
    private readonly Dictionary<(Unit, UnitClass), List<StatModifier>> _applied = new();
    private readonly HashSet<Unit> _units = new(); // 추적 중인 유닛 집합

    private void Awake()
    {
        // SynergyDefinition 매핑 초기화
        foreach (var def in definitions) _map[def.Class] = def;
    }


    // 새 유닛이 씬에 추가될 때 호출
    public void Register(Unit unit)
    {
        if (!_units.Contains(unit))
            _units.Add(unit); // 유닛 추적 집합에 추가

        var cls = unit.Config.UnitClass; // 유닛 클래스 가져오기
        _counts.TryGetValue(cls, out var c); // _counts에서 현재 유닛의 카운트 가져오기
        _counts[cls] = c + 1; // 카운트 증가

        Reapply(cls); // 시너지 효과 재적용
    }

    // 유닛이 씬에서 제거될 때 호출
    public void Unregister(Unit u)
    {
        if (_units.Contains(u))
            _units.Remove(u); // 유닛 추적 집합에서 제거
        
        var cls = u.Config.UnitClass; // 유닛 클래스 가져오기

        if (_counts.TryGetValue(cls, out var c))
            _counts[cls] = Mathf.Max(0, c - 1); // 하나 줄임 (0 아래로는 안되려고 안전장치 걸어둠)

        // 이 유닛에게 적용되어 있던 시너지 효과 제거
        if (_applied.TryGetValue((u, cls), out var mods))
        {
            foreach (var m in mods)
                u.Stats.RemoveModifier(m); // 스탯 버프 제거

            _applied.Remove((u, cls));
        }
        
        Reapply(cls); // 시너지 효과 재적용
    }


    // 시너지 효과 재적용
    private void Reapply(UnitClass cls)
    {
        if (!_map.TryGetValue(cls, out var def)) return;
        _counts.TryGetValue(cls, out var count);


        // 등록된 유닛에 대해 시너지 효과 적용
        foreach (var u in _units)
        {
            if (u.Config.UnitClass != cls) continue;

            var key = (u, cls);
            if (_applied.TryGetValue(key, out var prevMods))
            {
                foreach (var m in prevMods)
                    u.Stats.RemoveModifier(m);
                prevMods.Clear();
            }
            else
            {
                prevMods = new List<StatModifier>();
                _applied[key] = prevMods;
            }

            // 티어 조건에 맞는 새 시너지 효과 적용
            SynergyDefinition.Tier? tier = null;
            foreach (var t in def.Tiers)
                if (count >= t.CountRequired) tier = t;

            if (tier.HasValue)
            {
                foreach (var m in tier.Value.Modifiers)
                {
                    u.Stats.AddModifier(m);
                    prevMods.Add(m);
                }
            }
        }
    }
}
