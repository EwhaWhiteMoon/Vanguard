using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GoogleSheetSO에 직렬화된 유닛 데이터를 unitID로 조회하고, 필요 시 SO를 지연 로드하는 데이터베이스입니다.
/// 보상 시스템에서 유닛 보상 정보를 추출할 때 사용됩니다.
/// </summary>
public class UnitDatabase : MonoSingleton<UnitDatabase>
{
    private GoogleSheetSO _sheetData;

    private void Awake()
    {
        LoadUnitsFromGoogleSheet();
    }

    private void LoadUnitsFromGoogleSheet()
    {
        _sheetData = GoogleSheetManager.SO<GoogleSheetSO>();
        if (_sheetData == null || _sheetData.unitList == null)
        {
            Debug.LogWarning("[UnitDatabase] GoogleSheetSO를 찾을 수 없거나 unitList가 비어있습니다.");
            return;
        }

        if (_sheetData.unitDict == null)
        {
            _sheetData.BuildDictionaries();
        }

        Debug.Log($"[UnitDatabase] GoogleSheetSO.asset에서 {_sheetData.unitList.Count}개의 유닛을 로드했습니다.");
    }

    public unit GetUnitById(string unitId)
    {
        if (string.IsNullOrEmpty(unitId))
        {
            Debug.LogWarning("[UnitDatabase] unitId가 비어있습니다.");
            return null;
        }

        // 지연 초기화: 데이터가 없으면 다시 로드 시도
        if (_sheetData == null || _sheetData.unitDict == null)
        {
            Debug.LogWarning("[UnitDatabase] GoogleSheetSO 또는 unitDict가 초기화되지 않았습니다. 다시 로드 시도...");
            LoadUnitsFromGoogleSheet();
            
            if (_sheetData == null || _sheetData.unitDict == null)
            {
                Debug.LogError("[UnitDatabase] 데이터 로드 실패. GoogleSheetManager와 GoogleSheetSO가 제대로 설정되어 있는지 확인하세요.");
                return null;
            }
        }

        if (_sheetData.unitDict.TryGetValue(unitId, out var unitData))
        {
            return unitData;
        }

        Debug.LogWarning($"[UnitDatabase] unitId '{unitId}'를 찾을 수 없습니다.");
        return null;
    }

    public IReadOnlyList<unit> GetAllUnits()
    {
        return _sheetData?.unitList;
    }
}


