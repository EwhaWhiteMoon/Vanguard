using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템의 비주얼 데이터(아이콘, 설명)를 저장하는 SO입니다.
/// </summary>
[CreateAssetMenu(fileName = "ItemVisualDatabase", menuName = "Game/Item Visual Database")]
public class ItemVisualDatabase : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string itemId;
        public Sprite icon;
        [TextArea] public string description;
    }

    public List<Entry> entries = new List<Entry>();

    public Entry GetEntry(string itemId)
    {
        return entries.Find(e => e != null && e.itemId == itemId);
    }
}
