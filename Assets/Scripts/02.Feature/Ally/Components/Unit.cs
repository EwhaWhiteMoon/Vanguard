/*
 * ��Ÿ�� ���� ��ü, ���� �� ��� ����(�÷��̾�/��/��ȯ�� ��)�� ��Ÿ�� ���Ȱ� ���¸� �����ϴ� �ٽ� Ŭ����.
 */

using UnityEngine;

[DisallowMultipleComponent]
public class Unit : MonoBehaviour//, IDamageable, IHealable, IAbilityUser
{
    [SerializeField] private UnitConfig config;    // ������ �⺻ ���� (ScriptableObject)

    public UnitConfig Config => config;
    public int TeamId => config ? config.TeamId : 0;

    public StatBlock Stats { get; private set; }   // ���� ������ ���� ����
    public float CurrentHealth { get; private set; }
    public float CurrentMana { get; private set; }

    private float _skillTimer;

    // ===== Unity �����ֱ� =====
    private void Awake()
    {
        // StatBlock �ʱ�ȭ
        Stats = new StatBlock();

        // SO���� �⺻ ���� ����
        CopyBases(config.EntryStats, Stats);

        // HP, MP �ʱ�ȭ
        CurrentHealth = Mathf.Max(1f, Stats.GetValue(StatKind.MaxHealth));
        CurrentMana = Mathf.Min(Stats.GetValue(StatKind.ManaMax),
                                Stats.GetValue(StatKind.ManaMax) * 0.2f); // ���� MP 20%
    }

    private void Update()
    {
        // ���� ���
        var regen = Stats.GetValue(StatKind.ManaRegenPerSec);
        if (regen > 0)
        {
            CurrentMana = Mathf.Min(
                Stats.GetValue(StatKind.ManaMax),
                CurrentMana + regen * Time.deltaTime
            );
        }

        // ��ų �ڵ� ���� / ��ٿ� ó��
        _skillTimer += Time.deltaTime;
        TryCastSkill();
    }

    // ===== ��ų �ߵ� =====
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

    // ===== ���� ���� =====
    private static void CopyBases(StatBlock from, StatBlock to)
    {
        foreach (StatKind k in System.Enum.GetValues(typeof(StatKind)))
        {
            float baseVal = from.GetBase(k);
            if (Mathf.Abs(baseVal) > 0f)
                to.SetBase(k, baseVal);
        }
    }

    // ===== ���� / ȸ�� =====
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

    // ===== ���ҽ� �Һ� =====
    public bool SpendResource(float amount)
    {
        if (CurrentMana < amount) return false;
        CurrentMana -= amount;
        return true;
    }

    // ===== ��� =====
    private void Die()
    {
        /*
        var sm = FindObjectOfType<SynergyManager>();
        if (sm) sm.Unregister(this); // �ó��� ����

        Destroy(gameObject);
        */
    }
}