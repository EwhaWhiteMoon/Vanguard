using UnityEngine;

/// <summary>
/// 아이템 아이콘 및 설명을 제공하는 헬퍼입니다.
/// </summary>
public class ItemVisualHelper : MonoBehaviour
{
    public static ItemVisualHelper Instance { get; private set; }

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

    public Sprite GetIcon(string itemId)
    {
        if (visualDatabase == null) return null;
        var entry = visualDatabase.GetEntry(itemId);
        return entry?.icon;
    }

    public string GetDescription(string itemId)
    {
        if (visualDatabase == null) return string.Empty;
        var entry = visualDatabase.GetEntry(itemId);
        return entry?.description ?? string.Empty;
    }
}
