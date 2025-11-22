using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 획득으로 인해 직업별로 누적된 스탯 보너스를 저장/제공하는 싱글톤입니다.
/// ItemEffectManager가 AddItemBonus를 호출해 보너스를 누적하고,
/// UnitItemHelper가 GetItemBonus를 호출해 보너스를 조회합니다.
/// </summary>
public class ItemBonusManager : MonoSingleton<ItemBonusManager>
{
    /// <summary>
    /// 직업별로 누적된 아이템 보너스를 저장하는 딕셔너리입니다.
    /// Key: 직업 (Job enum)
    /// Value: 해당 직업에게 누적된 총 스탯 보너스 (StatData)
    /// 
    /// 예시:
    /// - Job.Warrior -> 전사에게 적용된 모든 아이템 보너스의 합
    /// - Job.All -> 모든 직업에게 공통으로 적용되는 보너스
    /// </summary>
    private Dictionary<Job, StatData> _bonusByJob = new Dictionary<Job, StatData>();

    /// <summary>
    /// Job.All 대상 아이템들의 누적 보너스를 별도로 저장합니다.
    /// GetItemBonus 호출 시 _globalBonus와 _bonusByJob[job]을 합산하여 반환합니다.
    /// </summary>
    private StatData _globalBonus = new StatData();

    /// <summary>
    /// 아이템을 획득했을 때, 지정된 직업에 스탯 보너스를 누적 추가합니다.
    /// 
    /// 동작 방식:
    /// - 해당 직업의 기존 보너스가 있으면 새로운 보너스를 더해서 누적합니다.
    /// - 해당 직업의 보너스가 없으면 새로운 보너스를 그대로 저장합니다.
    /// </summary>
    /// <param name="job">보너스를 적용할 직업 (Job.All도 가능)</param>
    /// <param name="bonus">추가할 스탯 보너스</param>
    public void AddItemBonus(Job job, StatData bonus)
    {
        // job == Job.All이면 _globalBonus에 누적
        if (job == Job.All)
        {
            _globalBonus = _globalBonus + bonus;
            Debug.Log($"[ItemBonusManager] 전체 직업(All)에게 보너스 추가: {StatDataHelper.FormatStatData(bonus)}");
        }
        else
        {
            // 그 외에는 _bonusByJob[job]에 누적
            if (_bonusByJob.ContainsKey(job))
            {
                // 기존 보너스가 있으면 더해서 누적
                _bonusByJob[job] = _bonusByJob[job] + bonus;
            }
            else
            {
                // 기존 보너스가 없으면 새로 추가
                _bonusByJob[job] = bonus;
            }

            Debug.Log($"[ItemBonusManager] {job}에게 보너스 추가: {StatDataHelper.FormatStatData(bonus)}");
        }
    }

    /// <summary>
    /// 팀 전체(모든 직업)에게 적용되는 보너스를 추가하는 편의 메서드입니다.
    /// </summary>
    /// <param name="bonus">전체 직업에게 적용할 스탯 보너스</param>
    public void AddGlobalBonus(StatData bonus)
    {
        AddItemBonus(Job.All, bonus);
    }

    /// <summary>
    /// 지정된 직업에 대한 누적된 아이템 보너스를 조회합니다.
    /// Job.All 보너스와 특정 직업 보너스를 합산하여 반환합니다.
    /// </summary>
    /// <param name="job">보너스를 조회할 직업</param>
    /// <returns>해당 직업이 아이템을 통해 얻은 전체 보너스 스탯</returns>
    public StatData GetItemBonus(Job job)
    {
        StatData result = new StatData(); // 기본값은 모두 0

        // 1. 항상 _globalBonus를 포함해서 계산
        result = result + _globalBonus;

        // 2. 해당 job에 대한 보너스가 있으면 포함
        // (Job.All이 아닌 경우에만)
        if (job != Job.All && _bonusByJob.ContainsKey(job))
        {
            result = result + _bonusByJob[job];
        }

        return result;
    }

    /// <summary>
    /// 모든 아이템 보너스를 초기화합니다.
    /// 게임 시작 전, 새 라운드 시작 시, 또는 테스트 목적으로 사용할 수 있습니다.
    /// 
    /// 사용 예시:
    /// <code>
    /// // 새 라운드 시작 시 보너스 초기화
    /// ItemBonusManager.Instance.ResetBonuses();
    /// </code>
    /// </summary>
    public void ResetBonuses()
    {
        _bonusByJob.Clear();
        _globalBonus = new StatData(); // 모든 값이 0인 StatData로 초기화
        Debug.Log("[ItemBonusManager] 모든 아이템 보너스가 초기화되었습니다.");
    }

    /// <summary>
    /// 디버깅용: 현재 저장된 모든 보너스를 로그로 출력합니다.
    /// </summary>
    [ContextMenu("Debug: Print All Bonuses")]
    public void DebugPrintAllBonuses()
    {
        Debug.Log("=== [ItemBonusManager] 현재 저장된 모든 보너스 ===");
        if (_globalBonus.Hp != 0 || _globalBonus.Mp != 0 || _globalBonus.Atk != 0 || 
            _globalBonus.Def != 0 || _globalBonus.Speed != 0 || _globalBonus.AtkSpeed != 0 ||
            _globalBonus.Crit != 0 || _globalBonus.CritD != 0 || _globalBonus.HpRegen != 0 || _globalBonus.MpRegen != 0)
        {
            Debug.Log($"전체 직업(All): {StatDataHelper.FormatStatData(_globalBonus)}");
        }
        foreach (var kvp in _bonusByJob)
        {
            Debug.Log($"{kvp.Key}: {StatDataHelper.FormatStatData(kvp.Value)}");
        }
    }
}

