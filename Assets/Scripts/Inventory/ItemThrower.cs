using UnityEngine;
using System.Collections;

/// <summary>
/// Handles throwing items from inventory to nearby locations.
/// Attached to the player character.
/// </summary>
public class ItemThrower : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float throwDistance = 2f;
    [SerializeField] private float throwArcHeight = 1.5f;
    [SerializeField] private float throwDuration = 0.5f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer = ~0; // All layers by default
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private float groundOffset = 0.1f; // Height above ground
    [SerializeField] private bool debugRaycast = false; // Show raycast in Scene view

    /// <summary>
    /// Throws an item from inventory slot to a nearby location.
    /// </summary>
    public void ThrowItemFromSlot(InventorySlot slot, int slotIndex)
    {
        if (slot == null || slot.IsEmpty)
        {
            Debug.LogWarning("Cannot throw empty slot!");
            return;
        }

        // 1. Calculate valid throw target position with ground detection
        Vector3 targetPosition = GetValidThrowTarget();

        // 2. Spawn item from slot data at player position
        GameObject item = SpawnItemFromSlot(slot, transform.position);

        // 3. Remove item from inventory
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.RemoveItem(slotIndex);
        }

        // 4. Animate throw with arc trajectory
        StartCoroutine(AnimateThrow(item, targetPosition));

        Debug.Log($"Threw {slot.itemName} from slot {slotIndex} to position {targetPosition}");
    }

    /// <summary>
    /// Gets a valid throw target position with ground detection.
    /// Uses raycast to ensure item lands on ground, not underground or in air.
    /// </summary>
    private Vector3 GetValidThrowTarget()
    {
        // Random angle in full circle (horizontal only)
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 horizontalDirection = new Vector3(Mathf.Cos(angle), 0, 0).normalized;

        // Calculate horizontal target position
        Vector3 targetPosition = transform.position + horizontalDirection * throwDistance;

        // Raycast downward from high above target to find ground
        Vector3 raycastStart = new Vector3(targetPosition.x, targetPosition.y + 10f, targetPosition.z);
        RaycastHit2D hit = Physics2D.Raycast(raycastStart, Vector2.down, raycastDistance, groundLayer);

        if (hit.collider != null)
        {
            // Ground found - place item slightly above ground
            targetPosition.y = hit.point.y + groundOffset;
            Debug.Log($"Ground detected at Y={hit.point.y}, item will land at {targetPosition}");
        }
        else
        {
            // No ground found - fallback to player's Y position
            targetPosition.y = transform.position.y;
            Debug.LogWarning($"No ground detected at {raycastStart}, using player Y={transform.position.y}");
        }

        // Optional debug visualization
        if (debugRaycast)
        {
            Debug.DrawRay(raycastStart, Vector2.down * raycastDistance,
                          hit.collider != null ? Color.green : Color.red, 2f);
        }

        return targetPosition;
    }

    /// <summary>
    /// Spawns a collectible GameObject from inventory slot data.
    /// </summary>
    private GameObject SpawnItemFromSlot(InventorySlot slot, Vector3 position)
    {
        GameObject obj = new GameObject(slot.itemName);
        obj.transform.position = position;

        // Add SpriteRenderer
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = slot.icon;
        sr.color = slot.iconColor;
        sr.sortingOrder = 5;

        // Add CircleCollider2D as trigger
        CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
        col.isTrigger = true; // Trigger for collection
        col.radius = 0.5f;

        // Add Collectible component
        Collectible collectible = obj.AddComponent<Collectible>();
        collectible.itemType = slot.itemType;
        collectible.itemName = slot.itemName;
        collectible.icon = slot.icon;
        collectible.iconColor = slot.iconColor;
        collectible.isStackable = slot.isStackable;
        collectible.maxStackSize = slot.maxStackSize;
        collectible.description = "";

        // Apply visuals
        collectible.ApplyVisuals();

        // Set cooldown to prevent immediate re-collection
        collectible.SetCollectionCooldown(0.3f);

        Debug.Log($"Spawned thrown item: {slot.itemName} at {position}");

        return obj;
    }

    /// <summary>
    /// Animates the throw using CollectionTween for smooth arc trajectory.
    /// </summary>
    private IEnumerator AnimateThrow(GameObject item, Vector3 targetPosition)
    {
        // Add CollectionTween component to handle the arc animation
        CollectionTween tween = item.AddComponent<CollectionTween>();
        tween.Initialize(() => targetPosition);

        // Wait for animation to complete
        while (tween != null && !tween.IsComplete)
        {
            yield return null;
        }

        // Clean up tween component
        if (tween != null)
        {
            Destroy(tween);
        }

        // Reset scale to normal size after throw
        item.transform.localScale = Vector3.one;

        Debug.Log($"Throw animation complete for {item.name} at {item.transform.position}");
    }
}
