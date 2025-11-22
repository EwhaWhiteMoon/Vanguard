using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 확보한 유닛 목록을 임시로 저장하는 매니저입니다.
/// 추후 유닛 시스템과 연동할 때 확장할 예정입니다.
/// </summary>
public class PlayerUnitRoster : MonoSingleton<PlayerUnitRoster>
{
    private readonly List<unit> _ownedUnits = new List<unit>();

    public IReadOnlyList<unit> OwnedUnits => _ownedUnits;

    public void AddUnit(unit unitData)
    {
        if (unitData == null)
        {
            Debug.LogWarning("[PlayerUnitRoster] null 유닛 데이터를 추가하려 했습니다.");
            return;
        }

        _ownedUnits.Add(unitData);
        Debug.Log($"[PlayerUnitRoster] 유닛 획득: {unitData.unitID} ({unitData.Job})");
    }
}


