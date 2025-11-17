using UnityEngine;

/// <summary>
/// 유닛의 직업을 나타내는 열거형입니다.
/// 아이템이 특정 직업에게만 적용될지, 전체에게 적용될지 결정하는 데 사용됩니다.
/// </summary>
public enum Job
{
    /// <summary>
    /// 모든 직업에게 적용되는 아이템에 사용됩니다.
    /// ItemBonusManager에서 GetItemBonus 호출 시, All 보너스도 함께 포함됩니다.
    /// </summary>
    All,

    /// <summary>
    /// 전사 직업
    /// </summary>
    Warrior,

    /// <summary>
    /// 궁수 직업
    /// </summary>
    Archer,

    /// <summary>
    /// 마법사 직업
    /// </summary>
    Mage,

    /// <summary>
    /// 암살자 직업
    /// </summary>
    Assassin,

    /// <summary>
    /// 탱커 직업
    /// </summary>
    Tanker,

    /// <summary>
    /// 힐러 직업
    /// </summary>
    Healer
}

