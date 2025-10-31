/*
 * 런타임 유닛 본체
 */

using UnityEngine;

[DisallowMultipleComponent]
public class Unit : MonoBehaviour
{
    [SerializeField] private UnitConfig config;    // 유닛의 원본 데이터 (ScriptableObject)

    public UnitConfig Config => config;
    public int TeamId => config ? config.TeamId : 0;

    public StatBlock Stats { get; private set; }   // 실시간 스탯상태 (버프 포함)
    public float CurrentHealth { get; private set; }  // 현재 hp
    public float CurrentMana { get; private set; }  //현재 mp

    private float _skillTimer;  //스킬 자동 시전용 타이머

    private void Awake()
    {
        // StatBlock 초기화
        Stats = new StatBlock();

        // SO에서 기본스탯 복사
        CopyBases(config.EntryStats, Stats);

        // HP, MP 초기화
        CurrentHealth = Mathf.Max(1f, Stats.GetValue(StatKind.MaxHealth));
        CurrentMana = Mathf.Min(Stats.GetValue(StatKind.ManaMax),
                                Stats.GetValue(StatKind.ManaMax) * 0.2f); // 시작 MP 20%(임의 값)
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

        // 스킬자동 시전 / 쿨다운 처리
        _skillTimer += Time.deltaTime;
        TryCastSkill();
    }

    // ===== 스킬발동 =====
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

    public void GainManaOnHit()  //공격성공시 MP 획득
    {
        /*
        float gain = Stats.GetValue(StatKind.ManaOnHit);
        if (gain > 0)
        {
            CurrentMana = Mathf.Min(Stats.GetValue(StatKind.ManaMax),
                                    CurrentMana + gain);
        }
        */
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
    public void TakeDamage(float rawDamage, GameObject source)  // 방어력 /감소율 계산 후 HP차감
    {
        float def = Mathf.Max(0f, Stats.GetValue(StatKind.Defense));
        float afterFlat = Mathf.Max(1f, rawDamage - def);

        float dr = Mathf.Clamp01(Stats.GetValue(StatKind.DamageReducePct));
        float final = afterFlat * (1f - dr);

        CurrentHealth -= final;
        if (CurrentHealth <= 0f)
            Die();
    }

    public void Heal(float amount, GameObject source)  //HP 회복
    {
        if (amount <= 0f) return;
        CurrentHealth = Mathf.Min(Stats.GetValue(StatKind.MaxHealth),
                                  CurrentHealth + amount);
    }

    // ===== 리소스 소비 =====
    public bool SpendResource(float amount)  //MP차감
    {
        if (CurrentMana < amount) return false;
        CurrentMana -= amount;
        return true;
    }

    // ===== 사망  =====
    private void Die()
    {
        /*
        var sm = FindObjectOfType<SynergyManager>();
        if (sm) sm.Unregister(this); // 시너지 해제

        Destroy(gameObject);
        */
    }
}