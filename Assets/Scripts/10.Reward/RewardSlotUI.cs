using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 보상 슬롯 UI를 제어하는 컴포넌트입니다.
/// </summary>
public class RewardSlotUI : MonoBehaviour
{
    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button selectButton;

    private RewardOption _option;
    private Action<RewardOption> _onSelected;

    private void Awake()
    {
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(HandleClick);
        }
    }

    public void SetOption(RewardOption option, Action<RewardOption> onSelected)
    {
        _option = option;
        _onSelected = onSelected;

        if (rewardImage != null)
        {
            rewardImage.sprite = option?.Icon;
            rewardImage.enabled = rewardImage.sprite != null;
        }

        if (titleText != null)
        {
            titleText.text = option?.Title ?? string.Empty;
        }

        if (descriptionText != null)
        {
            descriptionText.text = option?.Description ?? string.Empty;
        }

        if (selectButton != null)
        {
            selectButton.interactable = option != null;
        }
    }

    private void HandleClick()
    {
        if (_option == null)
            return;

        //한윤구 추가
        //보상 선택 효과음
        SoundManager.Instance?.PlaySFX("GetReward");

        _onSelected?.Invoke(_option);
    }
}


