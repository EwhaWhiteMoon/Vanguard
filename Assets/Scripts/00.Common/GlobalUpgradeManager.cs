using System;
using UnityEngine;

/// <summary>
/// 게임 간 영구적인 스탯 강화를 관리하는 매니저입니다.
/// </summary>
public class GlobalUpgradeManager : MonoSingleton<GlobalUpgradeManager>
{
    private const string CURRENCY_KEY = "Player_Soul";
    private const string UPGRADE_KEY_PREFIX = "Upgrade_";

    public int CurrentSoul { get; private set; }

    // 각 스탯별 레벨
    public int HealthLevel { get; private set; }
    public int AttackLevel { get; private set; }
    public int DefenseLevel { get; private set; }
    public int CritChanceLevel { get; private set; }
    public int MoveSpeedLevel { get; private set; }
    public int AttackSpeedLevel { get; private set; }
    public int CritDamageLevel { get; private set; }

    public event Action OnCurrencyChanged;
    public event Action OnUpgradeChanged;

    private void Start()
    {
        LoadData();
    }

    public void AddSoul(int amount)
    {
        CurrentSoul += amount;
        PlayerPrefs.SetInt(CURRENCY_KEY, CurrentSoul);
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke();
        Debug.Log($"[GlobalUpgradeManager] 소울 획득: {amount}, 현재 소울: {CurrentSoul}");
    }

    // 테스트/치트용 강제 설정 메서드
    public void SetSoulForce(int amount)
    {
        CurrentSoul = Mathf.Max(0, amount);
        PlayerPrefs.SetInt(CURRENCY_KEY, CurrentSoul);
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke();
        Debug.Log($"[GlobalUpgradeManager] 소울 강제 설정: {CurrentSoul}");
    }

    // 테스트/치트용 강제 소비 메서드
    public void SpendSoulForce(int amount)
    {
        CurrentSoul = Mathf.Max(0, CurrentSoul - amount);
        PlayerPrefs.SetInt(CURRENCY_KEY, CurrentSoul);
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke();
    }

    public bool TryUpgrade(StatUpgradeType type, int cost)
    {
        if (CurrentSoul < cost) return false;

        CurrentSoul -= cost;
        PlayerPrefs.SetInt(CURRENCY_KEY, CurrentSoul);

        switch (type)
        {
            case StatUpgradeType.Health:
                HealthLevel++;
                PlayerPrefs.SetInt(UPGRADE_KEY_PREFIX + "Health", HealthLevel);
                break;
            case StatUpgradeType.Attack:
                AttackLevel++;
                PlayerPrefs.SetInt(UPGRADE_KEY_PREFIX + "Attack", AttackLevel);
                break;
            case StatUpgradeType.Defense:
                DefenseLevel++;
                PlayerPrefs.SetInt(UPGRADE_KEY_PREFIX + "Defense", DefenseLevel);
                break;
            case StatUpgradeType.CritChance:
                CritChanceLevel++;
                PlayerPrefs.SetInt(UPGRADE_KEY_PREFIX + "CritChance", CritChanceLevel);
                break;
            case StatUpgradeType.MoveSpeed:
                MoveSpeedLevel++;
                PlayerPrefs.SetInt(UPGRADE_KEY_PREFIX + "MoveSpeed", MoveSpeedLevel);
                break;
            case StatUpgradeType.AttackSpeed:
                AttackSpeedLevel++;
                PlayerPrefs.SetInt(UPGRADE_KEY_PREFIX + "AttackSpeed", AttackSpeedLevel);
                break;
            case StatUpgradeType.CritDamage:
                CritDamageLevel++;
                PlayerPrefs.SetInt(UPGRADE_KEY_PREFIX + "CritDamage", CritDamageLevel);
                break;
        }

        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke();
        OnUpgradeChanged?.Invoke();
        
        Debug.Log($"[GlobalUpgradeManager] {type} 업그레이드 성공! Lv.{GetLevel(type)}");
        return true;
    }

    public int GetLevel(StatUpgradeType type)
    {
        switch (type)
        {
            case StatUpgradeType.Health: return HealthLevel;
            case StatUpgradeType.Attack: return AttackLevel;
            case StatUpgradeType.Defense: return DefenseLevel;
            case StatUpgradeType.CritChance: return CritChanceLevel;
            case StatUpgradeType.MoveSpeed: return MoveSpeedLevel;
            case StatUpgradeType.AttackSpeed: return AttackSpeedLevel;
            case StatUpgradeType.CritDamage: return CritDamageLevel;
            default: return 0;
        }
    }

    /// <summary>
    /// 현재 레벨에 따른 보너스 스탯을 반환합니다.
    /// </summary>
    public float GetStatBonus(StatUpgradeType type)
    {
        // 밸런스 조절 필요
        switch (type)
        {
            case StatUpgradeType.Health:
                return HealthLevel * 5f; // 레벨당 체력 +5
            case StatUpgradeType.Attack:
                return AttackLevel * 1f;  // 레벨당 공격력 +1
            case StatUpgradeType.Defense:
                return DefenseLevel * 0.1f; // 레벨당 방어력 +0.1
            case StatUpgradeType.CritChance:
                return CritChanceLevel * 0.01f; // 레벨당 치명타율 +1%
            case StatUpgradeType.MoveSpeed:
                return MoveSpeedLevel * 0.1f;   // 레벨당 이동속도 +0.1
            case StatUpgradeType.AttackSpeed:
                return AttackSpeedLevel * 0.05f; // 레벨당 공격속도 +0.05 (약 5%)
            case StatUpgradeType.CritDamage:
                return CritDamageLevel * 0.1f;   // 레벨당 치명타 데미지 +10% (0.1)
            default:
                return 0f;
        }
    }

    private void LoadData()
    {
        CurrentSoul = PlayerPrefs.GetInt(CURRENCY_KEY, 0);
        HealthLevel = PlayerPrefs.GetInt(UPGRADE_KEY_PREFIX + "Health", 0);
        AttackLevel = PlayerPrefs.GetInt(UPGRADE_KEY_PREFIX + "Attack", 0);
        DefenseLevel = PlayerPrefs.GetInt(UPGRADE_KEY_PREFIX + "Defense", 0);
        CritChanceLevel = PlayerPrefs.GetInt(UPGRADE_KEY_PREFIX + "CritChance", 0);
        MoveSpeedLevel = PlayerPrefs.GetInt(UPGRADE_KEY_PREFIX + "MoveSpeed", 0);
        AttackSpeedLevel = PlayerPrefs.GetInt(UPGRADE_KEY_PREFIX + "AttackSpeed", 0);
        CritDamageLevel = PlayerPrefs.GetInt(UPGRADE_KEY_PREFIX + "CritDamage", 0);
    }

    [ContextMenu("Reset Data")]
    public void ResetAllData()
    {
        PlayerPrefs.DeleteKey(CURRENCY_KEY);
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "Health");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "Attack");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "Defense");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "CritChance");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "MoveSpeed");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "AttackSpeed");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "CritDamage");
        PlayerPrefs.Save();
        LoadData();
        Debug.Log("[GlobalUpgradeManager] 데이터 초기화 완료");
    }

    /// <summary>
    /// 스탯 강화를 초기화하고 사용된 모든 소울을 환급받습니다.
    /// </summary>
    public void ResetUpgradesAndRefund()
    {
        int totalRefund = 0;

        // 각 스탯별로 레벨만큼 소모했던 비용을 역으로 계산하여 합산
        totalRefund += CalculateRefund(HealthLevel);
        totalRefund += CalculateRefund(AttackLevel);
        totalRefund += CalculateRefund(DefenseLevel);
        totalRefund += CalculateRefund(CritChanceLevel);
        totalRefund += CalculateRefund(MoveSpeedLevel);
        totalRefund += CalculateRefund(AttackSpeedLevel);
        totalRefund += CalculateRefund(CritDamageLevel);

        // 데이터 초기화 (소울은 제외)
        HealthLevel = 0;
        AttackLevel = 0;
        DefenseLevel = 0;
        CritChanceLevel = 0;
        MoveSpeedLevel = 0;
        AttackSpeedLevel = 0;
        CritDamageLevel = 0;

        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "Health");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "Attack");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "Defense");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "CritChance");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "MoveSpeed");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "AttackSpeed");
        PlayerPrefs.DeleteKey(UPGRADE_KEY_PREFIX + "CritDamage");

        // 소울 환급
        AddSoul(totalRefund);

        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke();
        OnUpgradeChanged?.Invoke();

        Debug.Log($"[GlobalUpgradeManager] 강화 초기화 완료. {totalRefund} 소울 환급됨.");
    }

    private int CalculateRefund(int currentLevel)
    {
        int refund = 0;
        // 0레벨부터 currentLevel-1 레벨까지 업그레이드 비용의 합
        for (int i = 0; i < currentLevel; i++)
        {
            // 비용 계산 식: 10 + (level * 5) -> UpgradeUI.cs의 GetCost와 동일해야 함
            // 안전성을 위해 여기서도 동일한 식 사용
            refund += 10 + (i * 5);
        }
        return refund;
    }
}

public enum StatUpgradeType
{
    Health,
    Attack,
    Defense,
    CritChance,
    MoveSpeed,
    AttackSpeed,
    CritDamage
}

