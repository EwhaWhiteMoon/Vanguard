using UnityEngine;

/// <summary>
/// 아이템 획득(OnGet) 및 전투 시작(OnCombat) 효과를 처리합니다.
/// </summary>
public class ItemEffectManager : MonoSingleton<ItemEffectManager>
{
    public void OnGet(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;

        item sheetItem = ItemDatabase.Instance.GetItemById(itemId);
        if (sheetItem == null)
        {
            Debug.LogWarning($"[ItemEffectManager] '{itemId}' 데이터 없음");
            return;
        }

        Job job = JobParser.Parse(sheetItem.Job);
        StatData bonus = StatDataHelper.ItemToStatData(sheetItem);

        // 보너스 적용 및 인벤토리 기록
        ItemBonusManager.Instance.AddItemBonus(job, bonus);
        InventoryManager.Instance.AddItem(itemId);

        string targetInfo = job == Job.All ? "모든 직업" : job.ToString();
        Debug.Log($"[ItemEffectManager] 아이템 적용: {sheetItem.Name}({itemId}) -> {targetInfo}");
    }

    public void OnCombat()
    {
        // 추후 전투 시작 시 발동하는 효과 구현
    }
}
