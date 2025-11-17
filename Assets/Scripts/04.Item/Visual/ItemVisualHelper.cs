using UnityEngine;

/// <summary>
/// ItemVisualDatabase를 통해 아이템 아이콘/설명을 쉽게 조회하기 위한 싱글톤 헬퍼입니다.
/// 
/// - 씬 어딘가에 빈 GameObject에 붙여서 사용합니다.
/// - visualDatabase 필드에 ItemVisualDatabase.asset을 할당합니다.
/// - GetIcon / GetDescription으로 다른 코드에서 아이콘/설명을 조회할 수 있습니다.
/// </summary>
public class ItemVisualHelper : MonoBehaviour
{
    public static ItemVisualHelper Instance { get; private set; }

    [Header("아이템 아이콘/설명 데이터베이스")]
    [SerializeField] private ItemVisualDatabase visualDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// itemId로 아이콘 스프라이트를 조회합니다.
    /// </summary>
    public Sprite GetIcon(string itemId)
    {
        if (visualDatabase == null)
        {
            Debug.LogWarning("[ItemVisualHelper] visualDatabase가 설정되지 않았습니다.");
            return null;
        }

        var entry = visualDatabase.GetEntry(itemId);
        if (entry == null)
        {
            Debug.LogWarning($"[ItemVisualHelper] 아이템 ID '{itemId}'에 대한 비주얼 데이터를 찾지 못했습니다.");
            return null;
        }

        return entry.icon;
    }

    /// <summary>
    /// itemId로 설명 텍스트를 조회합니다.
    /// </summary>
    public string GetDescription(string itemId)
    {
        if (visualDatabase == null)
        {
            Debug.LogWarning("[ItemVisualHelper] visualDatabase가 설정되지 않았습니다.");
            return string.Empty;
        }

        var entry = visualDatabase.GetEntry(itemId);
        if (entry == null)
        {
            Debug.LogWarning($"[ItemVisualHelper] 아이템 ID '{itemId}'에 대한 비주얼 데이터를 찾지 못했습니다.");
            return string.Empty;
        }

        return entry.description;
    }
}

