using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShopOptionType
{
    Item,
    Unit
}

public class ShopOption
{
    public ShopOptionType Type;
    public item ItemData;
    public unit UnitData;
    public Sprite Icon; // 아이콘 추가

    public string Name => Type == ShopOptionType.Item ? ItemData.Name : UnitData.Job;
    public int Price => Type == ShopOptionType.Item ? ItemData.Price : UnitData.Price;
    public string ID => Type == ShopOptionType.Item ? ItemData.itemID : UnitData.unitID;
    public string Description => Type == ShopOptionType.Item ? ItemData.description : $"{UnitData.Job} 유닛";
}

/// <summary>
/// 상점 시스템을 총괄하는 매니저입니다.
/// 맵 시스템에서 상점 노드 진입 시 ShopManager.Instance.ShowShop()을 호출하면 됩니다.
/// </summary>
public class ShopManager : MonoSingleton<ShopManager>
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private ShopSlotUI[] shopSlots;
    [SerializeField] private GameObject closeButton;

    [Header("Shop Settings")]
    [SerializeField] private int itemsPerShop = 6;
    [SerializeField] private Sprite defaultUnitIcon; // 기본 유닛 아이콘 추가

    private List<ShopOption> _allShopOptions = new List<ShopOption>();

    private void Start()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    /// <summary>
    /// 상점 UI를 열고 무작위 상품을 진열합니다.
    /// </summary>
    public void ShowShop()
    {
        var mapManager = FindFirstObjectByType<MapManager>();
        if (mapManager != null && mapManager.getCurrentRoomType() != RoomType.EventRoom)
        {
            Debug.LogWarning("[ShopManager] 현재 방이 상점(EventRoom)이 아니므로 상점을 열지 않습니다.");
            return;
        }

        if (shopPanel != null)
            shopPanel.SetActive(true);

        RefreshShopItems();
    }

    /// <summary>
    /// 상점 UI를 닫습니다.
    /// </summary>
    public void HideShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void RefreshShopItems()
    {
        LoadShopOptions();

        if (_allShopOptions.Count == 0)
        {
            Debug.LogWarning("[ShopManager] 판매할 상품이 없습니다 (Price > 0인 아이템/유닛 없음).");
            return;
        }

        // 랜덤으로 N개 선택
        var selectedOptions = _allShopOptions
            .OrderBy(x => Random.value)
            .Take(Mathf.Min(itemsPerShop, shopSlots.Length))
            .ToList();

        // 슬롯에 배치
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (i < selectedOptions.Count)
            {
                shopSlots[i].gameObject.SetActive(true);
                shopSlots[i].Setup(selectedOptions[i], HandlePurchase);
            }
            else
            {
                shopSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void LoadShopOptions()
    {
        _allShopOptions.Clear();

        // 1. 아이템 로드
        if (ItemDatabase.Instance != null)
        {
            foreach (var itemData in ItemDatabase.Instance.GetAllItems())
            {
                if (itemData != null && itemData.Price > 0)
                {
                    Sprite icon = ItemVisualHelper.Instance != null ? ItemVisualHelper.Instance.GetIcon(itemData.itemID) : null;
                    _allShopOptions.Add(new ShopOption
                    {
                        Type = ShopOptionType.Item,
                        ItemData = itemData,
                        Icon = icon
                    });
                }
            }
        }

        // 2. 유닛 로드
        if (UnitDatabase.Instance != null)
        {
            foreach (var unitData in UnitDatabase.Instance.GetAllUnits())
            {
                if (unitData != null && unitData.Price > 0)
                {
                    Sprite icon = UnitVisualHelper.Instance != null
                        ? UnitVisualHelper.Instance.GetIcon(unitData.unitID)
                        : defaultUnitIcon;

                    _allShopOptions.Add(new ShopOption
                    {
                        Type = ShopOptionType.Unit,
                        UnitData = unitData,
                        Icon = icon
                    });
                }
            }
        }
    }

    private void HandlePurchase(ShopOption option, ShopSlotUI slot)
    {
        if (GoldManager.Instance == null) return;

        // 1. 골드 확인
        if (GoldManager.Instance.CurrentGold < option.Price)
        {
            Debug.Log("[ShopManager] 골드가 부족합니다.");
            return;
        }

        // 2. 골드 차감 및 지급
        if (GoldManager.Instance.SpendGold(option.Price))
        {
            if (option.Type == ShopOptionType.Item)
            {
                ItemEffectManager.Instance?.OnGet(option.ItemData.itemID);
                Debug.Log($"[ShopManager] 아이템 구매 성공: {option.Name}");
            }
            else if (option.Type == ShopOptionType.Unit)
            {
                // 유닛 로스터가 없는 경우를 대비해 안전하게 접근 (MonoSingleton 에러 방지)
                var roster = FindFirstObjectByType<PlayerUnitRoster>();
                if (roster != null)
                {
                    roster.AddUnit(new UnitData(option.UnitData.ToString(), UnitClass.Warrior, UnitGrade.Common));
                    Debug.Log($"[ShopManager] 유닛 구매 성공: {option.Name}");
                }
                else
                {
                    Debug.LogWarning($"[ShopManager] PlayerUnitRoster를 찾을 수 없습니다. 유닛({option.Name}) 구매는 되었으나 명단에 추가되지 않았습니다.");
                }
            }

            // 3. UI 갱신 (품절 처리)
            slot.SetSoldOut(true);
        }
    }
}
