using UnityEngine;

public enum RewardType
{
    Item,
    Gold,
    Unit,
    Soul
}

public class UnitRewardInfo
{
    public UnitClass unitClass;
    public UnitGrade unitGrade;

    public string GetUnitId() {
        return $"{(int)unitClass}{(int)unitGrade}";
    }
}

/// <summary>
/// 보상 슬롯 하나에 대한 데이터를 담는 구조체입니다.
/// UI 표시용 정보와 실제 적용 데이터를 모두 포함합니다.
/// </summary>
public class RewardOption
{
    public RewardType Type;
    public item ItemData;
    public UnitRewardInfo UnitData;
    public int GoldAmount;
    public int SoulAmount; // 영혼석 수량
    public Sprite Icon;
    public string Title;
    public string Description;
}


