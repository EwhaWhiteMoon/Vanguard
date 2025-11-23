using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛 ID(unitID)를 키로 하여 유닛의 아이콘과 설명을 관리하는 데이터베이스입니다.
/// </summary>
[CreateAssetMenu(fileName = "UnitVisualDatabase", menuName = "Game/Unit Visual Database")]
public class UnitVisualDatabase : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        [Tooltip("구글 시트의 unitID와 동일한 값입니다.")]
        public string unitId;

        [Tooltip("유닛 아이콘 스프라이트")]
        public Sprite icon;

        [Tooltip("유닛 설명 (선택사항)")]
        [TextArea]
        public string description;
    }

    public List<Entry> entries = new List<Entry>();

    public Entry GetEntry(string unitId)
    {
        return entries.Find(e => e != null && e.unitId == unitId);
    }
}

