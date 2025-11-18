using UnityEngine;

/// <summary>
/// 아이템의 OnGet()과 OnCombat() 효과를 관리하는 싱글톤입니다.
/// 
/// - OnGet(): 아이템 획득 시 호출 (ItemEffectManager.Instance.OnGet(itemId) 직접 호출)
/// - OnCombat(): 전투 시작 시 호출 (전투 시스템에서 수동 호출 필요)
/// </summary>
public class ItemEffectManager : MonoSingleton<ItemEffectManager>
{
    /// <summary>
    /// 아이템 획득 시 OnGet() 효과를 처리합니다.
    /// 스탯 보너스를 ItemBonusManager에 추가하고 InventoryManager에 획득 기록합니다.
    /// </summary>
    /// <param name="itemId">획득한 아이템의 ID</param>
    public void OnGet(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[ItemEffectManager] itemId가 비어 있습니다.");
            return;
        }

        item sheetItem = ItemDatabase.Instance.GetItemById(itemId);
        if (sheetItem == null)
        {
            Debug.LogWarning($"[ItemEffectManager] ItemId '{itemId}'에 해당하는 시트 데이터가 없습니다.");
            return;
        }

        Job job = JobParser.Parse(sheetItem.Job);
        StatData bonus = StatDataHelper.ItemToStatData(sheetItem);

        ItemBonusManager.Instance.AddItemBonus(job, bonus);
        InventoryManager.Instance.AddItem(itemId);

        string targetInfo = job == Job.All ? "모든 직업" : job.ToString();
        Debug.Log($"[ItemEffectManager] OnGet 완료: {sheetItem.Name} (ID: {itemId}) - {targetInfo}에게 보너스 적용");
    }

    /// <summary>
    /// 전투 시작 시 OnCombat() 효과를 처리합니다.
    /// 전투 시스템에서 전투 시작 시 호출합니다.
    /// </summary>
    public void OnCombat()
    {
        // TODO: 나중에 OnCombat 전용 효과 구현
        Debug.Log("[ItemEffectManager] OnCombat 호출됨");
    }
}

