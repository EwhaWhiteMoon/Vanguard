using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 획득한 아이템 목록을 관리합니다.
/// 아이템 개수와 직업별 유니크 아이템 개수를 추적합니다.
/// </summary>
public class InventoryManager : MonoSingleton<InventoryManager>
{
    public event Action<item> OnItemAdded;
    public event Action OnInventoryReset;

    // itemID -> 보유 개수
    private Dictionary<string, int> _itemCounts = new Dictionary<string, int>();
    
    // 직업 -> 보유 중인 유니크 아이템 ID 목록 (중복 제거용)
    private Dictionary<Job, HashSet<string>> _uniqueItemIdsByJob = new Dictionary<Job, HashSet<string>>();

    public void AddItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;

        item itemData = ItemDatabase.Instance.GetItemById(itemId);
        if (itemData != null)
        {
            // 개수 증가
            if (_itemCounts.ContainsKey(itemId))
                _itemCounts[itemId]++;
            else
                _itemCounts[itemId] = 1;

            // 직업별 유니크 아이템 추적
            Job job = JobParser.Parse(itemData.Job);
            if (job != Job.All)
            {
                if (!_uniqueItemIdsByJob.ContainsKey(job))
                {
                    _uniqueItemIdsByJob[job] = new HashSet<string>();
                }
                _uniqueItemIdsByJob[job].Add(itemId);
            }

            Debug.Log($"[InventoryManager] 아이템 획득: {itemData.Name} ({itemId})");
            OnItemAdded?.Invoke(itemData);

            // 시너지 재계산
            if (SynergyManager.Instance != null)
            {
                SynergyManager.Instance.RecalculateSynergy();
            }
        }
    }

    public void ResetAll()
    {
        _itemCounts.Clear();
        _uniqueItemIdsByJob.Clear();
        Debug.Log("[InventoryManager] 인벤토리 초기화 완료");
        OnInventoryReset?.Invoke();

        if (SynergyManager.Instance != null)
        {
            SynergyManager.Instance.ResetSynergy();
        }
    }

    public int GetUniqueCount(Job job)
    {
        if (_uniqueItemIdsByJob.TryGetValue(job, out var set))
        {
            return set.Count;
        }
        return 0;
    }

    /// <summary>
    /// 해당 아이템을 이미 보유하고 있는지 확인합니다.
    /// </summary>
    public bool HasItem(string itemId)
    {
        return _itemCounts.ContainsKey(itemId) && _itemCounts[itemId] > 0;
    }

    public Dictionary<Job, int> GetUniqueCountsByJob()
    {
        var result = new Dictionary<Job, int>();
        foreach (var kvp in _uniqueItemIdsByJob)
        {
            result[kvp.Key] = kvp.Value.Count;
        }
        return result;
    }
}
