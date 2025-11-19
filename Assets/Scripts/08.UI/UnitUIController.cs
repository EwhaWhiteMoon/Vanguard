using UnityEngine;
using UnityEngine.UI;

public class UnitUIController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider HPBar;
    public Slider MPBar;

    [Header("Settings")]
    public float heightOffset = 0.002f;

    private UnitObj unit;
    private Camera cam;

    // [추가] 안전장치: 초기화가 되었는지 확인하는 깃발
    private bool isInitialized = false;

    public void Init(UnitObj owner)
    {
        unit = owner;
        cam = Camera.main;

        if (HPBar != null) { HPBar.minValue = 0f; HPBar.maxValue = 1f; }
        if (MPBar != null) { MPBar.minValue = 0f; MPBar.maxValue = 1f; }

        UpdateUI();

        // [추가] 초기화 완료 도장을 찍음
        isInitialized = true;
    }

    private void LateUpdate()
    {
        // [수정] 초기화도 안 됐는데 검사하지 마라 (안전장치)
        if (isInitialized == false) return;

        // 초기화는 됐었는데 유닛이 없다? -> 진짜로 죽어서 사라진 것임
        if (unit == null)
        {
            Destroy(this.gameObject);
            return;
        }

        // 1) 위치 업데이트
        transform.position = unit.transform.position + Vector3.up * heightOffset;

        // 2) 빌보드
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        // 3) UI 갱신
        UpdateUI();
    }

    private void UpdateUI()
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