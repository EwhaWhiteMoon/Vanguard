using System;
using UnityEngine;

[Serializable]
public struct ItemStat
{
    public ItemStatKind Kind;
    public float Value;
    public bool IsPercent;

    public ItemStat(ItemStatKind kind, float value, bool isPercent)
    {
        Kind = kind;
        Value = value;
        IsPercent = isPercent;
    }
}
