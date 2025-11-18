using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// GoogleSheetJson.json을 런타임에 파싱해서 GoogleSheetSO 인스턴스를 만들어 돌려준다.
/// - 방법 A) Resources 폴더에서 TextAsset 로드
/// - 방법 B) 절대 경로(예: Assets/GenerateGoogleSheet/GoogleSheetJson.json)에서 읽기
/// </summary>
public static class GoogleSheetLoader
{
    // JSON 루트 래퍼: JSON의 루트 키(unit/item/synergy/unitTable)와 동일한 필드명을 사용해야 JsonUtility가 매핑 가능
    [Serializable]
    private class GoogleSheetRoot
    {
        public List<unit> unit;
        public List<item> item;
        public List<synergy> synergy;
        public List<unitTable> unitTable;
    }

    /// <summary>
    /// 방법 A: Resources에서 TextAsset으로 읽기
    /// 예) 파일 경로가 Assets/Resources/GenerateGoogleSheet/GoogleSheetJson.json 라면,
    ///     resourcePath = "GenerateGoogleSheet/GoogleSheetJson"
    /// </summary>
    public static GoogleSheetSO LoadFromResources(string resourcePath)
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
        if (jsonAsset == null)
        {
            Debug.LogError($"[GoogleSheetLoader] Resources.Load 실패: {resourcePath}");
            return null;
        }
        return ParseToScriptableObject(jsonAsset.text);
    }

    /// <summary>
    /// 방법 B: 절대/상대 경로에서 파일로 읽기
    /// 예) "Assets/GenerateGoogleSheet/GoogleSheetJson.json"
    /// </summary>
    public static GoogleSheetSO LoadFromFile(string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"[GoogleSheetLoader] 파일이 없습니다: {jsonPath}");
            return null;
        }
        string json = File.ReadAllText(jsonPath);
        return ParseToScriptableObject(json);
    }

    // List<TValue>를 {TKey} 키로 매핑해서 Dictionary<TKey, TValue>로 바꿔주는 유틸
    private static Dictionary<TKey, TValue> ToDict<TValue, TKey>(List<TValue> list, Func<TValue, TKey> keySelector)
    {
        var dict = new Dictionary<TKey, TValue>();
        if (list == null)
            return dict;

        foreach (var item in list)
        {
            if (item == null)
                continue;

            var key = keySelector(item);
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, item);
            }
            else
            {
                dict[key] = item;
                // 필요하면 여기서 중복 키는 무시하도록 바꿔도 됨
            }
        }

        return dict;
    }

    /// <summary>
    /// JSON 문자열을 GoogleSheetSO로 변환
    /// </summary>
    private static GoogleSheetSO ParseToScriptableObject(string json)
    {
        // JsonUtility는 루트에 바로 리스트가 있으면 파싱이 안 되므로 래퍼 클래스로 받는다.
        GoogleSheetRoot root = JsonUtility.FromJson<GoogleSheetRoot>(json);
        if (root == null)
        {
            Debug.LogError("[GoogleSheetLoader] Json 파싱 실패 (루트 매핑 null)");
            return null;
        }

        // 메모리 상의 SO 인스턴스 생성 (프로젝트 에셋 저장 아님)
        var so = ScriptableObject.CreateInstance<GoogleSheetSO>();
        so.unitDict      = ToDict<unit, string>(root.unit,      u => u.unitID);
        so.itemDict      = ToDict<item, string>(root.item,      i => i.itemID.ToString());
        so.synergyDict   = ToDict<synergy, string>(root.synergy, s => s.synergyID.ToString());
        so.unitTableDict = ToDict<unitTable, string>(root.unitTable, t => t.unitTableID.ToString());

        return so;
    }
}
