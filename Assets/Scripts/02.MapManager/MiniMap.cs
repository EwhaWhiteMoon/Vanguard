using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite emptySprite;
    public Sprite combatSprite;
    public Sprite eventSprite;
    public Sprite mysterySprite;
    public Sprite bossSprite;
    public Sprite playerMarkerSprite;

    [Header("Visual Settings")]
    public RectTransform container;  // Assign a UI Panel under Canvas for minimap
    public float tileSpacing = 1.0f;

    private Image[,] tiles;
    private MapManager map => MapManager.Instance;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            RefreshMiniMap();
        }
    }

    public void DrawMiniMap()
    {
        if (map.Map == null)
        {
            Debug.LogWarning("No map generated yet!");
            return;
        }

        int width = map.WIDTH;
        int height = map.HEIGHT;

        tiles = new Image[width, height];

        // Calculate tile size to fit container
        float tileSize = Mathf.Min(
        container.rect.width / (width * tileSpacing),
        container.rect.height / (height * tileSpacing)
    );


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Room room = map.Map[x, y];
                if (room.Type == RoomType.Void)
                    continue;

                // Create UI Image
                GameObject tileGO = new GameObject($"Tile_{x}_{y}", typeof(Image));
                tileGO.transform.SetParent(container, false);

                Image img = tileGO.GetComponent<Image>();
                img.sprite = GetSpriteForRoom(room);
                img.rectTransform.sizeDelta = new Vector2(tileSize, tileSize);

                // Position in top-left anchored container
                img.rectTransform.anchorMin = img.rectTransform.anchorMax = new Vector2(0, 1);
                img.rectTransform.pivot = new Vector2(0, 1);
                img.rectTransform.anchoredPosition = new Vector2(x * tileSize * tileSpacing, -y * tileSize * tileSpacing);

                tiles[x, y] = img;
            }
        }

        HighlightPlayerRoom();
    }

    private Sprite GetSpriteForRoom(Room room)
    {
        switch (room.Type)
        {
            case RoomType.Empty: return emptySprite;
            case RoomType.CombatRoom: return combatSprite;
            case RoomType.EventRoom: return eventSprite;
            case RoomType.MysteryRoom: return mysterySprite;
            case RoomType.BossRoom: return bossSprite;
            default: return null;
        }
    }

    public void HighlightPlayerRoom()
    {
        if (map.Map == null || tiles == null) return;

        Vector2Int playerPos = map.playerPos;

        // Reset all tile colors
        foreach (var t in tiles)
        {
            if (t != null) t.color = Color.white;
        }

        if (playerPos.x >= 0 && playerPos.x < map.WIDTH && playerPos.y >= 0 && playerPos.y < map.HEIGHT)
        {
            var playerTile = tiles[playerPos.x, playerPos.y];
            if (playerTile != null)
            {
                playerTile.sprite = playerMarkerSprite; // swap sprite for player marker
            }
        }
    }

    public void RefreshMiniMap()
    {
        if (tiles != null)
        {
            foreach (var tile in tiles)
            {
                if (tile != null) Destroy(tile.gameObject);
            }
        }

        DrawMiniMap();
    }
}
