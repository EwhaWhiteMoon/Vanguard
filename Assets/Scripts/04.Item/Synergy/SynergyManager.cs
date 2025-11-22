using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보유 아이템에 따른 직업별 시너지 보너스를 계산합니다.
/// </summary>
public class SynergyManager : MonoSingleton<SynergyManager>
{
    public event Action OnSynergyUpdated;

    private Dictionary<Job, StatData> _synergyBonusByJob = new Dictionary<Job, StatData>();
    private Dictionary<Job, int> _uniqueItemCountsByJob = new Dictionary<Job, int>();
    private Dictionary<Job, int> _synergyMaxRequiredByJob = new Dictionary<Job, int>();
    private GoogleSheetSO _sheetData;

    private void Awake()
    {
        LoadSynergyData();
    }

    private void LoadSynergyData()
    {
        _sheetData = GoogleSheetManager.SO<GoogleSheetSO>();
        if (_sheetData == null) return;

        if (_sheetData.synergyDict == null)
            _sheetData.BuildDictionaries();

        BuildSynergyMaxRequirements();
        NotifySynergyChanged();
    }

    private void BuildSynergyMaxRequirements()
    {
        _synergyMaxRequiredByJob.Clear();
        if (_sheetData.synergyList == null) return;

        foreach (var synergy in _sheetData.synergyList)
        {
            if (synergy == null) continue;
            Job job = JobParser.Parse(synergy.synergyName);
            if (job == Job.All) continue;

            if (_synergyMaxRequiredByJob.TryGetValue(job, out int currentMax))
                _synergyMaxRequiredByJob[job] = Mathf.Max(currentMax, synergy.requiredCount);
            else
                _synergyMaxRequiredByJob[job] = synergy.requiredCount;
        }
    }

    public void RecalculateSynergy()
    {
        if (InventoryManager.Instance == null || ItemDatabase.Instance == null || _sheetData == null)
            return;

        var uniqueCountsByJob = InventoryManager.Instance.GetUniqueCountsByJob();
        _synergyBonusByJob.Clear();
        _uniqueItemCountsByJob.Clear();

        if (uniqueCountsByJob.Count == 0)
        {
            Debug.Log("[SynergyManager] 시너지 초기화 (아이템 없음)");
            NotifySynergyChanged();
            return;
        }

        foreach (var kvp in uniqueCountsByJob)
        {
            Job job = kvp.Key;
            int uniqueCount = kvp.Value;
            _uniqueItemCountsByJob[job] = uniqueCount;
            StatData totalBonus = new StatData();

            foreach (var synergy in _sheetData.synergyList)
            {
                if (synergy == null) continue;
                if (JobParser.Parse(synergy.synergyName) != job) continue;

                // 조건 충족 시너지 합산
                if (synergy.requiredCount <= uniqueCount)
                {
                    totalBonus = totalBonus + StatDataHelper.SynergyToStatData(synergy);
                }
            }

            if (totalBonus.IsAnyStatNonZero())
            {
                _synergyBonusByJob[job] = totalBonus;
                Debug.Log($"[SynergyManager] {job} 시너지 적용: {StatDataHelper.FormatStatData(totalBonus)}");
            }
        }

        NotifySynergyChanged();
    }

    public StatData GetSynergyBonus(Job job)
    {
        if (_synergyBonusByJob.TryGetValue(job, out StatData bonus))
            return bonus;
        return new StatData();
    }

    public void ResetSynergy()
    {
        _synergyBonusByJob.Clear();
        _uniqueItemCountsByJob.Clear();
        NotifySynergyChanged();
    }

    public int GetCurrentUniqueCount(Job job)
    {
        return _uniqueItemCountsByJob.TryGetValue(job, out int count) ? count : 0;
    }

    public int GetMaxRequiredCount(Job job)
    {
        return _synergyMaxRequiredByJob.TryGetValue(job, out int max) ? max : 0;
    }

    private void NotifySynergyChanged()
    {
        OnSynergyUpdated?.Invoke();
    }
}
