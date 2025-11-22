using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시너지 시스템을 관리하는 싱글톤 매니저입니다.
/// 
/// 동작 방식:
/// 1. 구글 시트의 synergyList를 로드합니다.
/// 2. RecalculateSynergy()를 호출하면, InventoryManager에 기록된 보유 아이템을 기반으로 시너지를 계산합니다.
/// 3. 직업별로 유니크 아이템 개수를 세고, requiredCount <= 유니크 개수인 시너지의 보너스를 합산합니다.
/// 4. OnSynergyUpdated 이벤트를 통해 HUD 등에서 최신 진행도를 표시할 수 있습니다.
/// 5. GetSynergyBonus()로 직업별 시너지 보너스를 조회할 수 있습니다.
/// 
/// 사용 예시:
/// <code>
/// // 아이템 획득 후 시너지 재계산
/// SynergyManager.Instance.RecalculateSynergy();
/// 
/// // 유닛의 최종 스탯 계산 시
/// StatData synergyBonus = SynergyManager.Instance.GetSynergyBonus(Job.Warrior);
/// </code>
/// </summary>
public class SynergyManager : MonoSingleton<SynergyManager>
{
    /// <summary>
    /// 직업별 시너지 보너스를 저장하는 딕셔너리입니다.
    /// </summary>
    private Dictionary<Job, StatData> _synergyBonusByJob = new Dictionary<Job, StatData>();

    /// <summary>
    /// 직업별로 현재 보유 중인 "서로 다른 itemID" 개수를 저장합니다.
    /// 예) 전사 아이템 00, 01, 01을 보유하면 유니크 개수는 2
    /// </summary>
    private Dictionary<Job, int> _uniqueItemCountsByJob = new Dictionary<Job, int>();

    /// <summary>
    /// 직업별 시너지 최대 요구 개수를 저장합니다.
    /// </summary>
    private Dictionary<Job, int> _synergyMaxRequiredByJob = new Dictionary<Job, int>();

    /// <summary>
    /// 시너지 수치가 갱신될 때 호출되는 이벤트입니다.
    /// </summary>
    public event Action OnSynergyUpdated;

    /// <summary>
    /// GoogleSheetSO 인스턴스 참조입니다.
    /// </summary>
    private GoogleSheetSO _sheetData;

    private void Awake()
    {
        LoadSynergyData();
    }

    /// <summary>
    /// 구글 시트에서 시너지 데이터를 로드합니다.
    /// GoogleSheetSO.synergyDict는 synergyID로 조회할 때 사용 가능합니다.
    /// </summary>
    private void LoadSynergyData()
    {
        _sheetData = GoogleSheetManager.SO<GoogleSheetSO>();
        if (_sheetData == null || _sheetData.synergyList == null)
        {
            Debug.LogWarning("[SynergyManager] GoogleSheetSO를 찾을 수 없거나 synergyList가 비어있습니다.");
            return;
        }

        // 딕셔너리가 초기화되지 않았다면 빌드
        if (_sheetData.synergyDict == null)
        {
            _sheetData.BuildDictionaries();
        }

        BuildSynergyMaxRequirements();

        Debug.Log($"[SynergyManager] GoogleSheetSO.asset에서 {_sheetData.synergyList.Count}개의 시너지를 로드했습니다.");
        NotifySynergyChanged();
    }

    private void BuildSynergyMaxRequirements()
    {
        _synergyMaxRequiredByJob.Clear();

        foreach (var synergy in _sheetData.synergyList)
        {
            if (synergy == null)
                continue;

            Job job = JobParser.Parse(synergy.synergyName);
            if (job == Job.All)
                continue;

            if (_synergyMaxRequiredByJob.TryGetValue(job, out int currentMax))
            {
                _synergyMaxRequiredByJob[job] = Mathf.Max(currentMax, synergy.requiredCount);
            }
            else
            {
                _synergyMaxRequiredByJob[job] = synergy.requiredCount;
            }
        }
    }

    /// <summary>
    /// InventoryManager에 기록된 보유 아이템을 기반으로 시너지를 재계산합니다.
    /// 
    /// 동작 방식:
    /// 1. InventoryManager에서 보유한 모든 아이템 ID를 가져옵니다.
    /// 2. ItemDatabase에서 아이템 데이터를 조회합니다.
    /// 3. 직업별로 "서로 다른 itemID의 개수"를 셉니다.
    /// 4. 각 직업에 대해, 해당 직업의 시너지 중 requiredCount <= 유니크 개수인 것들의 bonusStats를 모두 더합니다.
    /// 5. 더한 결과를 _synergyBonusByJob[job]에 저장합니다.
    /// 
    /// 예시:
    /// - 전사 아이템 4종류(itemID: 1, 2, 3, 4)를 보유한 경우
    /// - 유니크 개수 = 4
    /// - requiredCount <= 4인 모든 전사 시너지의 보너스를 합산
    /// </summary>
    public void RecalculateSynergy()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("[SynergyManager] InventoryManager를 찾을 수 없습니다.");
            return;
        }

        if (ItemDatabase.Instance == null)
        {
            Debug.LogWarning("[SynergyManager] ItemDatabase를 찾을 수 없습니다.");
            return;
        }

        if (_sheetData == null || _sheetData.synergyList == null)
        {
            Debug.LogWarning("[SynergyManager] 시너지 데이터가 로드되지 않았습니다.");
            return;
        }

        Dictionary<Job, int> uniqueCountsByJob = InventoryManager.Instance.GetUniqueCountsByJob();

        if (uniqueCountsByJob.Count == 0)
        {
            // 보유한 아이템이 없으면 모든 시너지 보너스를 초기화
            _synergyBonusByJob.Clear();
            _uniqueItemCountsByJob.Clear();
            Debug.Log("[SynergyManager] 보유한 아이템이 없어 시너지 보너스를 초기화했습니다.");
            NotifySynergyChanged();
            return;
        }

        // 2. 각 직업에 대해 시너지 보너스 계산
        _synergyBonusByJob.Clear();
        _uniqueItemCountsByJob.Clear();

        foreach (var kvp in uniqueCountsByJob)
        {
            Job job = kvp.Key;
            int uniqueCount = kvp.Value;
            _uniqueItemCountsByJob[job] = uniqueCount;
            StatData totalBonus = new StatData();

            // 해당 직업의 모든 시너지를 확인
            foreach (var synergy in _sheetData.synergyList)
            {
                if (synergy == null) continue;

                Job synergyJob = JobParser.Parse(synergy.synergyName);
                if (synergyJob != job) continue;

                // requiredCount <= 유니크 개수인 시너지만 적용
                if (synergy.requiredCount <= uniqueCount)
                {
                    StatData synergyBonus = StatDataHelper.SynergyToStatData(synergy);
                    totalBonus = totalBonus + synergyBonus;
                }
            }

            if (totalBonus.Hp != 0 || totalBonus.Mp != 0 || totalBonus.Atk != 0 || 
                totalBonus.Def != 0 || totalBonus.Speed != 0 || totalBonus.AtkSpeed != 0 ||
                totalBonus.Crit != 0 || totalBonus.CritD != 0 || totalBonus.HpRegen != 0 || totalBonus.MpRegen != 0)
            {
                _synergyBonusByJob[job] = totalBonus;
                Debug.Log($"[SynergyManager] {job} 시너지 계산 완료: 유니크 아이템 {uniqueCount}개, 보너스 합계 = {StatDataHelper.FormatStatData(totalBonus)}");
            }
        }

        NotifySynergyChanged();
    }

    /// <summary>
    /// 지정된 직업의 시너지 보너스를 조회합니다.
    /// </summary>
    /// <param name="job">조회할 직업</param>
    /// <returns>해당 직업의 시너지 보너스, 없으면 모든 값이 0인 StatData 반환</returns>
    public StatData GetSynergyBonus(Job job)
    {
        if (_synergyBonusByJob.TryGetValue(job, out StatData bonus))
        {
            return bonus;
        }

        return new StatData(); // 모든 값이 0인 StatData 반환
    }

    /// <summary>
    /// 모든 시너지 보너스를 초기화합니다.
    /// </summary>
    public void ResetSynergy()
    {
        _synergyBonusByJob.Clear();
        _uniqueItemCountsByJob.Clear();
        Debug.Log("[SynergyManager] 모든 시너지 보너스가 초기화되었습니다.");
        NotifySynergyChanged();
    }

    /// <summary>
    /// 특정 직업의 현재 유니크 아이템 개수를 반환합니다.
    /// </summary>
    public int GetCurrentUniqueCount(Job job)
    {
        if (_uniqueItemCountsByJob.TryGetValue(job, out int count))
        {
            return count;
        }

        return 0;
    }

    /// <summary>
    /// 특정 직업의 시너지 최종 요구 개수를 반환합니다.
    /// </summary>
    public int GetMaxRequiredCount(Job job)
    {
        if (_synergyMaxRequiredByJob.TryGetValue(job, out int max))
        {
            return max;
        }

        return 0;
    }

    /// <summary>
    /// HUD 등에서 한 번에 표시할 수 있도록 현재 정보를 튜플로 반환합니다.
    /// </summary>
    public IReadOnlyDictionary<Job, int> GetAllCurrentCounts() => _uniqueItemCountsByJob;

    public IReadOnlyDictionary<Job, int> GetAllMaxCounts() => _synergyMaxRequiredByJob;

    private void NotifySynergyChanged()
    {
        OnSynergyUpdated?.Invoke();
    }

}

