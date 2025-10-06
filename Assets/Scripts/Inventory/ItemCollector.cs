using UnityEngine;
using System.Collections;

public class ItemCollector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryDisplay inventoryDisplay;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        // Try to find InventoryDisplay if not assigned
        if (inventoryDisplay == null)
        {
            inventoryDisplay = FindAnyObjectByType<InventoryDisplay>();
        }
    }

    public void CollectItem(Collectible collectible)
    {
        if (collectible == null)
        {
            Debug.LogWarning($"CollectItem called with null collectible!");
            return;
        }

        Debug.Log($"Attempting to collect: {collectible.itemName}");

        // Try to add item to inventory
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem.Instance is null!");
            return;
        }

        // Add to inventory WITHOUT triggering visual update yet
        if (InventorySystem.Instance.TryAddItem(collectible, false))
        {
            Debug.Log($"Successfully added {collectible.itemName} to inventory!");
            collectible.MarkAsCollected();

            // Find which slot the item was added to
            int slotIndex = FindItemSlotIndex(collectible.itemName);

            if (slotIndex >= 0 && inventoryDisplay != null)
            {
                // Start collection animation
                Vector3 targetPosition = inventoryDisplay.GetSlotPosition(slotIndex);
                Debug.Log($"Starting animation to slot {slotIndex} at position {targetPosition}");
                StartCoroutine(AnimateCollection(collectible.gameObject, targetPosition, collectible.itemName, slotIndex));
            }
            else
            {
                Debug.LogWarning($"No animation: slotIndex={slotIndex}, inventoryDisplay={(inventoryDisplay != null ? "exists" : "null")}");
                // No animation, just destroy and show immediately
                InventorySystem.Instance.NotifyItemAdded(collectible.itemName, slotIndex);
                Destroy(collectible.gameObject);
            }

            // Play collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
    }

    private int FindItemSlotIndex(string itemName)
    {
        if (InventorySystem.Instance == null)
            return -1;

        var slots = InventorySystem.Instance.Slots;
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].itemName == itemName)
                return i;
        }
        return -1;
    }

    private IEnumerator AnimateCollection(GameObject item, Vector3 targetPosition, string itemName, int slotIndex)
    {
        CollectionTween tween = item.AddComponent<CollectionTween>();

        // Pass a lambda that queries fresh slot position every frame
        tween.Initialize(() => inventoryDisplay.GetSlotPosition(slotIndex));

        // Wait for animation to complete
        while (tween != null && !tween.IsComplete)
        {
            yield return null;
        }

        // NOW trigger the visual update in inventory (item has reached the slot)
        InventorySystem.Instance.NotifyItemAdded(itemName, slotIndex);

        // Destroy the item
        Destroy(item);
    }
}
