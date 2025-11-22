using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투 승리 이후 자동으로 골드/아이템/유닛 보상 슬롯을 생성하고 선택 결과를 적용하는 컨트롤러입니다.
/// GameManager의 상태를 구독해 패널 표시를 제어합니다.
/// </summary>
public class RewardPanelController : MonoBehaviour
{
    [SerializeField] private RewardSlotUI[] slots;
    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite defaultUnitIcon;
    [SerializeField] private UnitIconEntry[] unitIcons;
    [SerializeField] private string[] unitRewardIds = { "00", "10", "20", "30", "40", "50" };
    [SerializeField] private string goldDescription = "골드를 획득합니다.";

    private readonly List<RewardOption> _currentOptions = new List<RewardOption>();
    private readonly Dictionary<string, Sprite> _unitIconLookup = new Dictionary<string, Sprite>();
    private bool _rewardChosen;

    public event Action OnRewardFinished;

    private void Awake()
    {
        BuildUnitIconLookup();
        gameObject.SetActive(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChange += HandleGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        // OnDestroy 시점에는 싱글톤이 이미 파괴되었을 수 있으므로 FindFirstObjectByType으로 직접 확인
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnGameStateChange -= HandleGameStateChanged;
        }
    }

    private void BuildUnitIconLookup()
    {
        _unitIconLookup.Clear();
        if (unitIcons == null)
            return;

        foreach (var entry in unitIcons)
        {
            if (entry == null || string.IsNullOrEmpty(entry.unitId) || entry.icon == null)
                continue;

            _unitIconLookup[entry.unitId] = entry.icon;
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.AfterCombat)
        {
            ShowRewardPanel();
        }
        else
        {
            HideRewardPanel();
        }
    }

    private void ShowRewardPanel()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning("[RewardPanelController] 슬롯이 설정되지 않았습니다.");
            return;
        }

        GenerateRewards();

        for (int i = 0; i < slots.Length; i++)
        {
            RewardOption option = i < _currentOptions.Count ? _currentOptions[i] : null;
            slots[i].SetOption(option, HandleRewardSelected);
        }

        _rewardChosen = false;
        gameObject.SetActive(true);
    }

    private void HideRewardPanel()
    {
        gameObject.SetActive(false);
    }

    private void GenerateRewards()
    {
        _currentOptions.Clear();

        for (int i = 0; i < slots.Length; i++)
        {
            RewardOption option = CreateRandomReward();
            if (option == null)
            {
                option = CreateGoldReward(); // 최소한 골드라도 나오도록
            }

            _currentOptions.Add(option);
        }
    }

    private RewardOption CreateRandomReward()
    {
        RewardType randomType = (RewardType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(RewardType)).Length);
        switch (randomType)
        {
            case RewardType.Item:
                return CreateItemReward();
            case RewardType.Unit:
                return CreateUnitReward();
            default:
                return CreateGoldReward();
        }
    }

    private RewardOption CreateGoldReward()
    {
        int amount = UnityEngine.Random.Range(50, 201);

        return new RewardOption
        {
            Type = RewardType.Gold,
            GoldAmount = amount,
            Icon = goldIcon,
            Title = $"{amount} 골드",
            Description = goldDescription
        };
    }

    private RewardOption CreateItemReward()
    {
        var items = CollectItems();
        if (items.Count == 0)
            return null;

        item selected = items[UnityEngine.Random.Range(0, items.Count)];
        Sprite icon = ItemVisualHelper.Instance != null ? ItemVisualHelper.Instance.GetIcon(selected.itemID) : null;
        string description = ItemVisualHelper.Instance != null ? ItemVisualHelper.Instance.GetDescription(selected.itemID) : selected.description;

        return new RewardOption
        {
            Type = RewardType.Item,
            ItemData = selected,
            Icon = icon,
            Title = selected.Name,
            Description = description
        };
    }

    private RewardOption CreateUnitReward()
    {
        var candidates = CollectUnits();
        if (candidates.Count == 0)
            return null;

        unit selected = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        Sprite icon = GetUnitIcon(selected.unitID);

        return new RewardOption
        {
            Type = RewardType.Unit,
            UnitData = selected,
            Icon = icon ?? defaultUnitIcon,
            Title = selected.unitID,
            Description = $"{selected.Job} 유닛을 영입합니다."
        };
    }

    private List<item> CollectItems()
    {
        List<item> result = new List<item>();
        if (ItemDatabase.Instance == null)
            return result;

        foreach (var data in ItemDatabase.Instance.GetAllItems())
        {
            if (data != null)
            {
                result.Add(data);
            }
        }

        return result;
    }

    private List<unit> CollectUnits()
    {
        List<unit> result = new List<unit>();
        if (UnitDatabase.Instance == null || unitRewardIds == null || unitRewardIds.Length == 0)
        {
            Debug.LogWarning("[RewardPanelController] UnitDatabase가 없거나 unitRewardIds가 비어있습니다.");
            return result;
        }

        foreach (string id in unitRewardIds)
        {
            var unitData = UnitDatabase.Instance.GetUnitById(id);
            if (unitData != null)
            {
                result.Add(unitData);
            }
            else
            {
                Debug.LogWarning($"[RewardPanelController] unitId '{id}' 유닛 데이터를 찾을 수 없습니다.");
            }
        }

        return result;
    }

    private Sprite GetUnitIcon(string unitId)
    {
        if (string.IsNullOrEmpty(unitId))
            return null;

        if (_unitIconLookup.TryGetValue(unitId, out var sprite))
        {
            return sprite;
        }

        return defaultUnitIcon;
    }

    private void HandleRewardSelected(RewardOption option)
    {
        if (_rewardChosen || option == null)
            return;

        _rewardChosen = true;
        ApplyReward(option);
        HideRewardPanel();
        OnRewardFinished?.Invoke();
    }

    private void ApplyReward(RewardOption option)
    {
        switch (option.Type)
        {
            case RewardType.Gold:
                GoldManager.Instance?.AddGold(option.GoldAmount);
                Debug.Log($"[RewardPanel] 골드 {option.GoldAmount} 획득");
                break;
            case RewardType.Item:
                if (option.ItemData != null)
                {
                    ItemEffectManager.Instance?.OnGet(option.ItemData.itemID);
                    Debug.Log($"[RewardPanel] 아이템 획득: {option.ItemData.Name}");
                }
                break;
            case RewardType.Unit:
                if (option.UnitData != null)
                {
                    var roster = FindFirstObjectByType<PlayerUnitRoster>();
                    if (roster != null)
                    {
                        roster.AddUnit(option.UnitData);
                        Debug.Log($"[RewardPanel] 유닛 획득: {option.UnitData.unitID}");
                    }
                    else
                    {
                        Debug.LogWarning("[RewardPanel] PlayerUnitRoster를 찾을 수 없습니다. 유닛은 저장되지 않았습니다.");
                    }
                }
                break;
        }
    }

    [Serializable]
    public class UnitIconEntry
    {
        public string unitId;
        public Sprite icon;
    }
}


