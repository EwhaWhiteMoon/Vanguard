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
    public float Hp;

    public float Mp;

    public float Atk;

    public float Def;

    public float Speed;

    public float AtkSpeed;

    public float Crit;

    public float CritD;

    public float HpRegen;

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

    /// <summary>
    /// 하나라도 0이 아닌 스탯이 있는지 확인합니다.
    /// </summary>
    public bool IsAnyStatNonZero()
    {
        return Hp != 0 || Mp != 0 || Atk != 0 || Def != 0 ||
               Speed != 0 || AtkSpeed != 0 || Crit != 0 || CritD != 0 ||
               HpRegen != 0 || MpRegen != 0;
    }
}

