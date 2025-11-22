using TMPro;
using UnityEngine;

/// <summary>
/// 화면 상단에 현재 골드를 표시하는 HUD 컴포넌트입니다.
/// </summary>
public class GoldHUD : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;

    private GoldManager _manager;

    private void OnEnable()
    {
        _manager = FindFirstObjectByType<GoldManager>();
        if (_manager == null)
        {
            Debug.LogWarning("[GoldHUD] GoldManager를 찾지 못했습니다. 씬에 GoldManager가 존재하는지 확인하세요.");
            return;
        }

        _manager.OnGoldChanged += HandleGoldChanged;
        HandleGoldChanged(_manager.CurrentGold);
    }

    private void OnDisable()
    {
        if (_manager != null)
        {
            _manager.OnGoldChanged -= HandleGoldChanged;
            _manager = null;
        }
    }

    private void HandleGoldChanged(int value)
    {
        if (goldText == null)
            return;

        goldText.text = $"Gold : {value}";
    }
}


