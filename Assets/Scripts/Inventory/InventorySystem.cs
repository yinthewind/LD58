using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 12;

    private List<InventorySlot> slots = new List<InventorySlot>();

    public event Action<string, int> OnItemAdded; // itemName, slotIndex
    public event Action<int, int> OnItemsMoved; // fromIndex, toIndex
    public event Action<int> OnItemRemoved; // slotIndex

    public int MaxSlots => maxSlots;
    public List<InventorySlot> Slots => slots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        slots.Clear();
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public int TryAddItem(Collectible collectible)
    {
        return TryAddItem(collectible, true);
    }

    public int TryAddItem(Collectible collectible, bool notifyImmediately)
    {
        if (collectible == null)
            return -1;

        // Try to stack if stackable
        if (collectible.isStackable)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].itemName == collectible.itemName && slots[i].quantity < collectible.maxStackSize)
                {
                    slots[i].quantity++;
                    if (notifyImmediately)
                        OnItemAdded?.Invoke(collectible.itemName, i);
                    return i; // Return the slot index where item was stacked
                }
            }
        }

        // Find empty slot
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                // Copy properties from collectible to slot
                slots[i].itemType = collectible.itemType;
                slots[i].itemName = collectible.itemName;
                slots[i].icon = collectible.icon;
                slots[i].iconColor = collectible.iconColor;
                slots[i].isStackable = collectible.isStackable;
                slots[i].maxStackSize = collectible.maxStackSize;
                slots[i].quantity = 1;

                if (notifyImmediately)
                    OnItemAdded?.Invoke(collectible.itemName, i);
                return i; // Return the slot index where item was added
            }
        }

        Debug.Log("Inventory is full!");
        return -1; // Return -1 to indicate failure (inventory full)
    }

    public void NotifyItemAdded(string itemName, int slotIndex)
    {
        OnItemAdded?.Invoke(itemName, slotIndex);
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= slots.Count || toIndex < 0 || toIndex >= slots.Count)
            return;

        var temp = slots[fromIndex];
        slots[fromIndex] = slots[toIndex];
        slots[toIndex] = temp;

        OnItemsMoved?.Invoke(fromIndex, toIndex);
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return;

        slots[slotIndex].Clear();

        // Trigger removal event
        OnItemRemoved?.Invoke(slotIndex);
    }

    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return null;
        return slots[index];
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemType itemType = ItemType.Empty;
    public string itemName;
    public Sprite icon;
    public Color iconColor;
    public bool isStackable;
    public int maxStackSize;
    public int quantity;

    public bool IsEmpty => itemType == ItemType.Empty;

    public void Clear()
    {
        itemType = ItemType.Empty;
        itemName = null;
        icon = null;
        iconColor = Color.white;
        isStackable = false;
        maxStackSize = 1;
        quantity = 0;
    }
}
