using UnityEngine;
using CameraSystem;

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

    [Header("Camera")]
    [SerializeField] private CameraStatsData cameraPreset;
    [SerializeField] private bool setupCamera = true;

    private void Start()
    {
        InitializeInventory();
        SetupPlayer();
        SpawnCoins();

        // Setup camera after player is ready (use invoke to ensure player is fully initialized)
        Invoke(nameof(SetupCamera), 0.1f);
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

    private void SetupCamera()
    {
        if (!setupCamera)
        {
            Debug.Log("Camera setup disabled in InventoryScene settings");
            return;
        }

        // Find the main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Cannot setup camera follow system.");
            return;
        }

        Debug.Log($"Setting up camera on: {mainCamera.gameObject.name}");

        // Add FollowCamera component if it doesn't exist
        FollowCamera followCamera = mainCamera.GetComponent<FollowCamera>();
        if (followCamera == null)
        {
            followCamera = mainCamera.gameObject.AddComponent<FollowCamera>();
            Debug.Log("Added FollowCamera component to Main Camera");
        }
        else
        {
            Debug.Log("FollowCamera component already exists on Main Camera");
        }

        // Find the player to follow
        GameObject player = GameObject.Find("Hero");
        if (player == null)
        {
            Debug.LogError("Hero GameObject not found! Camera will not follow player. Make sure Hero exists in scene.");
            return;
        }

        Debug.Log($"Found Hero at position: {player.transform.position}");
        Debug.Log($"Camera is at position: {mainCamera.transform.position}");

        // Set the target
        followCamera.SetTarget(player.transform);
        Debug.Log($"Camera target set to Hero (Transform: {player.transform.GetInstanceID()})");

        // Apply camera preset if assigned
        if (cameraPreset != null)
        {
            followCamera.SetCameraStats(cameraPreset);
            Debug.Log($"Applied camera preset: {cameraPreset.name} (Deadzone: {cameraPreset.deadzoneWidth}x{cameraPreset.deadzoneHeight}, Speed: {cameraPreset.smoothSpeed})");
        }
        else
        {
            Debug.LogWarning("No camera preset assigned! Using default FollowCamera settings.");
        }

        // Force enable the component
        followCamera.enabled = true;
        Debug.Log($"Camera follow system setup complete! Component enabled: {followCamera.enabled}");
    }
}