/*
 * 런타임 유닛 본체, 게임 내 모든 유닛(플레이어/적/소환수 등)의 런타임 스탯과 상태를 관리하는 핵심 클래스.
 */

using UnityEngine;

[DisallowMultipleComponent]
public class Unit : MonoBehaviour, IDamageable, IHealable, IAbilityUser
{
    [SerializeField] private UnitConfig config;    // 유닛의 기본 설정 (ScriptableObject)

    public UnitConfig Config => config;
    public int TeamId => config ? config.TeamId : 0;

    public StatBlock Stats { get; private set; }   // 현재 유닛의 스탯 상태
    public float CurrentHealth { get; private set; }
    public float CurrentMana { get; private set; }

    private float _skillTimer;

    // ===== Unity 생명주기 =====
    private void Awake()
    {
        // StatBlock 초기화
        Stats = new StatBlock();

        // SO에서 기본 스탯 복사
        CopyBases(config.EntryStats, Stats);

        // HP, MP 초기화
        CurrentHealth = Mathf.Max(1f, Stats.GetValue(StatKind.MaxHealth));
        CurrentMana = Mathf.Min(Stats.GetValue(StatKind.ManaMax),
                                Stats.GetValue(StatKind.ManaMax) * 0.2f); // 시작 MP 20%
    }

    private void Update()
    {
        // 마나 재생
        var regen = Stats.GetValue(StatKind.ManaRegenPerSec);
        if (regen > 0)
        {
            CurrentMana = Mathf.Min(
                Stats.GetValue(StatKind.ManaMax),
                CurrentMana + regen * Time.deltaTime
            );
        }

        // 스킬 자동 시전 / 쿨다운 처리
        _skillTimer += Time.deltaTime;
        TryCastSkill();
    }

    // ===== 스킬 발동 =====
    private void TryCastSkill()
    {
        if (Config.DefaultSkill == null) return;
        var skill = Config.DefaultSkill;

        if (skill.CastMode == SkillCastMode.OnManaFull)
        {
            if (CurrentMana >= skill.ManaCost)
            {
                skill.Execute(this);
                CurrentMana = 0f;
            }
        }
        else // AutoInterval
        {
            float cdr = Mathf.Clamp01(Stats.GetValue(StatKind.CooldownReducePct));
            float interval = Mathf.Max(0.1f, skill.IntervalSeconds * (1f - cdr));
            if (_skillTimer >= interval)
            {
                _skillTimer = 0f;
                skill.Execute(this);
            }
        }
    }

    public void GainManaOnHit()
    {
        float gain = Stats.GetValue(StatKind.ManaOnHit);
        if (gain > 0)
        {
            CurrentMana = Mathf.Min(Stats.GetValue(StatKind.ManaMax),
                                    CurrentMana + gain);
        }
    }

    // ===== 스탯 복사 =====
    private static void CopyBases(StatBlock from, StatBlock to)
    {
        foreach (StatKind k in System.Enum.GetValues(typeof(StatKind)))
        {
            float baseVal = from.GetBase(k);
            if (Mathf.Abs(baseVal) > 0f)
                to.SetBase(k, baseVal);
        }
    }

    // ===== 피해 / 회복 =====
    public void TakeDamage(float rawDamage, GameObject source)
    {
        float def = Mathf.Max(0f, Stats.GetValue(StatKind.Defense));
        float afterFlat = Mathf.Max(1f, rawDamage - def);

        float dr = Mathf.Clamp01(Stats.GetValue(StatKind.DamageReducePct));
        float final = afterFlat * (1f - dr);

        CurrentHealth -= final;
        if (CurrentHealth <= 0f)
            Die();
    }

    public void Heal(float amount, GameObject source)
    {
        if (amount <= 0f) return;
        CurrentHealth = Mathf.Min(Stats.GetValue(StatKind.MaxHealth),
                                  CurrentHealth + amount);
    }

    // ===== 리소스 소비 =====
    public bool SpendResource(float amount)
    {
        if (CurrentMana < amount) return false;
        CurrentMana -= amount;
        return true;
    }

    // ===== 사망 =====
    private void Die()
    {
        var sm = FindObjectOfType<SynergyManager>();
        if (sm) sm.Unregister(this); // 시너지 해제

        Destroy(gameObject);
    }
}