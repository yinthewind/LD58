using UnityEngine;

public class InventoryScene : MonoBehaviour
{
    [Header("Inventory System")]
    [SerializeField] private GameObject inventorySystemPrefab;

    [Header("Inventory Display")]
    [SerializeField] private GameObject inventoryDisplayPrefab;
    [SerializeField] private bool useScreenSpace = true;
    [SerializeField] private Vector2 screenOffset = new(0.15f, 0.85f); // Left side, near top

    [Header("Collectibles")]
    [SerializeField] private int numberOfCoins = 10;
    [SerializeField] private float coinSpawnRadius = 8f;
    [SerializeField] private float coinSize = 0.5f;

    [Header("Player")]
    [SerializeField] private Sprite playerSprite;

    private void Start()
    {
        InitializeInventory();
        SetupPlayer();
        SpawnCoins();
    }

    private void InitializeInventory()
    {
        // Create InventorySystem if it doesn't exist
        if (InventorySystem.Instance == null && inventorySystemPrefab != null)
        {
            Instantiate(inventorySystemPrefab);
        }

        // Create and configure the inventory display
        if (inventoryDisplayPrefab != null)
        {
            GameObject inventoryDisplay = Instantiate(inventoryDisplayPrefab);

            // Configure for left-side screen space positioning
            InventoryDisplay display = inventoryDisplay.GetComponent<InventoryDisplay>();
            if (display != null)
            {
                // The InventoryDisplay script will handle positioning based on these settings
                // which should be configured on the prefab or via script here
            }
        }
    }

    private void SpawnCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            GoldCoin.CreateCoin(GetRandomSpawnPosition(), coinSize);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * coinSpawnRadius;
        return new Vector3(randomCircle.x, randomCircle.y, 0);
    }

    private void SetupPlayer()
    {
        // Find the Hero/Player GameObject
        GameObject player = GameObject.Find("Hero");

        if (player == null)
        {
            Debug.LogWarning("Hero GameObject not found in scene!");
            return;
        }

        // Ensure it has the "Player" tag
        if (!player.CompareTag("Player"))
        {
            Debug.Log("Setting Hero tag to 'Player'");
            player.tag = "Player";
        }

        // Ensure it has ItemCollector component
        ItemCollector collector = player.GetComponent<ItemCollector>();
        if (collector == null)
        {
            Debug.Log("Adding ItemCollector component to Hero");
            player.AddComponent<ItemCollector>();
        }

        // Set player sprite to memeface if assigned
        if (playerSprite != null)
        {
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = playerSprite;
                Debug.Log("Player sprite updated to memeface!");
            }
            else
            {
                Debug.LogWarning("Hero doesn't have SpriteRenderer component!");
            }
        }

        Debug.Log($"Player setup complete! Tag: {player.tag}, Has ItemCollector: {player.GetComponent<ItemCollector>() != null}");
    }
}