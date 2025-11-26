using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI soulText;
    [SerializeField] private GameObject panel;

    [Header("Health Upgrade")]
    [SerializeField] private TextMeshProUGUI healthLevelText;
    [SerializeField] private TextMeshProUGUI healthCostText;
    [SerializeField] private Button healthButton;

    [Header("Attack Upgrade")]
    [SerializeField] private TextMeshProUGUI attackLevelText;
    [SerializeField] private TextMeshProUGUI attackCostText;
    [SerializeField] private Button attackButton;

    [Header("Defense Upgrade")]
    [SerializeField] private TextMeshProUGUI defenseLevelText;
    [SerializeField] private TextMeshProUGUI defenseCostText;
    [SerializeField] private Button defenseButton;

    [Header("CritChance Upgrade")]
    [SerializeField] private TextMeshProUGUI critLevelText;
    [SerializeField] private TextMeshProUGUI critCostText;
    [SerializeField] private Button critButton;

    [Header("MoveSpeed Upgrade")]
    [SerializeField] private TextMeshProUGUI moveSpeedLevelText;
    [SerializeField] private TextMeshProUGUI moveSpeedCostText;
    [SerializeField] private Button moveSpeedButton;

    [Header("AttackSpeed Upgrade")]
    [SerializeField] private TextMeshProUGUI attackSpeedLevelText;
    [SerializeField] private TextMeshProUGUI attackSpeedCostText;
    [SerializeField] private Button attackSpeedButton;

    [Header("CritDamage Upgrade")]
    [SerializeField] private TextMeshProUGUI critDamageLevelText;
    [SerializeField] private TextMeshProUGUI critDamageCostText;
    [SerializeField] private Button critDamageButton;

    [Header("Reset")]
    [SerializeField] private Button resetButton;

    private GlobalUpgradeManager _upgradeManager;

    // 업그레이드 비용 계산 로직 (예: 기본 10 + 레벨 * 5)
    private int GetCost(int level)
    {
        return 10 + (level * 5);
    }

    private void Start()
    {
        _upgradeManager = GlobalUpgradeManager.Instance;

        if (_upgradeManager != null)
        {
            _upgradeManager.OnCurrencyChanged += UpdateUI;
            _upgradeManager.OnUpgradeChanged += UpdateUI;
        }

        healthButton.onClick.AddListener(() => TryUpgrade(StatUpgradeType.Health));
        attackButton.onClick.AddListener(() => TryUpgrade(StatUpgradeType.Attack));
        defenseButton.onClick.AddListener(() => TryUpgrade(StatUpgradeType.Defense));
        critButton.onClick.AddListener(() => TryUpgrade(StatUpgradeType.CritChance));
        moveSpeedButton.onClick.AddListener(() => TryUpgrade(StatUpgradeType.MoveSpeed));
        attackSpeedButton.onClick.AddListener(() => TryUpgrade(StatUpgradeType.AttackSpeed));
        critDamageButton.onClick.AddListener(() => TryUpgrade(StatUpgradeType.CritDamage));
        
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(TryReset);
        }

        UpdateUI();
        
        // 초기에는 패널을 꺼둠 (버튼으로 열도록)
        // panel.SetActive(false); 
    }

    private void OnDestroy()
    {
        if (_upgradeManager != null)
        {
            _upgradeManager.OnCurrencyChanged -= UpdateUI;
            _upgradeManager.OnUpgradeChanged -= UpdateUI;
        }
    }

    public void TogglePanel()
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }

    private void TryUpgrade(StatUpgradeType type)
    {
        if (GlobalUpgradeManager.Instance == null) return;

        int currentLevel = GlobalUpgradeManager.Instance.GetLevel(type);
        int cost = GetCost(currentLevel);

        if (GlobalUpgradeManager.Instance.TryUpgrade(type, cost))
        {
            // 성공 효과음 등
            SoundManager.Instance?.PlaySFX("UpgradeSuccess");
        }
        else
        {
            // 실패 효과음 등
            SoundManager.Instance?.PlaySFX("UpgradeFail");
        }
    }

    private void TryReset()
    {
        if (GlobalUpgradeManager.Instance == null) return;
        
        GlobalUpgradeManager.Instance.ResetUpgradesAndRefund();
        SoundManager.Instance?.PlaySFX("UpgradeSuccess"); // 초기화 성공음 (임시로 UpgradeSuccess 사용)
    }

    private void UpdateUI()
    {
        if (GlobalUpgradeManager.Instance == null) return;

        int currentSoul = GlobalUpgradeManager.Instance.CurrentSoul;
        if (soulText != null) soulText.text = $"보유 영혼석: {currentSoul}";

        UpdateSlot(StatUpgradeType.Health, healthLevelText, healthCostText, healthButton, currentSoul);
        UpdateSlot(StatUpgradeType.Attack, attackLevelText, attackCostText, attackButton, currentSoul);
        UpdateSlot(StatUpgradeType.Defense, defenseLevelText, defenseCostText, defenseButton, currentSoul);
        UpdateSlot(StatUpgradeType.CritChance, critLevelText, critCostText, critButton, currentSoul);
        UpdateSlot(StatUpgradeType.MoveSpeed, moveSpeedLevelText, moveSpeedCostText, moveSpeedButton, currentSoul);
        UpdateSlot(StatUpgradeType.AttackSpeed, attackSpeedLevelText, attackSpeedCostText, attackSpeedButton, currentSoul);
        UpdateSlot(StatUpgradeType.CritDamage, critDamageLevelText, critDamageCostText, critDamageButton, currentSoul);
    }

    private void UpdateSlot(StatUpgradeType type, TextMeshProUGUI levelText, TextMeshProUGUI costText, Button button, int currentSoul)
    {
        int level = GlobalUpgradeManager.Instance.GetLevel(type);
        int cost = GetCost(level);

        if (levelText != null)
        {
            float bonus = GlobalUpgradeManager.Instance.GetStatBonus(type);
            
            string prefix = "";
            switch (type)
            {
                case StatUpgradeType.Health: prefix = "HP "; break;
                case StatUpgradeType.Attack: prefix = "Atk "; break;
                case StatUpgradeType.Defense: prefix = "Def "; break;
                case StatUpgradeType.CritChance: prefix = "Crit "; break;
                case StatUpgradeType.CritDamage: prefix = "CritDmg "; break;
                case StatUpgradeType.MoveSpeed: prefix = "Spd "; break;
                case StatUpgradeType.AttackSpeed: prefix = "AtkSpd "; break;
            }

            // 표시 형식: "HP Lv.0\n+0 => +5 (0/10)"
            // bonusText: 현재 적용 수치
            // nextBonusText: 다음 레벨 적용 수치 (증가량)

            string bonusText = "";
            string nextBonusText = "";
            
            bool isPercent = (type == StatUpgradeType.CritChance || type == StatUpgradeType.CritDamage);
            bool isDecimal = (type == StatUpgradeType.MoveSpeed || type == StatUpgradeType.AttackSpeed);

            // 다음 레벨 보너스 계산을 위해 임시로 레벨 계산
            // GlobalUpgradeManager의 공식: level * 증가량
            float currentBonus = GlobalUpgradeManager.Instance.GetStatBonus(type);
            
            // 증가량은 GlobalUpgradeManager 공식을 역추적하거나 별도 상수로 관리하는 게 좋지만
            // 여기서는 공식을 그대로 적용해서 차이를 계산 (다음 레벨 보너스 = (level+1) * 증가량)
            // 간단하게 '레벨 1일 때의 보너스'가 곧 '레벨당 증가량'임 (선형 증가이므로)
            // 단, 현재 GlobalUpgradeManager 구조상 GetStatBonus가 인스턴스 레벨 기반이라
            // 증가량을 얻기 위해 약간의 트릭을 사용하거나, 하드코딩된 증가량을 사용해야 함.
            
            float perLevelBonus = 0f;
            switch(type)
            {
                case StatUpgradeType.Health: perLevelBonus = 5f; break;
                case StatUpgradeType.Attack: perLevelBonus = 1f; break;
                case StatUpgradeType.Defense: perLevelBonus = 0.1f; break;
                case StatUpgradeType.CritChance: perLevelBonus = 0.01f; break;
                case StatUpgradeType.MoveSpeed: perLevelBonus = 0.1f; break;
                case StatUpgradeType.AttackSpeed: perLevelBonus = 0.05f; break;
                case StatUpgradeType.CritDamage: perLevelBonus = 0.1f; break;
            }
            
            float nextBonus = currentBonus + perLevelBonus;

            if (isPercent)
            {
                bonusText = $"{currentBonus * 100:0}%";
                nextBonusText = $"{nextBonus * 100:0}%";
            }
            else if (isDecimal)
            {
                bonusText = $"{currentBonus:0.##}";
                nextBonusText = $"{nextBonus:0.##}";
            }
            else
            {
                bonusText = $"{currentBonus:0}";
                nextBonusText = $"{nextBonus:0}";
            }

            // 최대 레벨 10 가정
            int maxLevel = 10; 
            
            if (level >= maxLevel)
            {
                // MAX 레벨이면 화살표 없이 현재 수치만
                levelText.text = $"{prefix}Lv.{level}\n<size=70%>+{bonusText}\n({level}/{maxLevel})</size>";
            }
            else
            {
                // 강화 가능하면 화살표로 표시: +0 -> +5
                levelText.text = $"{prefix}Lv.{level}\n<size=70%>+{bonusText} -> +{nextBonusText}\n({level}/{maxLevel})</size>";
            }
        }

        if (costText != null) costText.text = $"{cost} Soul";
        
        if (button != null)
        {
            // 최대 레벨 제한: 10레벨 이상이면 버튼 비활성화
            int maxLevel = 10;
            bool isMaxed = level >= maxLevel;
            bool canAfford = currentSoul >= cost;
            
            button.interactable = !isMaxed && canAfford;

            // (선택사항) 만약 Max 레벨이면 비용 텍스트를 "MAX"로 표시하고 싶다면:
            if (isMaxed && costText != null)
            {
                costText.text = "MAX";
            }
        }
    }
}

