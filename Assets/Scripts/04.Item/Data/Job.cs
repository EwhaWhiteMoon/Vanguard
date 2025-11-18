using UnityEngine;


public enum Job
{
    /// <summary>
    /// 모든 직업에게 적용되는 아이템에 사용됩니다.
    /// ItemBonusManager에서 GetItemBonus 호출 시, All 보너스도 함께 포함됩니다.
    /// </summary>
    All,

    Warrior,

    Archer,

    Mage,

    Assassin,

    Tanker,

    Healer
}

