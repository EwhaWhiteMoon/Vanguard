using System;

public enum StatModOp { Add, Mul } // Add: 더하기, Mul: (1+x) 곱

[Serializable]
public struct StatModifier
{
    public StatKind Kind;
    public StatModOp Op;
    public float Value;     // Add: 절대값, Mul: 예 0.2f -> +20%
    public int Order;       // 적용 순서(정렬용). 낮을수록 먼저 적용.

    public StatModifier(StatKind kind, StatModOp op, float value, int order = 0)
    {
        Kind = kind; Op = op; Value = value; Order = order;
    }
}