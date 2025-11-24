using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점의 개별 상품 슬롯(아이템 또는 유닛)을 관리하는 UI 클래스입니다.
/// </summary>
public class ShopSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject soldOutOverlay;

    private ShopOption _currentOption;
    private Action<ShopOption, ShopSlotUI> _onBuyClick;

    public void Setup(ShopOption option, Action<ShopOption, ShopSlotUI> onBuyClick)
    {
        _currentOption = option;
        _onBuyClick = onBuyClick;

        // 데이터 바인딩
        if (_currentOption != null)
        {
            if (nameText != null)
            {
                string displayName = _currentOption.Name;
                bool isOwned = false;

                if (_currentOption.Type == ShopOptionType.Item)
                {
                    isOwned = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(_currentOption.ID);
                }
                else if (_currentOption.Type == ShopOptionType.Unit)
                {
                    // unitID 파싱 (예: "00", "10" -> Class + Grade)
                    // 여기서는 간단히 unitID가 있으므로 그것을 활용하거나, UnitData 자체에서 Class/Grade를 가져올 수 있다면 좋음.
                    // 하지만 현재 ShopOption.UnitData는 `unit` (구글시트 데이터) 타입임.
                    // unitID는 string이므로 파싱해야 함. 
                    // unitID 구조: (int)UnitClass + (int)UnitGrade (각 1자리라고 가정하면 위험할 수 있으나 현재 구조상 그러함)
                    // 안전하게 파싱하려면 별도 로직 필요. 
                    // 다행히 unit 클래스에 unitID가 있으므로 이를 통해 확인 가능.
                    
                    // ShopManager에서 로드할 때 UnitData(unit)를 넣어줬음.
                    // unitID 문자열을 파싱해서 enum으로 변환
                    if (int.TryParse(_currentOption.UnitData.unitID, out int idInt))
                    {
                        // 예: 10 -> 1 (Class), 0 (Grade)
                        // 예: 50 -> 5 (Class), 0 (Grade)
                        // 100 이상인 적 유닛은 상점에 안 나올 것으로 가정.
                        int classInt = idInt / 10; 
                        int gradeInt = idInt % 10;
                        isOwned = PlayerUnitRoster.Instance != null && PlayerUnitRoster.Instance.HasUnit((UnitClass)classInt, (UnitGrade)gradeInt);
                    }
                }

                if (isOwned)
                {
                    displayName += "\n(보유중)";
                    // 필요하다면 색상 변경 등 추가 처리 가능
                    // nameText.color = Color.yellow; 
                }
                nameText.text = displayName;
            }
            if (priceText != null) priceText.text = $"{_currentOption.Price} G";
            
            // 아이콘 설정
            if (iconImage != null)
            {
                iconImage.sprite = _currentOption.Icon;
                iconImage.enabled = _currentOption.Icon != null;
            }

            // 설명 설정
            if (descText != null)
            {
                if (_currentOption.Type == ShopOptionType.Item && ItemVisualHelper.Instance != null)
                {
                    // 아이템이면 비주얼 헬퍼에서 설명 가져오기 시도
                    string helperDesc = ItemVisualHelper.Instance.GetDescription(_currentOption.ID);
                    descText.text = string.IsNullOrEmpty(helperDesc) ? _currentOption.Description : helperDesc;
                }
                else
                {
                    descText.text = _currentOption.Description;
                }
            }
        }

        // 상태 초기화
        SetSoldOut(false);
        
        // 버튼 이벤트 연결
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => _onBuyClick?.Invoke(_currentOption, this));
        }
    }

    public void SetSoldOut(bool isSoldOut)
    {
        if (buyButton != null) buyButton.interactable = !isSoldOut;
        if (soldOutOverlay != null) soldOutOverlay.SetActive(isSoldOut);
    }
}
