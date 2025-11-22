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
            if (nameText != null) nameText.text = _currentOption.Name;
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
