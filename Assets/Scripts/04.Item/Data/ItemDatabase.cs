using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ItemID로 GoogleSheetSO.asset의 item 데이터를 조회할 수 있는 데이터베이스 클래스입니다.
/// GoogleSheetSO.itemDict를 직접 사용하여 중복 딕셔너리 생성을 방지합니다.
/// </summary>
public class ItemDatabase : MonoSingleton<ItemDatabase>
{
    /// <summary>
    /// GoogleSheetSO 인스턴스 참조입니다.
    /// </summary>
    private GoogleSheetSO _sheetData;

    private void Awake()
    {
        LoadItemsFromGoogleSheet();
    }

    /// <summary>
    /// 구글 시트에서 아이템 데이터를 로드합니다.
    /// GoogleSheetSO.asset의 itemDict를 직접 사용합니다. (딕셔너리는 GoogleSheetSO에서 자동 생성됨)
    /// </summary>
    private void LoadItemsFromGoogleSheet()
    {
        // GoogleSheetSO 가져오기 (이미 존재하는 SO 파일)
        _sheetData = GoogleSheetManager.SO<GoogleSheetSO>();
        if (_sheetData == null || _sheetData.itemList == null)
        {
            Debug.LogWarning("[ItemDatabase] GoogleSheetSO를 찾을 수 없거나 itemList가 비어있습니다.");
            return;
        }

        // 딕셔너리가 초기화되지 않았다면 빌드
        if (_sheetData.itemDict == null)
        {
            _sheetData.BuildDictionaries();
        }

        Debug.Log($"[ItemDatabase] GoogleSheetSO.asset에서 {_sheetData.itemList.Count}개의 아이템을 로드했습니다.");
    }

    /// <summary>
    /// ItemID로 GoogleSheetSO.asset의 item 데이터를 직접 조회합니다.
    /// GoogleSheetSO.itemDict를 사용하여 빠르게 조회합니다.
    /// 
    /// 사용 예시:
    /// <code>
    /// item sheetItem = ItemDatabase.Instance.GetItemById("1");
    /// if (sheetItem != null)
    /// {
    ///     Debug.Log($"아이템 이름: {sheetItem.Name}");
    /// }
    /// </code>
    /// </summary>
    /// <param name="itemId">조회할 아이템의 ID (구글 시트의 itemID 문자열 값)</param>
    /// <returns>GoogleSheetSO.asset의 item 객체, 없으면 null</returns>
    public item GetItemById(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[ItemDatabase] ItemId가 비어있습니다.");
            return null;
        }

        if (_sheetData == null || _sheetData.itemDict == null)
        {
            Debug.LogWarning("[ItemDatabase] GoogleSheetSO 또는 itemDict가 초기화되지 않았습니다.");
            return null;
        }

        if (_sheetData.itemDict.TryGetValue(itemId, out var sheetItem))
        {
            return sheetItem;
        }

        Debug.LogWarning($"[ItemDatabase] ItemId '{itemId}'를 찾을 수 없습니다.");
        return null;
    }

    /// <summary>
    /// 등록된 모든 아이템을 반환합니다.
    /// GoogleSheetSO.asset의 itemList를 직접 반환합니다.
    /// 
    /// 사용 예시:
    /// <code>
    /// foreach (item sheetItem in ItemDatabase.Instance.GetAllItems())
    /// {
    ///     Debug.Log($"아이템: {sheetItem.Name}");
    /// }
    /// </code>
    /// </summary>
    /// <returns>GoogleSheetSO.asset의 itemList</returns>
    public IEnumerable<item> GetAllItems()
    {
        if (_sheetData == null || _sheetData.itemList == null)
            yield break;

        foreach (var sheetItem in _sheetData.itemList)
        {
            yield return sheetItem;
        }
    }

}
