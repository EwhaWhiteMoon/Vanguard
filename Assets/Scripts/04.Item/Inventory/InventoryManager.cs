using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 지금까지 획득한 아이템 목록을
/// "itemID별 개수"로만 기록하는 매니저입니다.
/// "어떤 아이템을 몇 개 먹었는지"를 저장하는 용도입니다.
/// </summary>
public class InventoryManager : MonoSingleton<InventoryManager>
{
    /// <summary>
    /// itemID -> 현재까지 먹은 개수
    /// </summary>
    private Dictionary<string, int> _itemCounts = new Dictionary<string, int>();

    /// <summary>
    /// 아이템을 하나 획득했다고 기록합니다.
    /// </summary>
    /// <param name="itemId">획득한 아이템의 ID</param>
    public void AddItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[InventoryManager] itemId가 비어 있습니다.");
            return;
        }

        if (_itemCounts.ContainsKey(itemId))
        {
            _itemCounts[itemId]++;
        }
        else
        {
            _itemCounts[itemId] = 1;
        }

        Debug.Log($"[InventoryManager] 아이템 획득: {itemId}, 현재 개수 = {_itemCounts[itemId]}");
    }

    /// <summary>
    /// 특정 itemID를 몇 개 먹었는지 반환합니다.
    /// </summary>
    /// <param name="itemId">조회할 아이템의 ID</param>
    /// <returns>획득한 개수, 없으면 0</returns>
    public int GetItemCount(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return 0;

        if (_itemCounts.TryGetValue(itemId, out int count))
            return count;

        return 0;
    }

    /// <summary>
    /// 지금까지 먹은 모든 아이템의 (itemID, 개수) 목록을 돌려줍니다.
    /// UI나 디버그용으로 사용할 수 있습니다.
    /// </summary>
    /// <returns>모든 아이템의 (itemID, 개수) 쌍</returns>
    public IEnumerable<KeyValuePair<string, int>> GetAllItemCounts()
    {
        return _itemCounts;
    }

    /// <summary>
    /// 새 게임 시작 등에서 전체 초기화가 필요할 때 사용합니다.
    /// </summary>
    public void ResetAll()
    {
        _itemCounts.Clear();
        Debug.Log("[InventoryManager] 모든 획득 아이템 기록 초기화");
    }
}

