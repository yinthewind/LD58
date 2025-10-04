using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask dragLayerMask;
    [SerializeField] private float dragZOffset = -1f;

    private InventorySlotDisplay parentSlot;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private int originalSlotIndex;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        // Start dragging
        if (mainCamera == null) return;

        parentSlot = GetComponentInParent<InventorySlotDisplay>();
        if (parentSlot == null || parentSlot.IsEmpty) return;

        isDragging = true;
        originalSlotIndex = parentSlot.SlotIndex;
        originalPosition = transform.position;

        Vector3 mousePos = GetMouseWorldPosition();
        offset = transform.position - mousePos;

        // Bring to front
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 100;
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging || mainCamera == null) return;

        Vector3 mousePos = GetMouseWorldPosition();
        transform.position = mousePos + offset;
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        // Reset sorting order
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 2;
        }

        // Try to find a slot to drop into
        InventorySlotDisplay targetSlot = FindSlotUnderMouse();

        if (targetSlot != null && targetSlot != parentSlot)
        {
            // Swap items
            int targetIndex = targetSlot.SlotIndex;
            InventorySystem.Instance.MoveItem(originalSlotIndex, targetIndex);
        }
        else
        {
            // Return to original position
            transform.position = originalPosition;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z) + dragZOffset;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    private InventorySlotDisplay FindSlotUnderMouse()
    {
        Vector3 mousePos = GetMouseWorldPosition();

        // Use raycast or overlap to find slot
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

        foreach (var hit in hits)
        {
            InventorySlotDisplay slot = hit.GetComponent<InventorySlotDisplay>();
            if (slot != null)
            {
                return slot;
            }
        }

        // Alternative: Find closest slot
        InventoryDisplay display = FindAnyObjectByType<InventoryDisplay>();
        if (display != null)
        {
            float closestDistance = float.MaxValue;
            InventorySlotDisplay closestSlot = null;

            for (int i = 0; i < InventorySystem.Instance.MaxSlots; i++)
            {
                InventorySlotDisplay slot = display.GetSlot(i);
                if (slot != null)
                {
                    float distance = Vector3.Distance(mousePos, slot.GetWorldPosition());
                    if (distance < closestDistance && distance < 0.5f) // Within reasonable range
                    {
                        closestDistance = distance;
                        closestSlot = slot;
                    }
                }
            }

            return closestSlot;
        }

        return null;
    }
}
