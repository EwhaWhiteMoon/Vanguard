using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Definition")]
public class ItemDefinition : ScriptableObject
{
    public string DisplayName;
    public Sprite Icon;
    [TextArea] public string Description;

    [Header("Item Effects")]
    public List<ItemStat> ItemStats = new();  // 여러 효과 보유
}