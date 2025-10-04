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
}
