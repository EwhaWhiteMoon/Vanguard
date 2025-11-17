using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템으로 인한 스탯 보너스를 직업별로 관리하는 싱글톤 매니저입니다.
/// 
/// 주요 역할:
/// 1. 아이템 획득 시 직업별로 스탯 보너스를 누적 저장
/// 2. 유닛 담당자가 최종 스탯 계산 시 필요한 보너스를 제공
/// 
/// 사용 흐름:
/// 1. 아이템 획득 시: ItemBase.OnGet() -> ItemBonusManager.AddItemBonus() 호출
/// 2. 유닛 스탯 계산 시: ItemBonusManager.GetItemBonus(job) 호출하여 보너스 획득
/// 3. 최종 스탯 계산: baseStat + itemBonus = finalStat
/// 
/// 유닛 담당자 사용 예시:
/// <code>
/// // 유닛의 기본 스탯 (구글 시트나 ScriptableObject에서 로드)
/// StatData baseStat = unitBaseStat;
/// 
/// // 아이템으로 인한 보너스 스탯 획득
/// StatData itemBonus = ItemBonusManager.Instance.GetItemBonus(job);
/// 
/// // 최종 스탯 계산
/// StatData finalStat = baseStat + itemBonus;
/// 
/// // finalStat을 실제 전투에서 사용할 유닛 스탯으로 사용
/// </code>
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
    /// 
    /// 사용 예시:
    /// <code>
    /// // 전사에게 공격력 +10, 체력 +50 보너스 추가
    /// StatData bonus = new StatData { Atk = 10, Hp = 50 };
    /// ItemBonusManager.Instance.AddItemBonus(Job.Warrior, bonus);
    /// 
    /// // 나중에 또 다른 아이템으로 전사에게 공격력 +5 추가
    /// StatData bonus2 = new StatData { Atk = 5 };
    /// ItemBonusManager.Instance.AddItemBonus(Job.Warrior, bonus2);
    /// // 결과: 전사는 총 Atk +15, Hp +50 보너스를 받게 됩니다.
    /// </code>
    /// </summary>
    /// <param name="job">보너스를 적용할 직업 (Job.All도 가능)</param>
    /// <param name="bonus">추가할 스탯 보너스</param>
    public void AddItemBonus(Job job, StatData bonus)
    {
        // job == Job.All이면 _globalBonus에 누적
        if (job == Job.All)
        {
            _globalBonus = _globalBonus + bonus;
            Debug.Log($"[ItemBonusManager] 전체 직업(All)에게 보너스 추가: Hp={bonus.Hp}, Atk={bonus.Atk}, Def={bonus.Def} 등");
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

            Debug.Log($"[ItemBonusManager] {job}에게 보너스 추가: Hp={bonus.Hp}, Atk={bonus.Atk}, Def={bonus.Def} 등");
        }
    }

    /// <summary>
    /// 팀 전체(모든 직업)에게 적용되는 보너스를 추가하는 편의 메서드입니다.
    /// Job.All에 보너스를 추가하는 것과 동일합니다.
    /// 
    /// 사용 예시:
    /// <code>
    /// // 모든 직업에게 체력 +100 보너스
    /// StatData globalBonus = new StatData { Hp = 100 };
    /// ItemBonusManager.Instance.AddGlobalBonus(globalBonus);
    /// </code>
    /// </summary>
    /// <param name="bonus">전체 직업에게 적용할 스탯 보너스</param>
    public void AddGlobalBonus(StatData bonus)
    {
        AddItemBonus(Job.All, bonus);
    }


    /// <summary>
    /// 유닛 담당자가 최종 스탯 계산 시 사용하는 핵심 메서드입니다.
    /// 
    /// 동작 방식:
    /// 1. Job.All에 해당하는 보너스가 있으면 포함해서 더합니다.
    /// 2. 해당 job(예: Warrior)에 대한 보너스가 있으면 포함해서 더합니다.
    /// 3. 둘 다 없으면 모든 값이 0인 StatData를 반환합니다.
    /// 
    /// 중요: Job.All 보너스와 특정 직업 보너스는 함께 적용됩니다.
    /// 예를 들어, All 보너스로 Atk +10이 있고, Warrior 보너스로 Atk +5가 있으면,
    /// 전사는 총 Atk +15 보너스를 받게 됩니다.
    /// 
    /// 사용 예시:
    /// <code>
    /// // 전사 유닛의 최종 스탯 계산
    /// StatData baseStat = unitBaseStat; // 기본 스탯
    /// StatData itemBonus = ItemBonusManager.Instance.GetItemBonus(Job.Warrior);
    /// StatData finalStat = baseStat + itemBonus;
    /// </code>
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
        foreach (var kvp in _bonusByJob)
        {
            StatData stat = kvp.Value;
            Debug.Log($"{kvp.Key}: Hp={stat.Hp}, Mp={stat.Mp}, Atk={stat.Atk}, Def={stat.Def}, " +
                      $"Speed={stat.Speed}, AtkSpeed={stat.AtkSpeed}, Crit={stat.Crit}, " +
                      $"CritD={stat.CritD}, HpRegen={stat.HpRegen}, MpRegen={stat.MpRegen}");
        }
    }
}

