using UnityEngine;

/// <summary>
/// Generic spawner for collectible items.
/// Supports spawning different collectible types using factory methods.
/// </summary>
public class CollectibleSpawner : MonoBehaviour
{
    public enum CollectibleType
    {
        GoldCoin
        // Add more types here: SilverCoin, RedGem, etc.
    }

    [Header("Collectible Type")]
    [SerializeField] private CollectibleType collectibleType = CollectibleType.GoldCoin;

    [Header("Spawn Settings")]
    [SerializeField] private int numberOfItems = 5;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private Vector3 spawnCenter = Vector3.zero;

    [Header("Item Appearance")]
    [SerializeField] private float itemSize = 0.5f;

    private void Start()
    {
        SpawnItems();
    }

    [ContextMenu("Spawn Items")]
    public void SpawnItems()
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            CreateCollectible(GetRandomSpawnPosition());
        }
    }

    private GameObject CreateCollectible(Vector3 position)
    {
        GameObject collectible = null;

        switch (collectibleType)
        {
            case CollectibleType.GoldCoin:
                collectible = GoldCoin.CreateCoin(position, itemSize);
                break;

            // Add more cases here for other collectible types
            // case CollectibleType.RedGem:
            //     collectible = RedGem.CreateGem(position, itemSize);
            //     break;

            default:
                Debug.LogError($"Unknown collectible type: {collectibleType}");
                break;
        }

        return collectible;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return spawnCenter + new Vector3(randomCircle.x, randomCircle.y, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnCenter, spawnRadius);
    }
}
