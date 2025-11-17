using UnityEngine;

/// <summary>
/// 유닛 시스템(03.Unit)과 아이템 시스템(04.Item)을 연결하는 헬퍼 클래스입니다.
/// 
/// 주요 기능:
/// 1. UnitClass를 Job으로 변환
/// 2. StatData를 Stat으로 변환
/// 3. 유닛의 최종 스탯 계산 (기본 스탯 + 아이템 보너스)
/// 
/// 사용 예시:
/// <code>
/// // 유닛 스폰 시 아이템 보너스 적용
/// Stat baseStat = unitData.BaseStat;
/// Job job = UnitItemHelper.UnitClassToJob(unitClass);
/// Stat finalStat = UnitItemHelper.ApplyItemBonusToStat(baseStat, job);
/// unitObj.stat = finalStat;
/// </code>
/// </summary>
public static class UnitItemHelper
{
    /// <summary>
    /// UnitClass를 Job enum으로 변환합니다.
    /// UnitClass와 Job은 같은 값들을 가지고 있습니다.
    /// </summary>
    /// <param name="unitClass">변환할 UnitClass</param>
    /// <returns>변환된 Job enum</returns>
    public static Job UnitClassToJob(UnitClass unitClass)
    {
        switch (unitClass)
        {
            case UnitClass.Warrior:
                return Job.Warrior;
            case UnitClass.Archer:
                return Job.Archer;
            case UnitClass.Mage:
                return Job.Mage;
            case UnitClass.Assassin:
                return Job.Assassin;
            case UnitClass.Tanker:
                return Job.Tanker;
            case UnitClass.Healer:
                return Job.Healer;
            default:
                Debug.LogWarning($"[UnitItemHelper] 알 수 없는 UnitClass: {unitClass}. Warrior로 설정합니다.");
                return Job.Warrior;
        }
    }

    /// <summary>
    /// StatData를 Stat 클래스로 변환합니다.
    /// 구글 시트의 StatData 형식을 유닛의 Stat 형식으로 매핑합니다.
    /// </summary>
    /// <param name="statData">변환할 StatData</param>
    /// <param name="baseStat">기본 Stat (기존 값 유지용, null이면 새로 생성)</param>
    /// <returns>변환된 Stat 객체</returns>
    public static Stat StatDataToStat(StatData statData, Stat baseStat = null)
    {
        Stat result = baseStat != null ? new Stat(baseStat) : new Stat();

        // StatData의 필드를 Stat의 필드로 매핑
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

    /// <summary>
    /// 유닛의 기본 Stat에 아이템 보너스를 적용하여 최종 Stat을 계산합니다.
    /// 
    /// 사용 예시:
    /// <code>
    /// Stat baseStat = unitData.BaseStat;
    /// Job job = UnitItemHelper.UnitClassToJob(unitData.Class);
    /// Stat finalStat = UnitItemHelper.ApplyItemBonusToStat(baseStat, job);
    /// unitObj.stat = finalStat;
    /// </code>
    /// </summary>
    /// <param name="baseStat">유닛의 기본 Stat</param>
    /// <param name="job">유닛의 직업 (Job enum)</param>
    /// <returns>아이템 보너스가 적용된 최종 Stat</returns>
    public static Stat ApplyItemBonusToStat(Stat baseStat, Job job)
    {
        // ItemBonusManager에서 아이템 보너스 조회
        StatData itemBonus = ItemBonusManager.Instance.GetItemBonus(job);

        // StatData를 Stat으로 변환하여 적용
        return StatDataToStat(itemBonus, baseStat);
    }

    /// <summary>
    /// UnitClass를 사용하여 유닛의 기본 Stat에 아이템 보너스를 적용합니다.
    /// UnitClass를 Job으로 자동 변환합니다.
    /// </summary>
    /// <param name="baseStat">유닛의 기본 Stat</param>
    /// <param name="unitClass">유닛의 직업 (UnitClass enum)</param>
    /// <returns>아이템 보너스가 적용된 최종 Stat</returns>
    public static Stat ApplyItemBonusToStat(Stat baseStat, UnitClass unitClass)
    {
        Job job = UnitClassToJob(unitClass);
        return ApplyItemBonusToStat(baseStat, job);
    }
}

