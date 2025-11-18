using UnityEngine;
using TMPro;

/// <summary>
/// 걍 참고용
/// 현재 누적된 아이템 보너스를 표시하는 UI입니다.
/// 
/// 사용 방법:
/// 1. 씬에 Canvas 생성
/// 2. GameObject에 이 컴포넌트 추가
/// 3. 인스펙터에서 TextMeshProUGUI 참조 연결
/// 
/// UI 구조 예시:
/// - ItemBonusPanel (Panel)
///   - BonusText (TextMeshProUGUI) - 보너스 정보 표시
/// </summary>
public class ItemBonusDisplayUI : MonoBehaviour
{
    [Header("UI 참조")]
    [Tooltip("보너스 정보를 표시할 텍스트")]
    public TextMeshProUGUI bonusText;

    [Header("설정")]
    [Tooltip("표시할 직업 (null이면 모든 직업 표시)")]
    public Job displayJob = Job.Warrior;

    [Tooltip("자동 업데이트 여부")]
    public bool autoUpdate = true;

    [Tooltip("업데이트 주기 (초)")]
    public float updateInterval = 0.5f;

    private float _lastUpdateTime;

    private void Update()
    {
        if (autoUpdate && Time.time - _lastUpdateTime >= updateInterval)
        {
            UpdateDisplay();
            _lastUpdateTime = Time.time;
        }
    }

    /// <summary>
    /// 보너스 정보를 수동으로 업데이트합니다.
    /// </summary>
    public void UpdateDisplay()
    {
        if (bonusText == null) return;

        StatData bonus = ItemBonusManager.Instance.GetItemBonus(displayJob);
        bonusText.text = FormatBonusText(bonus, displayJob);
    }

    /// <summary>
    /// 특정 직업의 보너스를 표시합니다.
    /// </summary>
    public void SetDisplayJob(Job job)
    {
        displayJob = job;
        UpdateDisplay();
    }

    /// <summary>
    /// 보너스 정보를 텍스트로 포맷팅합니다.
    /// </summary>
    private string FormatBonusText(StatData bonus, Job job)
    {
        string jobName = job == Job.All ? "전체" : job.ToString();
        string result = $"<b>[{jobName}] 아이템 보너스</b>\n";

        bool hasBonus = false;

        if (bonus.Hp != 0)
        {
            result += $"HP: +{bonus.Hp}\n";
            hasBonus = true;
        }
        if (bonus.Mp != 0)
        {
            result += $"MP: +{bonus.Mp}\n";
            hasBonus = true;
        }
        if (bonus.Atk != 0)
        {
            result += $"공격력: +{bonus.Atk}\n";
            hasBonus = true;
        }
        if (bonus.Def != 0)
        {
            result += $"방어력: +{bonus.Def}\n";
            hasBonus = true;
        }
        if (bonus.Speed != 0)
        {
            result += $"이동속도: +{bonus.Speed}\n";
            hasBonus = true;
        }
        if (bonus.AtkSpeed != 0)
        {
            result += $"공격속도: +{bonus.AtkSpeed}\n";
            hasBonus = true;
        }
        if (bonus.Crit != 0)
        {
            result += $"치명타 확률: +{bonus.Crit * 100:F1}%\n";
            hasBonus = true;
        }
        if (bonus.CritD != 0)
        {
            result += $"치명타 피해: +{bonus.CritD * 100:F1}%\n";
            hasBonus = true;
        }
        if (bonus.HpRegen != 0)
        {
            result += $"HP 재생: +{bonus.HpRegen}\n";
            hasBonus = true;
        }
        if (bonus.MpRegen != 0)
        {
            result += $"MP 재생: +{bonus.MpRegen}\n";
            hasBonus = true;
        }

        if (!hasBonus)
        {
            result += "보너스 없음";
        }

        return result;
    }

    private void OnEnable()
    {
        UpdateDisplay();
    }
}

