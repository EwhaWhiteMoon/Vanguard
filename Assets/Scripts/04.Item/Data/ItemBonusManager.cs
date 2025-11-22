using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 획득으로 누적된 직업별 스탯 보너스를 관리합니다.
/// </summary>
public class ItemBonusManager : MonoSingleton<ItemBonusManager>
{
    // 직업별 누적 보너스
    private Dictionary<Job, StatData> _bonusByJob = new Dictionary<Job, StatData>();

    // 모든 직업 공통 보너스 (Job.All)
    private StatData _globalBonus = new StatData();

    public void AddItemBonus(Job job, StatData bonus)
    {
        if (job == Job.All)
        {
            _globalBonus = _globalBonus + bonus;
            Debug.Log($"[ItemBonusManager] 전체 직업(All) 보너스 추가됨: {StatDataHelper.FormatStatData(bonus)} | 누적 합계: {StatDataHelper.FormatStatData(_globalBonus)}");
        }
        else
        {
            if (_bonusByJob.ContainsKey(job))
                _bonusByJob[job] = _bonusByJob[job] + bonus;
            else
                _bonusByJob[job] = bonus;

            // 해당 직업의 총 보너스 (공통 보너스 포함)
            StatData totalBonus = GetItemBonus(job);
            Debug.Log($"[ItemBonusManager] {job} 보너스 추가됨: {StatDataHelper.FormatStatData(bonus)} | {job} 누적 합계(All포함): {StatDataHelper.FormatStatData(totalBonus)}");
        }
    }

    public void AddGlobalBonus(StatData bonus)
    {
        AddItemBonus(Job.All, bonus);
    }

    public StatData GetItemBonus(Job job)
    {
        StatData result = new StatData();
        result = result + _globalBonus;

        if (job != Job.All && _bonusByJob.ContainsKey(job))
        {
            result = result + _bonusByJob[job];
        }

        return result;
    }

    public void ResetBonuses()
    {
        _bonusByJob.Clear();
        _globalBonus = new StatData();
        Debug.Log("[ItemBonusManager] 아이템 보너스 초기화 완료");
    }

    [ContextMenu("Debug: Print All Bonuses")]
    public void DebugPrintAllBonuses()
    {
        Debug.Log("=== [ItemBonusManager] 현재 보너스 현황 ===");
        if (_globalBonus.IsAnyStatNonZero())
        {
            Debug.Log($"전체 직업(All): {StatDataHelper.FormatStatData(_globalBonus)}");
        }
        foreach (var kvp in _bonusByJob)
        {
            Debug.Log($"{kvp.Key}: {StatDataHelper.FormatStatData(kvp.Value)}");
        }
    }
}
