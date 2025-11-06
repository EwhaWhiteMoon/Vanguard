// StatBlock.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class StatBlock
{
    [Serializable]
    public struct Entry { public StatKind Kind; public float Base; }

    [SerializeField] private Entry[] _bases = Array.Empty<Entry>();
    private readonly Dictionary<StatKind, List<StatModifier>> _mods = new();

    public float GetBase(StatKind kind)
    {
        
        foreach (var e in _bases) if (e.Kind == kind) return e.Base;
        return 0f;
        
    }

    public void SetBase(StatKind kind, float value)
    {
        for (int i = 0; i < _bases.Length; i++)
        {
            if (_bases[i].Kind == kind) { _bases[i].Base = value; return; }
        }
        Array.Resize(ref _bases, _bases.Length + 1);
        _bases[^1] = new Entry { Kind = kind, Base = value };
    }

    public void AddModifier(StatModifier mod)
    {
        if (!_mods.TryGetValue(mod.Kind, out var list))
        {
            list = new List<StatModifier>();
            _mods[mod.Kind] = list;
        }
        list.Add(mod);
        list.Sort((a, b) => a.Order.CompareTo(b.Order));
    }

    public void RemoveModifier(StatModifier mod)
    {
        if (_mods.TryGetValue(mod.Kind, out var list))
            list.Remove(mod);
    }

    public float GetValue(StatKind kind)
    {
        float baseVal = GetBase(kind);
        if (!_mods.TryGetValue(kind, out var list) || list.Count == 0)
            return baseVal;

        float add = 0f;
        float mul = 1f;
        foreach (var m in list)
        {
            if (m.Op == StatModOp.Add) add += m.Value;
            else mul *= (1f + m.Value);
        }
        return (baseVal + add) * mul;
    }

    public void ClearAllModifiers() => _mods.Clear();

    public void CopyBaseFrom(StatBlock from)  //Unit.copyBase에서 이동 다른 statBlock의 base값 복사
    {
        if (from == null || from._bases == null) return;

        foreach (var entry in from._bases)
        {
            SetBase(entry.Kind, entry.Base);
        }
    }
}
