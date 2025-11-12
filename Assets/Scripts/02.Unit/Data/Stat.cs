using System;

[Serializable]
public class Stat
{
    public float MaxHealth = 100f,      // HP 최대치
        HealthRegenPerSec = 0f,         // 초당 HP 재생
        Attack = 10f,                   // 공격력
        Defense = 0f,                   // 기존 방어력(깡 감소)
        DamageReducePct = 0f,           // 피해감소 % (0~1)
        MoveSpeed = 1f,                 // 이동속도
        AttackSpeed = 1f,               // 공격속도(초당 공격)
        CritChance = 0f,                // 0~1
        CritMultiplier = 1.3f,          // ex) 1.5
        ManaMax = 100f,                 // MP 최대치
        ManaRegenPerHit = 10f,          // 히트당 MP 재생
        Range = 2f,                     // 사거리
        AggroValue = 0f;                // 어그로 수치

    public Stat() { }
    
    public Stat(Stat s)
    {
        MaxHealth = s.MaxHealth;
        HealthRegenPerSec = s.HealthRegenPerSec;
        Attack = s.Attack;
        Defense = s.Defense;
        DamageReducePct = s.DamageReducePct;
        MoveSpeed = s.MoveSpeed;
        AttackSpeed = s.AttackSpeed;
        CritChance = s.CritChance;
        CritMultiplier = s.CritMultiplier;
        ManaMax = s.ManaMax;
        ManaRegenPerHit = s.ManaRegenPerHit;
        Range = s.Range;
        AggroValue = s.AggroValue;
    }
}