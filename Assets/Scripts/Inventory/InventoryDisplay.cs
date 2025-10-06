using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InventoryDisplay : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int gridColumns = 4;
    [SerializeField] private float slotSize = 0.8f;
    [SerializeField] private float slotSpacing = 0.1f;

    [Header("Position")]
    [SerializeField] private bool followPlayer = false;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offsetFromPlayer = new Vector3(2f, 1f, 0f);

    [Header("Screen-Space Mode")]
    [SerializeField] private bool useScreenSpace = true;
    [SerializeField] private Vector2 screenOffset = new Vector2(0.8f, 0.8f); // Viewport coordinates (0-1)

    private List<InventorySlotDisplay> inventorySlots = new List<InventorySlotDisplay>();
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Wait for InventorySystem to initialize
        if (InventorySystem.Instance != null)
        {
            InitializeSlots();
            SubscribeToInventory();
        }
        else
        {
            Debug.LogError("InventorySystem not found! Make sure it exists in the scene.");
        }
    }

    private void InitializeSlots()
    {
        int maxSlots = InventorySystem.Instance.MaxSlots;
        int rows = Mathf.CeilToInt((float)maxSlots / gridColumns);

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj;

            if (slotPrefab != null)
            {
                slotObj = Instantiate(slotPrefab, transform);
            }
            else
            {
                // Create default slot
                slotObj = new GameObject($"Slot_{i}");
                slotObj.transform.SetParent(transform);
                slotObj.AddComponent<InventorySlotDisplay>();

                // Add a simple square sprite renderer for background
                SpriteRenderer sr = slotObj.GetOrAddComponent<SpriteRenderer>();
                sr.sprite = CreateSquareSprite();
                sr.sortingOrder = 1;
            }

            // Calculate grid position
            int row = i / gridColumns;
            int col = i % gridColumns;

            float x = col * (slotSize + slotSpacing);
            float y = -row * (slotSize + slotSpacing);

            slotObj.transform.localPosition = new Vector3(x, y, 0);
            slotObj.transform.localScale = Vector3.one * slotSize;

            InventorySlotDisplay slot = slotObj.GetComponent<InventorySlotDisplay>();
            slot.Initialize(i);
            inventorySlots.Add(slot);
        }

        // Center the grid
        float totalWidth = (gridColumns - 1) * (slotSize + slotSpacing);
        transform.position -= new Vector3(totalWidth / 2f, 0, 0);
    }

    private void SubscribeToInventory()
    {
        InventorySystem.Instance.OnItemAdded += OnItemAdded;
        InventorySystem.Instance.OnItemsMoved += OnItemsMoved;
        InventorySystem.Instance.OnItemRemoved += OnItemRemoved;
    }

    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnItemAdded -= OnItemAdded;
            InventorySystem.Instance.OnItemsMoved -= OnItemsMoved;
            InventorySystem.Instance.OnItemRemoved -= OnItemRemoved;
        }
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (useScreenSpace && mainCamera != null)
        {
            // Position in screen space (viewport coordinates)
            Vector3 viewportPos = new Vector3(screenOffset.x, screenOffset.y, 10f);
            Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);
            worldPos.z = 0;
            transform.position = worldPos;
        }
        else if (followPlayer && playerTransform != null)
        {
            transform.position = playerTransform.position + offsetFromPlayer;
        }
    }

    private void OnItemAdded(string itemName, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Count)
        {
            var inventorySlot = InventorySystem.Instance.GetSlot(slotIndex);
            inventorySlots[slotIndex].SetItem(inventorySlot);
        }
    }

    private void OnItemsMoved(int fromIndex, int toIndex)
    {
        if (fromIndex >= 0 && fromIndex < inventorySlots.Count)
        {
            var inventorySlot = InventorySystem.Instance.GetSlot(fromIndex);
            inventorySlots[fromIndex].SetItem(inventorySlot);
        }

        if (toIndex >= 0 && toIndex < inventorySlots.Count)
        {
            var inventorySlot = InventorySystem.Instance.GetSlot(toIndex);
            inventorySlots[toIndex].SetItem(inventorySlot);
        }
    }

    public InventorySlotDisplay GetSlot(int index)
    {
        if (index >= 0 && index < inventorySlots.Count)
            return inventorySlots[index];
        return null;
    }

    public Vector3 GetSlotPosition(int index)
    {
        var slot = GetSlot(index);
        return slot != null ? slot.GetWorldPosition() : transform.position;
    }

    private Sprite CreateSquareSprite()
    {
        // Create a simple white square texture
        Texture2D tex = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
    }

    /// <summary>
    /// Called when an item is removed from inventory. Updates slot visual.
    /// </summary>
    private void OnItemRemoved(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Count)
        {
            // Set slot to empty
            inventorySlots[slotIndex].SetItem(new InventorySlot());
            Debug.Log($"Slot {slotIndex} visual cleared after item removal");
        }
    }

    /// <summary>
    /// Unloads an item from the specified slot and throws it from the player.
    /// </summary>
    public void UnloadSlot(int slotIndex)
    {
        // Get slot data
        InventorySlot slot = InventorySystem.Instance.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty)
        {
            Debug.LogWarning($"Cannot unload slot {slotIndex}: slot is empty");
            return;
        }

        // Find player
        GameObject player = FindPlayer();
        if (player == null)
        {
            Debug.LogError("Cannot unload: Player not found!");
            return;
        }

        // Get or add ItemThrower component
        ItemThrower thrower = player.GetComponent<ItemThrower>();
        if (thrower == null)
        {
            thrower = player.AddComponent<ItemThrower>();
            Debug.Log("Added ItemThrower component to player");
        }

        // Throw the item
        thrower.ThrowItemFromSlot(slot, slotIndex);

        Debug.Log($"Unloaded and threw {slot.itemName} from slot {slotIndex}");
    }

    /// <summary>
    /// Finds the player GameObject in the scene.
    /// </summary>
    private GameObject FindPlayer()
    {
        // Try to find Hero first (as used in InventoryScene.cs)
        GameObject player = GameObject.Find("Hero");

        // Fallback to Player tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        return player;
    }
}
