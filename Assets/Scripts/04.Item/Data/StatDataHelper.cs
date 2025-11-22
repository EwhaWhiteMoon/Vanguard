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

    /// <summary>
    /// StatData를 읽기 쉬운 문자열로 포맷팅합니다.
    /// 모든 스탯을 표시하며, 0인 값은 생략합니다.
    /// </summary>
    /// <param name="data">포맷팅할 StatData</param>
    /// <returns>포맷팅된 문자열 (예: "Hp:10, Atk:5, Crit:0.1(10%)")</returns>
    public static string FormatStatData(StatData data)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        void Append(string label, float value, bool percent = false)
        {
            if (value == 0) return;
            if (sb.Length > 0) sb.Append(", ");
            sb.Append(label);
            sb.Append(':');
            if (percent)
            {
                sb.Append($"{value * 100:F1}%");
            }
            else
            {
                sb.Append(value);
            }
        }

        Append("Hp", data.Hp);
        Append("Mp", data.Mp);
        Append("Atk", data.Atk);
        Append("Def", data.Def);
        Append("Speed", data.Speed);
        Append("AtkSpd", data.AtkSpeed);
        Append("Crit", data.Crit, true);
        Append("CritD", data.CritD, true);
        Append("HpRegen", data.HpRegen);
        Append("MpRegen", data.MpRegen);

        if (sb.Length == 0)
            sb.Append("보너스 없음");

        return sb.ToString();
    }
}

