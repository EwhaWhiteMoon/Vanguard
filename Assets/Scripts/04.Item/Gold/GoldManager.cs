using System;
using UnityEngine;

/// <summary>
/// 플레이어의 골드를 관리하는 싱글톤입니다.
/// 지금 단계에서는 값 저장과 변경 알림, HUD 갱신만 담당합니다.
/// </summary>
public class GoldManager : MonoSingleton<GoldManager>
{
    [SerializeField]
    private int currentGold;

    /// <summary>
    /// 골드 변경 시 호출됩니다. 인자로 현재 골드 값을 전달합니다.
    /// </summary>
    public event Action<int> OnGoldChanged;

    public int CurrentGold => currentGold;

    private void Start()
    {
        NotifyChange();
    }

    /// <summary>
    /// 골드를 지정한 값으로 초기화합니다. 기본값은 0입니다.
    /// </summary>
    /// <param name="value">초기 골드 값</param>
    public void ResetGold(int value = 500)
    {
        currentGold = value;
        NotifyChange();
    }

    /// <summary>
    /// 지정된 양만큼 골드를 추가합니다. 음수 값은 무시됩니다.
    /// </summary>
    /// <param name="amount">추가할 골드 양</param>
    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        currentGold += amount;
        NotifyChange();
    }

    /// <summary>
    /// 골드를 소비합니다. 충분한 골드가 없으면 false를 반환합니다.
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount <= 0 || currentGold < amount)
            return false;

        currentGold -= amount;
        NotifyChange();
        return true;
    }

    private void NotifyChange()
    {
        OnGoldChanged?.Invoke(currentGold);
        Debug.Log($"[GoldManager] 현재 골드: {currentGold}");
    }
}


