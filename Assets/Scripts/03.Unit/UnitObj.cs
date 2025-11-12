using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitObj : MonoBehaviour
{
    public UnitData unitData;
    public Stat stat;
    public int Team;

    private float _hp;

    public float HP
    {
        get => _hp;
        set
        {
            _hp = value;
            if(HPText)
                HPText.text = _hp.ToString("N0");
        }
    }
    public TextMeshPro HPText;
    public ICombatManager combatManager;
    public SpriteRenderer spriteRenderer;

    public void Init(UnitData data, int team, ICombatManager combatManager, float HP = -1)
    {
        unitData = data;
        spriteRenderer.sprite = unitData.sprite;
        Team = team;
        this.combatManager = combatManager;

        // 스탯 설정해야함.
        this.stat = new Stat(unitData.BaseStat);
        this.HP = HP == -1 ? unitData.BaseStat.MaxHealth : HP;
    }

    public void Attack(UnitObj target)
    {
        target.TakeDamage(stat.Attack * (Random.Range(0f, 1f) < stat.CritChance ? stat.CritMultiplier : 1f));
    }
    
    public void TakeDamage(float damage)
    {
        HP -= (damage - stat.Defense) * (1 - stat.DamageReducePct);
        if (HP <= 0)
        {
            EffectManager.Instance.PlayEffect("Death", transform.position);
            Destroy(this.gameObject);
        }
        EffectManager.Instance.PlayEffect("Hit", transform.position); // 이게 여기 들어가는 게 맞을까 생각 중. "피격"애니메이션이니까 괜찮지 않을까요?
    }
}