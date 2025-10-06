using UnityEngine;

public class InventorySlotDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer itemRenderer;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Settings")]
    [SerializeField] private Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color filledColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private int slotIndex;
    private InventorySlot inventorySlot;

    public int SlotIndex => slotIndex;
    public bool IsEmpty => inventorySlot == null || inventorySlot.IsEmpty;

    private void Awake()
    {
        if (itemRenderer == null)
        {
            // Create item renderer as child
            GameObject itemObj = new GameObject("ItemSprite");
            itemObj.transform.SetParent(transform);
            itemObj.transform.localPosition = Vector3.zero;
            itemRenderer = itemObj.AddComponent<SpriteRenderer>();
            itemRenderer.sortingOrder = 2;
        }

        if (backgroundRenderer == null)
        {
            backgroundRenderer = GetComponent<SpriteRenderer>();
            if (backgroundRenderer == null)
            {
                backgroundRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            backgroundRenderer.sortingOrder = 1;
        }

        // Add BoxCollider2D for click detection
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            // Collider size will match the slot's localScale (set by InventoryDisplay)
            collider.size = Vector2.one;
        }

        UpdateVisuals();
    }

    public void Initialize(int index)
    {
        slotIndex = index;
        UpdateVisuals();
    }

    public void SetItem(InventorySlot slot)
    {
        inventorySlot = slot;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (inventorySlot != null && !inventorySlot.IsEmpty)
        {
            itemRenderer.sprite = inventorySlot.icon;
            itemRenderer.color = inventorySlot.iconColor;
            itemRenderer.gameObject.SetActive(true);
            if (backgroundRenderer != null)
                backgroundRenderer.color = filledColor;
        }
        else
        {
            itemRenderer.sprite = null;
            itemRenderer.gameObject.SetActive(false);
            if (backgroundRenderer != null)
                backgroundRenderer.color = emptyColor;
        }
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Handles mouse click on the slot to unload/throw item.
    /// </summary>
    private void OnMouseUpAsButton()
    {
        // Only unload if slot is not empty
        if (IsEmpty)
        {
            Debug.Log($"Slot {slotIndex} is empty, nothing to unload");
            return;
        }

        Debug.Log($"Clicked on slot {slotIndex} to unload item: {inventorySlot.itemName}");

        // Find InventoryDisplay and trigger unload
        InventoryDisplay display = GetComponentInParent<InventoryDisplay>();
        if (display == null)
        {
            display = FindObjectOfType<InventoryDisplay>();
        }

        if (display != null)
        {
            display.UnloadSlot(slotIndex);
        }
        else
        {
            Debug.LogError("InventoryDisplay not found! Cannot unload item.");
        }
    }
}
