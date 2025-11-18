using UnityEngine;

/// <summary>
/// 구글 시트의 item과 synergy 클래스를 StatData로 변환하는 헬퍼 클래스입니다.
/// </summary>
public static class StatDataHelper
{
    /// <summary>
    /// 구글 시트의 item 객체를 StatData로 변환합니다.
    /// </summary>
    /// <param name="item">변환할 item 객체</param>
    /// <returns>변환된 StatData</returns>
    public static StatData ItemToStatData(item item)
    {
        if (item == null)
            return new StatData();

        return new StatData
        {
            Hp = item.Hp,
            Mp = item.Mp,
            Atk = item.Atk,
            Def = item.Def,
            Speed = item.Speed,
            AtkSpeed = item.AtkSpeed,
            Crit = item.Crit,
            CritD = item.CritD,
            HpRegen = item.HpRegen,
            MpRegen = item.MpRegen
        };
    }

    /// <summary>
    /// 구글 시트의 synergy 객체를 StatData로 변환합니다.
    /// </summary>
    /// <param name="synergy">변환할 synergy 객체</param>
    /// <returns>변환된 StatData</returns>
    public static StatData SynergyToStatData(synergy synergy)
    {
        if (synergy == null)
            return new StatData();

        return new StatData
        {
            Hp = synergy.Hp,
            Mp = synergy.Mp,
            Atk = synergy.Atk,
            Def = synergy.Def,
            Speed = synergy.Speed,
            AtkSpeed = synergy.AtkSpeed,
            Crit = synergy.Crit,
            CritD = synergy.CritD,
            HpRegen = synergy.HpRegen,
            MpRegen = synergy.MpRegen
        };
    }
}

