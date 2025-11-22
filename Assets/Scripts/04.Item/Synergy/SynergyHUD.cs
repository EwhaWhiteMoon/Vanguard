using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// 직업별 시너지 진행도를 표시하는 UI입니다. (Combat/AfterCombat에서만 표시)
/// </summary>
public class SynergyHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI synergyText;
    private SynergyManager _manager;
    private GameManager _gameManager;

    private static readonly Job[] DisplayJobs =
    {
        Job.Warrior, Job.Archer, Job.Mage,
        Job.Assassin, Job.Tanker, Job.Healer
    };

    private void Start()
    {
        _manager = FindFirstObjectByType<SynergyManager>();
        if (_manager != null)
        {
            _manager.OnSynergyUpdated += HandleSynergyUpdated;
            HandleSynergyUpdated();
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
            _manager.OnSynergyUpdated -= HandleSynergyUpdated;
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
        if (synergyText != null) synergyText.enabled = shouldShow;
    }

    private void HandleSynergyUpdated()
    {
        if (synergyText == null || _manager == null) return;

        var builder = new StringBuilder("시너지: ");

        for (int i = 0; i < DisplayJobs.Length; i++)
        {
            Job job = DisplayJobs[i];
            int current = _manager.GetCurrentUniqueCount(job);
            int max = _manager.GetMaxRequiredCount(job);
            
            builder.Append($"{job}({current}/{Mathf.Max(max, 0)})");

            if (i < DisplayJobs.Length - 1) builder.Append(" ");
        }

        synergyText.text = builder.ToString();
        if (synergyText != null) synergyText.textWrappingMode = TextWrappingModes.NoWrap;
    }
}
