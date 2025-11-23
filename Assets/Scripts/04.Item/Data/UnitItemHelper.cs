using UnityEngine;

/// <summary>
/// 유닛 시스템과 아이템/시너지 시스템을 연결하는 헬퍼 클래스입니다.
/// 유닛의 직업을 기반으로 아이템 및 시너지 보너스를 적용합니다.
/// </summary>
public static class UnitItemHelper
{
    // UnitClass -> Job 변환
    public static Job UnitClassToJob(UnitClass unitClass)
    {
        switch (unitClass)
        {
            case UnitClass.Warrior: return Job.Warrior;
            case UnitClass.Archer: return Job.Archer;
            case UnitClass.Mage: return Job.Mage;
            case UnitClass.Assassin: return Job.Assassin;
            case UnitClass.Tanker: return Job.Tanker;
            case UnitClass.Healer: return Job.Healer;
            default:
                Debug.LogWarning($"[UnitItemHelper] 알 수 없는 UnitClass: {unitClass}");
                return Job.Warrior;
        }
    }

    // StatData(시트 데이터) -> Stat(인게임 데이터) 변환
    public static Stat StatDataToStat(StatData statData, Stat baseStat = null)
    {
        Stat result = baseStat != null ? new Stat(baseStat) : new Stat();

        result.MaxHealth += statData.Hp;
        result.ManaMax += statData.Mp;
        result.Attack += statData.Atk;
        result.Defense += statData.Def;
        result.MoveSpeed += statData.Speed;
        result.AttackSpeed += statData.AtkSpeed;
        result.CritChance += statData.Crit;
        result.CritMultiplier += statData.CritD;
        result.HealthRegenPerSec += statData.HpRegen;
        result.ManaRegenPerHit += statData.MpRegen;

        return result;
    }

    // 아이템 보너스만 적용
    public static Stat ApplyItemBonusToStat(Stat baseStat, Job job)
    {
        StatData itemBonus = ItemBonusManager.Instance != null 
            ? ItemBonusManager.Instance.GetItemBonus(job) 
            : new StatData();

        return StatDataToStat(itemBonus, baseStat);
    }

    public static Stat ApplyItemBonusToStat(Stat baseStat, UnitClass unitClass)
    {
        Job job = UnitClassToJob(unitClass);
        return ApplyItemBonusToStat(baseStat, job);
    }

    // 아이템 + 시너지 보너스 모두 적용 (권장)
    public static Stat ApplyAllBonusesToStat(Stat baseStat, Job job)
    {
        StatData itemBonus = ItemBonusManager.Instance != null 
            ? ItemBonusManager.Instance.GetItemBonus(job) 
            : new StatData();

        StatData synergyBonus = SynergyManager.Instance != null 
            ? SynergyManager.Instance.GetSynergyBonus(job) 
            : new StatData();

        StatData totalBonus = itemBonus + synergyBonus;
        Stat finalStat = StatDataToStat(totalBonus, baseStat);

        // 디버깅 로그
        Debug.Log($"[UnitItemHelper] {job} 스탯 계산:\n" +
                  $"기본: {FormatStat(baseStat)}\n" +
                  $"아이템: {StatDataHelper.FormatStatData(itemBonus)}\n" +
                  $"시너지: {StatDataHelper.FormatStatData(synergyBonus)}\n" +
                  $"최종: {FormatStat(finalStat)}");

        return finalStat;
    }

    public static Stat ApplyAllBonusesToStat(Stat baseStat, UnitClass unitClass)
    {
        Job job = UnitClassToJob(unitClass);
        return ApplyAllBonusesToStat(baseStat, job);
    }

    private static string FormatStat(Stat stat)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append($"Hp:{stat.MaxHealth}, Atk:{stat.Attack}, Def:{stat.Defense}");
        return sb.ToString();
    }
}
