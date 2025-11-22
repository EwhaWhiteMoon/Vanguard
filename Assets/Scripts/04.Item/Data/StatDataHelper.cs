using UnityEngine;

/// <summary>
/// 구글 시트 데이터(item, synergy)를 인게임 StatData로 변환하고 포맷팅합니다.
/// </summary>
public static class StatDataHelper
{
    public static StatData ItemToStatData(item item)
    {
        if (item == null) return new StatData();
        return CreateStatData(item.Hp, item.Mp, item.Atk, item.Def, item.Speed, item.AtkSpeed, item.Crit, item.CritD, item.HpRegen, item.MpRegen);
    }

    public static StatData SynergyToStatData(synergy synergy)
    {
        if (synergy == null) return new StatData();
        return CreateStatData(synergy.Hp, synergy.Mp, synergy.Atk, synergy.Def, synergy.Speed, synergy.AtkSpeed, synergy.Crit, synergy.CritD, synergy.HpRegen, synergy.MpRegen);
    }

    private static StatData CreateStatData(float hp, float mp, float atk, float def, float speed, float atkSpeed, float crit, float critD, float hpRegen, float mpRegen)
    {
        return new StatData
        {
            Hp = hp, Mp = mp, Atk = atk, Def = def,
            Speed = speed, AtkSpeed = atkSpeed,
            Crit = crit, CritD = critD,
            HpRegen = hpRegen, MpRegen = mpRegen
        };
    }

    // 디버깅용 로그 포맷팅 (0이 아닌 값만 표시)
    public static string FormatStatData(StatData data)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        void Append(string label, float value, bool percent = false)
        {
            if (value == 0) return;
            if (sb.Length > 0) sb.Append(", ");
            sb.Append(label).Append(':').Append(percent ? $"{value * 100:F1}%" : $"{value}");
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

        if (sb.Length == 0) sb.Append("보너스 없음");
        return sb.ToString();
    }
}
