using UnityEngine;

/// <summary>
/// Initializes a GameObject as a collectible item at runtime.
/// Automatically adds and configures required components (Collectible, SpriteRenderer, Collider2D).
/// </summary>
public class CollectibleInitializer : MonoBehaviour
{
    [Header("Item Configuration")]
    [SerializeField] private string itemName = "Gold Coin";
    [SerializeField] private Sprite icon;
    [SerializeField] private Color iconColor = new Color(1f, 0.84f, 0f);
    [SerializeField] private bool isStackable = false;
    [SerializeField] private int maxStackSize = 1;
    [TextArea(2, 4)]
    [SerializeField] private string description = "A collectible item";

    [Header("Visual Settings")]
    [SerializeField] private bool useGoldCoinDefaults = true;
    [SerializeField] private int sortingOrder = 5;

    [Header("Collider Settings")]
    [SerializeField] private bool useCircleCollider = true;
    [SerializeField] private float colliderRadius = 0.5f;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the GameObject as a collectible. Can be called manually if needed.
    /// </summary>
    public void Initialize()
    {
        // Apply gold coin defaults if enabled
        if (useGoldCoinDefaults)
        {
            itemName = GoldCoin.Name;
            icon = GoldCoin.GetSprite();
            iconColor = GoldCoin.Color;
            isStackable = GoldCoin.IsStackable;
            maxStackSize = GoldCoin.MaxStackSize;
            description = GoldCoin.Description;
        }

        // Add or get SpriteRenderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (icon != null)
        {
            spriteRenderer.sprite = icon;
        }
        spriteRenderer.color = iconColor;
        spriteRenderer.sortingOrder = sortingOrder;

        // Add or get Collider2D
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            if (useCircleCollider)
            {
                CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
                circleCollider.radius = colliderRadius;
                collider = circleCollider;
            }
            else
            {
                BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                collider = boxCollider;
            }
        }
        collider.isTrigger = true;

        // Add or get Collectible component
        Collectible collectible = GetComponent<Collectible>();
        if (collectible == null)
        {
            collectible = gameObject.AddComponent<Collectible>();
        }

        // Configure collectible properties
        collectible.itemName = itemName;
        collectible.icon = icon;
        collectible.iconColor = iconColor;
        collectible.isStackable = isStackable;
        collectible.maxStackSize = maxStackSize;
        collectible.description = description;

        Debug.Log($"CollectibleInitializer: Initialized '{itemName}' collectible on {gameObject.name}");
    }
}
