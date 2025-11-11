using System;

[Serializable]
public class Stat
{
    public float MaxHealth = 100f,
        Attack = 10f,
        Defense = 0f, // 기존 방어력(깡 감소)
        DamageReducePct = 0f, // 피해감소 % (0~1)
        MoveSpeed = 1f,
        AttackSpeed = 1f, // 공격속도(초당 공격)
        CritChance = 0.3f, // 0~1
        CritMultiplier = 1.5f, // ex) 1.5
        ManaMax = 100f, // MP 최대치
        ManaRegenPerSec = 10f, // 초당 MP 재생
        Range = 2f;

    public Stat(Stat s)
    {
        MaxHealth = s.MaxHealth;
        Attack = s.Attack;
        Defense = s.Defense;
        DamageReducePct = s.DamageReducePct;
        MoveSpeed = s.MoveSpeed;
        AttackSpeed = s.AttackSpeed;
        CritChance = s.CritChance;
        CritMultiplier = s.CritMultiplier;
        ManaMax = s.ManaMax;
        ManaRegenPerSec = s.ManaRegenPerSec;
        Range = s.Range;
    }
}