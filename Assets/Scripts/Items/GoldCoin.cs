using UnityEngine;

/// <summary>
/// Static configuration for Gold Coin collectible items.
/// This is the single source of truth for all gold coin properties.
/// </summary>
public static class GoldCoin
{
    public const string Name = "Gold Coin";
    public static readonly Color Color = new Color(1f, 0.84f, 0f); // Gold color
    public const bool IsStackable = false;  // Each coin gets its own slot
    public const int MaxStackSize = 1;
    public const string Description = "everybody loves me";

    private const string CIRCLE_SPRITE_PATH = "Assets/Tarodev 2D Controller/Sprites/Circle.png";
    private static Sprite cachedSprite;

    /// <summary>
    /// Gets the circle sprite, loading it if necessary.
    /// </summary>
    public static Sprite GetSprite()
    {
        if (cachedSprite != null)
            return cachedSprite;

#if UNITY_EDITOR
        cachedSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(CIRCLE_SPRITE_PATH);
#else
        cachedSprite = Resources.Load<Sprite>("Circle");
#endif

        if (cachedSprite == null)
        {
            Debug.LogWarning("GoldCoin: Circle sprite not found!");
        }

        return cachedSprite;
    }

    /// <summary>
    /// Configures a Collectible component with gold coin properties.
    /// </summary>
    public static void ConfigureCollectible(Collectible collectible)
    {
        Sprite sprite = GetSprite();

        collectible.itemName = Name;
        collectible.icon = sprite;
        collectible.iconColor = Color;
        collectible.isStackable = IsStackable;
        collectible.maxStackSize = MaxStackSize;
        collectible.description = Description;
    }

    /// <summary>
    /// Creates a complete gold coin GameObject at the specified position.
    /// </summary>
    public static GameObject CreateCoin(Vector3 position, float size = 0.5f)
    {
        Sprite sprite = GetSprite();

        // Create GameObject
        GameObject coin = new GameObject("GoldCoin");
        coin.transform.position = position;
        coin.transform.localScale = Vector3.one * size;

        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = coin.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = Color;
        spriteRenderer.sortingOrder = 5;

        // Add CircleCollider2D as trigger
        CircleCollider2D collider = coin.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        // Add Collectible component
        Collectible collectible = coin.AddComponent<Collectible>();
        ConfigureCollectible(collectible);

        return coin;
    }
}
