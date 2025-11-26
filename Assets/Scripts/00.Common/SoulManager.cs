using System;
using UnityEngine;

/// <summary>
/// 테스트 및 치트용 영혼석 관리자입니다.
/// GlobalUpgradeManager가 있음에도 별도로 영혼석을 조작하고 싶을 때 사용합니다.
/// </summary>
public class SoulManager : MonoSingleton<SoulManager>
{
    // GlobalUpgradeManager를 래핑하여 사용
    // 실제 데이터는 GlobalUpgradeManager에 있지만,
    // GoldManager처럼 간편하게 호출하거나 인스펙터에서 조작하기 위한 용도

    [Header("Debug / Cheat")]
    [SerializeField] private int addAmount = 100;
    
    [Tooltip("이 값을 변경하고 Context Menu의 'Set Soul (Cheat)'를 실행하면 해당 값으로 설정됩니다.")]
    [SerializeField] private int setAmount = 0;

    public int CurrentSoul => GlobalUpgradeManager.Instance != null ? GlobalUpgradeManager.Instance.CurrentSoul : 0;

    [ContextMenu("Add Soul (Cheat)")]
    public void AddSoulCheat()
    {
        AddSoul(addAmount);
    }

    [ContextMenu("Set Soul (Cheat)")]
    public void SetSoulCheat()
    {
        if (GlobalUpgradeManager.Instance != null)
        {
            GlobalUpgradeManager.Instance.SetSoulForce(setAmount);
        }
    }

    public void AddSoul(int amount)
    {
        if (GlobalUpgradeManager.Instance != null)
        {
            GlobalUpgradeManager.Instance.AddSoul(amount);
        }
    }

    public bool SpendSoul(int amount)
    {
        if (GlobalUpgradeManager.Instance != null && GlobalUpgradeManager.Instance.CurrentSoul >= amount)
        {
            GlobalUpgradeManager.Instance.SpendSoulForce(amount);
            return true;
        }
        return false;
    }

    private void Update()
    {
        // 치트키: Ctrl + Alt + S 누르면 영혼석 100개 추가
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.S))
        {
            AddSoul(100);
            Debug.Log("[SoulManager] Cheat Activated: +100 Soul");
        }
    }
}

