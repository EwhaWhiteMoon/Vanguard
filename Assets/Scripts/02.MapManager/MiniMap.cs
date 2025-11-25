using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class MiniMap : MonoBehaviour
{
    public void OnGameStateChange(GameState state)
    {
        gameObject.SetActive(state == GameState.AfterCombat);
    }
    
    [Header("Visual Settings")]
    public Sprite playerMarkerSprite;

    public RectTransform container;  // Assign a UI Panel under Canvas for minimap
    public float tileSpacing = 1.0f;

    private Image[,] tiles;
    private Vector2Int? previousPlayerPos = null;
    public MapManager map;

    private void Awake()
    {
        if(map == null)
        {
            map = FindFirstObjectByType<MapManager>();
        }
        
        GameManager.Instance.OnGameStateChange += OnGameStateChange;
        gameObject.SetActive(false); // 구독하고, 일단 필요해질때까지 꺼두기.
        OnGameStateChange(GameManager.Instance.GameState); //지금 State에 맞게 한번 호출해줘야함.
    }

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
                img.rectTransform.anchoredPosition = new Vector2(x * tileSize * tileSpacing, -(height - 1 - y) * tileSize * tileSpacing);

                tiles[x, y] = img;
            }
        }

        HighlightPlayerRoom();
    }

    public void HighlightPlayerRoom()
    {
        if (map.Map == null || tiles == null) return;

        Vector2Int playerPos = map.playerPos;

        if (previousPlayerPos.HasValue)
        {
            var prev = tiles[previousPlayerPos.Value.x, previousPlayerPos.Value.y];
            if (prev != null)
                prev.sprite = GetSpriteForRoom(map.Map[previousPlayerPos.Value.x, previousPlayerPos.Value.y]);
        }

        if (playerPos.x >= 0 && playerPos.x < map.WIDTH && playerPos.y >= 0 && playerPos.y < map.HEIGHT)
        {
            var playerTile = tiles[playerPos.x, playerPos.y];
            if (playerTile != null)
                playerTile.sprite = playerMarkerSprite;

            previousPlayerPos = playerPos;
        }
    }

    private Sprite GetSpriteForRoom(Room room)
    {
        if (room.Data == null)
        {
            return null;
        }
        return room.Data.minimapSprite;
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
