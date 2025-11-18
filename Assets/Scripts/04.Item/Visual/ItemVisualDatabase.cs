using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// itemID 기준으로 아이템의 아이콘(Sprite)과 설명(string)을 저장하는 데이터베이스 ScriptableObject입니다.
/// 
/// - 구글 시트에서 불러온 스탯은 기존 ItemDatabase에서 관리합니다.
/// - 이 클래스는 "아이템 비주얼(아이콘, 설명)"만 따로 관리합니다.
/// - itemId는 구글 시트의 itemID와 동일한 문자열이어야 합니다.
/// 
/// 사용 방법:
/// 1. 프로젝트에 ItemVisualDatabase.asset을 하나 생성합니다.
/// 2. entries 리스트에 itemId / icon / description을 채워 넣습니다.
/// 3. 나중에 보상/상점 UI에서 itemId로 아이콘/설명을 조회할 수 있습니다.
/// </summary>
[CreateAssetMenu(
    fileName = "ItemVisualDatabase",
    menuName = "Game/Item Visual Database"
)]
public class ItemVisualDatabase : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        [Tooltip("구글 시트의 itemID와 동일한 값입니다.")]
        public string itemId;

        [Tooltip("아이템 아이콘 스프라이트")]
        public Sprite icon;

        [Tooltip("아이템 설명 텍스트")]
        [TextArea]
        public string description;
    }

    [Header("itemID 기준으로 아이템 아이콘/설명을 매칭하는 리스트입니다.")]
    public List<Entry> entries = new List<Entry>();

    /// <summary>
    /// itemId로 아이템 비주얼 정보를 조회합니다.
    /// </summary>
    /// <param name="itemId">구글 시트 itemID와 동일한 문자열</param>
    /// <returns>해당 itemId에 대응하는 Entry. 없으면 null.</returns>
    public Entry GetEntry(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[ItemVisualDatabase] itemId가 비어있습니다.");
            return null;
        }

        return entries.Find(e => e != null && e.itemId == itemId);
    }
}

