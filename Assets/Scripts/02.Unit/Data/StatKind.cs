using UnityEngine;

public enum StatKind
{
    MaxHealth,              // HP 최대치
    HealthRegenPerSec,      // 초당 HP 재생
    Attack,                 // 공격력
    Defense,                // 기존 방어력(깡 감소)
    DamageReducePct,        // 피해감소 % (0~1)
    MoveSpeed,              // 이동속도
    AttackSpeed,            // 공격속도(초당 공격)
    CritChance,             // 0~1
    CritMultiplier,         // ex) 1.5
    ManaMax,                // MP 최대치
    ManaRegenPerHit,        // 히트당 MP 재생
    Range,                  // 사거리
    AggroValue,            // 어그로 수치
}

