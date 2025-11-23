using UnityEngine;

/// <summary>
/// 유닛 아이콘 및 설명을 제공하는 헬퍼입니다.
/// </summary>
public class UnitVisualHelper : MonoBehaviour
{
    public static UnitVisualHelper Instance { get; private set; }

    [SerializeField] private UnitVisualDatabase visualDatabase;

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

    public Sprite GetIcon(string unitId)
    {
        if (visualDatabase == null) return null;
        var entry = visualDatabase.GetEntry(unitId);
        return entry?.icon;
    }

    public string GetDescription(string unitId)
    {
        if (visualDatabase == null) return string.Empty;
        var entry = visualDatabase.GetEntry(unitId);
        return entry?.description ?? string.Empty;
    }
}

