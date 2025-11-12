using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitObj : MonoBehaviour
{
    public UnitData unitData;
    public Stat stat;
    public int Team;
    public float HP;
    public float BeforeHP = -1;
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
        HPText.text = this.HP.ToString("N0");
    }

    private void Update()
    {
        if (BeforeHP != HP)
        {
            BeforeHP = HP;
            HPText.text = HP.ToString("N0"); // TODO: HP가 Update 될 때만 텍스트 갱신되도록 하기
        }
    }

    public void Attack(UnitObj target)
    {
        target.TakeDamage(stat.Attack * (Random.Range(0f, 1f) < stat.CritChance ? stat.CritMultiplier : 1f));
    }
    
    public void TakeDamage(float damage)
    {
        Debug.Log(damage);
        HP -= (damage - stat.Defense) * (1 - stat.DamageReducePct);
        if (HP <= 0) Destroy(this.gameObject);
    }
}