/*
 * 런타임 유닛 본체
 */
using System;
using System.Collections.Generic;

[Serializable]
public class Unit
{
    public string Name;
    public int TeamId;
    public UnitClass UnitClass;
    public UnitGrade Grade;

    public StatBlock Stats = new();
    public float CurrentHealth;
    public float CurrentMana;

    // 생성자: UnitConfig 기반으로 데이터 초기화
    public Unit(UnitConfig config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        Name = config.Name;
        TeamId = config.TeamId;
        UnitClass = config.UnitClass;
        Grade = config.Grade;

        // 기본 스탯 복사 (CopyBaseFrom은 StatBlock에 구현됨)
        Stats.CopyBaseFrom(config.EntryStats);

        // HP / MP 초기화
        CurrentHealth = Math.Max(1f, Stats.GetValue(StatKind.MaxHealth));
        CurrentMana = 0f; // 시작값 정책은 게임디자인에 맞춰 조정
    }

    // 상태이상 런타임 인스턴스 관리
    private readonly List<StatusEffectInstance> _effects = new();
    public IReadOnlyList<StatusEffectInstance> Effects => _effects;

    public void ApplyStatus(StatusEffect effect, int stack = 1)
    {
        if (effect == null) return;
        var inst = _effects.Find(e => e.Effect == effect);
        if (inst != null && effect.IsStackable)
        {
            inst.Stack += stack;
            inst.Remaining = Math.Max(inst.Remaining, effect.Duration);
        }
        else
        {
            _effects.Add(new StatusEffectInstance(effect, stack, effect.Duration));
            effect.OnApply(this, stack);
        }
    }

    public void Tick(float dt)
    {
        // 재생(HP/MP), 쿨감, 패시브 등도 여기서 계산 가능
        for (int i = _effects.Count - 1; i >= 0; --i)
        {
            var e = _effects[i];
            e.Remaining -= dt;
            if (e.Remaining <= 0f)
            {
                e.Effect.OnExpire(this, e.Stack);
                _effects.RemoveAt(i);
            }
        }
    }

    // ==== 이벤트 & 상태 ====
    // 피격/사망 이벤트 훅: MonoBehaviour(이펙트/애니메이션)는 이걸 구독해 표현만 처리
    public event Action<Unit, DamageResult> OnDamaged;
    public event Action<Unit> OnDied;

    public bool IsDead => CurrentHealth <= 0f;

    // ==== 결과 전달용 구조체 ====
    public readonly struct DamageResult
    {
        public readonly float Before;
        public readonly float Applied;
        public readonly float After;
        public readonly bool Killed;

        public DamageResult(float before, float applied, float after, bool killed)
        {
            Before = before; Applied = applied; After = after; Killed = killed;
        }
    }

    // ==== 피격 처리 ====
    // rawDamage: 최종 계산된 피해량(음수/NaN 방지), onHitEffects: 피격 시 적용할 상태이상 목록
    public DamageResult TakeDamage(float rawDamage, IEnumerable<StatusEffect> onHitEffects = null, int stack = 1)
    {
        if (IsDead) return new DamageResult(CurrentHealth, 0f, CurrentHealth, true);

        // 1) 피해량 정규화
        var dmg = float.IsNaN(rawDamage) ? 0f : Math.Max(0f, rawDamage);
        var before = CurrentHealth;

        // 2) HP 감소
        CurrentHealth = Math.Max(0f, CurrentHealth - dmg);

        // 3) 피격 시 상태이상 적용
        if (onHitEffects != null)
        {
            foreach (var eff in onHitEffects)
            {
                if (eff != null) ApplyStatus(eff, stack);
            }
        }

        // 4) 이벤트/사망 처리
        var killed = CurrentHealth <= 0f;
        var result = new DamageResult(before, dmg, CurrentHealth, killed);
        OnDamaged?.Invoke(this, result);
        if (killed) OnDied?.Invoke(this);
        return result;
    }

    /*
    // 사용 예시
    var result = unit.TakeDamage(35f, new[]{ bleedEffect, slowEffect }, stack:1);
    */
}
