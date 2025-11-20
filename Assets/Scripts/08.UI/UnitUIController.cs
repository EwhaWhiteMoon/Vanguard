using UnityEngine;
using UnityEngine.UI;

public class UnitUIController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider HPBar;
    public Slider MPBar;

    [Header("Settings")]
    public float heightOffset = 0.002f;

    public UnitObj unit;

    public void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        // 1. 유닛이나 스탯이 없으면 중단
        if (unit == null || unit.stat == null) return;

        // 2. 최대 체력이 0이면 0으로 처리 (0 나누기 방지)
        float maxHealth = unit.stat.MaxHealth;
        float currentHealth = unit.HP;

        float hpRatio = 0f;
        if (maxHealth > 0)
        {
            hpRatio = currentHealth / maxHealth;
        }

        // [디버깅용 로그] 이 줄을 추가해서 콘솔창(Console)을 확인해보세요!
        // 빨간 글씨나 흰 글씨로 값이 계속 변하는지 확인해야 합니다.
        // Debug.Log($"[UI] HP: {currentHealth} / {maxHealth} = 비율: {hpRatio}");

        // 3. HP 슬라이더 적용
        if (HPBar != null)
        {
            HPBar.value = hpRatio;
        }

        // 4. MP 슬라이더 적용 (현재는 MP 데이터가 없어서 1로 고정)
        // 나중에 UnitObj에 MP가 생기면 위 HP 로직처럼 바꾸세요.
        if (MPBar != null)
        {
            MPBar.value = 1f;
        }
    }
}