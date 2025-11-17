using UnityEngine;

/// <summary>
/// 아이템 획득 시 필요한 로직을 처리하는 헬퍼 클래스입니다.
/// 
/// 아이템 획득 시:
/// 1. ItemBonusManager에 스탯 보너스를 누적합니다.
/// 2. InventoryManager에 itemID를 기록합니다.
/// 
/// 사용 예시:
/// <code>
/// // 아이템 획득 (보상 화면에서 호출)
/// InventoryHelper.AcquireItem("1");
/// </code>
/// </summary>
public static class InventoryHelper
{
    /// <summary>
    /// 아이템을 획득합니다.
    /// 
    /// 동작 흐름:
    /// 1. ItemDatabase에서 itemID로 구글 시트 데이터 조회
    /// 2. Job 문자열을 Job enum으로 변환
    /// 3. item 데이터를 StatData로 변환
    /// 4. ItemBonusManager에 스탯 보너스 누적
    /// 5. InventoryManager에 획득 기록
    /// </summary>
    /// <param name="itemId">획득할 아이템의 ID (구글 시트의 itemID를 문자열로 변환한 값)</param>
    public static void AcquireItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[InventoryHelper] itemId가 비어 있습니다.");
            return;
        }

        // 1. 구글 시트 row 가져오기
        item sheetItem = ItemDatabase.Instance.GetItemById(itemId);
        if (sheetItem == null)
        {
            Debug.LogWarning($"[InventoryHelper] ItemId '{itemId}'에 해당하는 시트 데이터가 없습니다.");
            return;
        }

        // 2. Job 문자열 -> Job enum
        Job job = JobParser.Parse(sheetItem.Job);

        // 3. 시트 row -> StatData 변환
        StatData bonus = new StatData
        {
            Hp = sheetItem.Hp,
            Mp = sheetItem.Mp,
            Atk = sheetItem.Atk,
            Def = sheetItem.Def,
            Speed = sheetItem.Speed,
            AtkSpeed = sheetItem.AtkSpeed,
            Crit = sheetItem.Crit,
            CritD = sheetItem.CritD,
            HpRegen = sheetItem.HpRegen,
            MpRegen = sheetItem.MpRegen
        };

        // 4. 직업별 스탯 보너스 누적
        ItemBonusManager.Instance.AddItemBonus(job, bonus);

        // 5. 획득한 아이템 목록에 기록
        InventoryManager.Instance.AddItem(itemId);

        string targetInfo = job == Job.All ? "모든 직업" : job.ToString();
        Debug.Log($"[InventoryHelper] 아이템 획득 완료: {sheetItem.Name} (ID: {itemId}) - {targetInfo}에게 보너스 적용");
    }
}

