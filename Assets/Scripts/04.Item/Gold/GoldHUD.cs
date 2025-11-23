using TMPro;
using UnityEngine;

/// <summary>
/// 현재 골드를 표시하는 UI입니다. (Combat/AfterCombat에서만 표시)
/// </summary>
public class GoldHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    private GoldManager _manager;
    private GameManager _gameManager;

    private void Start()
    {
        _manager = FindFirstObjectByType<GoldManager>();
        if (_manager != null)
        {
            _manager.OnGoldChanged += HandleGoldChanged;
            HandleGoldChanged(_manager.CurrentGold);
        }

        _gameManager = FindFirstObjectByType<GameManager>();
        if (_gameManager != null)
        {
            _gameManager.OnGameStateChange += HandleGameStateChanged;
            HandleGameStateChanged(_gameManager.GameState);
        }
    }

    private void OnDisable()
    {
        if (_manager != null)
        {
            _manager.OnGoldChanged -= HandleGoldChanged;
            _manager = null;
        }
        if (_gameManager != null)
        {
            _gameManager.OnGameStateChange -= HandleGameStateChanged;
            _gameManager = null;
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        bool shouldShow = state == GameState.Combat || state == GameState.AfterCombat;
        if (goldText != null) goldText.enabled = shouldShow;
    }

    private void HandleGoldChanged(int value)
    {
        if (goldText != null) goldText.text = $"Gold : {value}";
    }
}
