using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Collectible : MonoBehaviour
{
    [Header("Item Properties")]
    public ItemType itemType = ItemType.Empty;
    public string itemName = "Item";
    public Sprite icon;
    public Color iconColor = Color.white;
    public bool isStackable = false;
    public int maxStackSize = 1;
    [TextArea(2, 4)]
    public string description = "";

    [Header("Settings")]
    [SerializeField] private float collectRadius = 1f;
    [SerializeField] private bool autoCollectOnTrigger = true;

    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;

    // Optional: Collection cooldown (can be used for delayed item spawning or preventing spam collection)
    private float collectionCooldown = 0f;

    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Applies the visual settings (sprite and color) to the SpriteRenderer.
    /// Call this after setting icon and iconColor properties.
    /// </summary>
    public void ApplyVisuals()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (icon != null)
        {
            spriteRenderer.sprite = icon;
        }
        spriteRenderer.color = iconColor;
    }

    private void Update()
    {
        if (collectionCooldown > 0)
        {
            collectionCooldown -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        if (collectionCooldown > 0) return; // Prevent collection during cooldown

        Debug.Log($"Collectible triggered by: {other.gameObject.name}, Tag: {other.tag}");

        if (autoCollectOnTrigger && other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Looking for ItemCollector...");
            ItemCollector collector = other.GetComponent<ItemCollector>();
            if (collector != null)
            {
                Debug.Log("ItemCollector found! Collecting item...");
                collector.CollectItem(this);
            }
            else
            {
                Debug.LogWarning("Player doesn't have ItemCollector component!");
            }
        }
        else if (!other.CompareTag("Player"))
        {
            Debug.LogWarning($"Object {other.gameObject.name} doesn't have 'Player' tag!");
        }
    }

    /// <summary>
    /// Sets a cooldown period during which the item cannot be collected.
    /// Used to prevent immediate re-collection of dropped items.
    /// </summary>
    public void SetCollectionCooldown(float seconds)
    {
        collectionCooldown = seconds;
    }

    public void MarkAsCollected()
    {
        isCollected = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}
