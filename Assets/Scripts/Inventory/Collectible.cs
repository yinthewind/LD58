using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Collectible : MonoBehaviour
{
    [Header("Item Properties")]
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

    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set sprite and color if assigned
        if (icon != null)
        {
            spriteRenderer.sprite = icon;
            spriteRenderer.color = iconColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

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
