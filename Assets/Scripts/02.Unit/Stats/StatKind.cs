using UnityEngine;

public enum StatKind
{
    MaxHealth,
    Attack,
    Defense,                // 기존 방어력(깡 감소)
    DamageReducePct,        // 피해감소 % (0~1)
    MoveSpeed,
    AttackSpeed,            // 공격속도(초당 공격)
    CritChance,             // 0~1
    CritMultiplier,         // ex) 1.5
    ManaMax,                // MP 최대치
    ManaRegenPerSec,        // 초당 MP 재생(옵션)
    CooldownReducePct,      // 쿨감 % (0~1)
    StatusPower,            // 상태이상 강도(스턴/슬로우 등
    Range // 사거리
}

