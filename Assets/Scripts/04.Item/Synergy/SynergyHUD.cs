using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// 직업별 시너지 진행도를 HUD에 표시하는 컴포넌트입니다.
/// </summary>
public class SynergyHUD : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI synergyText;

    private SynergyManager _manager;

    private static readonly Job[] DisplayJobs =
    {
        Job.Warrior,
        Job.Archer,
        Job.Mage,
        Job.Assassin,
        Job.Tanker,
        Job.Healer
    };

    private void OnEnable()
    {
        _manager = FindFirstObjectByType<SynergyManager>();
        if (_manager != null)
        {
            _manager.OnSynergyUpdated += HandleSynergyUpdated;
            HandleSynergyUpdated();
        }
        else
        {
            Debug.LogWarning("[SynergyHUD] SynergyManager를 찾지 못했습니다. 씬에 SynergyManager가 존재하는지 확인하세요.");
        }
    }

    private void OnDisable()
    {
        if (_manager != null)
        {
            _manager.OnSynergyUpdated -= HandleSynergyUpdated;
            _manager = null;
        }
    }

    private void HandleSynergyUpdated()
    {
        if (synergyText == null || _manager == null)
            return;

        var builder = new StringBuilder("시너지: ");

        for (int i = 0; i < DisplayJobs.Length; i++)
        {
            Job job = DisplayJobs[i];
            int current = _manager.GetCurrentUniqueCount(job);
            int max = _manager.GetMaxRequiredCount(job);
            string jobLabel = job.ToString();
            builder.Append($"{jobLabel}({current}/{Mathf.Max(max, 0)})");

            if (i < DisplayJobs.Length - 1)
            {
                builder.Append(" ");
            }
        }

        synergyText.text = builder.ToString();
        
        // 줄바꿈 방지 (TextMeshProUGUI 설정)
        if (synergyText != null)
        {
            synergyText.textWrappingMode = TextWrappingModes.NoWrap;
        }
    }
}


