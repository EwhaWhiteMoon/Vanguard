using UnityEngine;

/// <summary>
/// 구글 시트의 스탯 컬럼과 1:1로 매칭되는 스탯 데이터 구조체입니다.
/// 아이템이 제공하는 스탯 보너스를 저장하고, 유닛의 최종 스탯 계산에 사용됩니다.
/// 
/// 사용 예시:
/// <code>
/// StatData baseStat = new StatData { Hp = 100, Atk = 10 };
/// StatData itemBonus = ItemBonusManager.Instance.GetItemBonus(Job.Warrior);
/// StatData finalStat = baseStat + itemBonus; // + 연산자로 쉽게 합산 가능
/// </code>
/// </summary>
[System.Serializable]
public struct StatData
{
    /// <summary>
    /// 체력 (Health Points)
    /// </summary>
    public float Hp;

    /// <summary>
    /// 마나 (Mana Points)
    /// </summary>
    public float Mp;

    /// <summary>
    /// 공격력 (Attack)
    /// </summary>
    public float Atk;

    /// <summary>
    /// 방어력 (Defense)
    /// </summary>
    public float Def;

    /// <summary>
    /// 이동 속도 (Speed)
    /// </summary>
    public float Speed;

    /// <summary>
    /// 공격 속도 (Attack Speed)
    /// </summary>
    public float AtkSpeed;

    /// <summary>
    /// 치명타 확률 (Critical Rate)
    /// </summary>
    public float Crit;

    /// <summary>
    /// 치명타 피해량 (Critical Damage)
    /// </summary>
    public float CritD;

    /// <summary>
    /// 체력 재생 (Health Regeneration)
    /// </summary>
    public float HpRegen;

    /// <summary>
    /// 마나 재생 (Mana Regeneration)
    /// </summary>
    public float MpRegen;

    /// <summary>
    /// 두 StatData를 더하는 연산자 오버로딩입니다.
    /// 유닛의 기본 스탯과 아이템 보너스를 합산할 때 사용됩니다.
    /// 
    /// 예시:
    /// <code>
    /// StatData baseStat = new StatData { Hp = 100, Atk = 10 };
    /// StatData bonus = new StatData { Hp = 50, Atk = 5 };
    /// StatData result = baseStat + bonus; // result.Hp = 150, result.Atk = 15
    /// </code>
    /// </summary>
    /// <param name="a">첫 번째 스탯 데이터</param>
    /// <param name="b">두 번째 스탯 데이터</param>
    /// <returns>두 스탯 데이터를 합산한 결과</returns>
    public static StatData operator +(StatData a, StatData b)
    {
        return new StatData
        {
            Hp = a.Hp + b.Hp,
            Mp = a.Mp + b.Mp,
            Atk = a.Atk + b.Atk,
            Def = a.Def + b.Def,
            Speed = a.Speed + b.Speed,
            AtkSpeed = a.AtkSpeed + b.AtkSpeed,
            Crit = a.Crit + b.Crit,
            CritD = a.CritD + b.CritD,
            HpRegen = a.HpRegen + b.HpRegen,
            MpRegen = a.MpRegen + b.MpRegen
        };
    }
}

