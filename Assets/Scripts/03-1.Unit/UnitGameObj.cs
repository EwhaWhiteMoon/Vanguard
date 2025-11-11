using TMPro;
using UnityEngine;

public class UnitGameObj : MonoBehaviour
{
    public UnitData unitData;
    public SpriteRenderer spriteRenderer;
    public int Team;

    public Stat stat;
    public float HP;
    
    public TextMeshPro HPText;
    
    public ICombatManager combatManager; // unit의 list를 받아오는 기능(지금은...)
    public void Init(UnitData data, int team, ICombatManager combatManager)
    {
        unitData = data;
        spriteRenderer.sprite = unitData.sprite;
        Team = team;
        this.combatManager = combatManager;
        
        stat = new Stat(unitData.stat);
        HP = stat.MaxHealth;
    }

    private void Update()
    {
        HPText.text = HP.ToString("N0"); // TODO: HP가 Update 될 때만 텍스트 갱신되도록 하기
    }

    public void Attack(UnitGameObj target)
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