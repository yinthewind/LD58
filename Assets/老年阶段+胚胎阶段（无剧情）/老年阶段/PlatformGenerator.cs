using UnityEngine;
using System.Collections.Generic;

// 定义滚动轴枚举
public enum ScrollAxis
{
    Horizontal,
    Vertical,
    Both
}

public class PlatformGenerator : MonoBehaviour
{
    [System.Serializable]
    public class PlatformType
    {
        public GameObject prefab;
        public float weight = 1f;
        public float minHeight = -2f;
        public float maxHeight = 2f;
    }

    [Header("生成设置")]
    public ScrollAxis generationAxis = ScrollAxis.Horizontal;
    public float spawnDistance = 10f;
    public float despawnDistance = 15f;
    public int initialPlatforms = 5;

    [Header("平台类型")]
    public List<PlatformType> platformTypes = new List<PlatformType>();

    [Header("生成参数")]
    public float minPlatformSpacing = 2f;
    public float maxPlatformSpacing = 5f;
    public float minHeightChange = -1f;
    public float maxHeightChange = 1f;

    [Header("调试")]
    public bool showGizmos = true;
    public Color spawnZoneColor = new Color(0, 1, 0, 0.3f);
    public Color despawnZoneColor = new Color(1, 0, 0, 0.3f);

    private Camera mainCamera;
    private List<GameObject> activePlatforms = new List<GameObject>();
    private float lastSpawnPosition;
    private SimpleObjectPool platformPool; // 使用简化的对象池

    void Start()
    {
        mainCamera = Camera.main;
        platformPool = new SimpleObjectPool();

        // 初始化对象池
        foreach (var platformType in platformTypes)
        {
            if (platformType.prefab != null)
            {
                platformPool.AddPrefab(platformType.prefab);
            }
        }

        // 生成初始平台
        InitializePlatforms();
    }

    void Update()
    {
        UpdatePlatformGeneration();
        CleanupPlatforms();
    }

    void InitializePlatforms()
    {
        Vector3 startPosition = GetStartPosition();

        for (int i = 0; i < initialPlatforms; i++)
        {
            Vector3 spawnPos = CalculateNextPlatformPosition(startPosition, i);
            SpawnPlatform(spawnPos);
        }

        lastSpawnPosition = GetAxisPosition(GetFurthestPlatformPosition());
    }

    void UpdatePlatformGeneration()
    {
        float cameraPosition = GetAxisPosition(mainCamera.transform.position);
        float furthestPlatformPosition = GetAxisPosition(GetFurthestPlatformPosition());

        if (furthestPlatformPosition - cameraPosition < spawnDistance)
        {
            Vector3 nextPosition = CalculateNextPlatformPosition(GetFurthestPlatformPosition(), 1);
            SpawnPlatform(nextPosition);
        }
    }

    Vector3 CalculateNextPlatformPosition(Vector3 lastPosition, int platformIndex)
    {
        float spacing = Random.Range(minPlatformSpacing, maxPlatformSpacing);
        float heightChange = Random.Range(minHeightChange, maxHeightChange);

        PlatformType selectedType = GetRandomPlatformType();
        float baseHeight = Mathf.Clamp(lastPosition.y + heightChange, selectedType.minHeight, selectedType.maxHeight);

        Vector3 nextPosition = Vector3.zero;

        switch (generationAxis)
        {
            case ScrollAxis.Horizontal:
                nextPosition = new Vector3(lastPosition.x + spacing, baseHeight, lastPosition.z);
                break;
            case ScrollAxis.Vertical:
                nextPosition = new Vector3(lastPosition.x + Random.Range(-2f, 2f), lastPosition.y + spacing, lastPosition.z);
                break;
        }

        return nextPosition;
    }

    void SpawnPlatform(Vector3 position)
    {
        PlatformType platformType = GetRandomPlatformType();
        if (platformType.prefab == null) return;

        GameObject platform = platformPool.GetObject(platformType.prefab);
        if (platform != null)
        {
            platform.transform.position = position;
            platform.SetActive(true);
            activePlatforms.Add(platform);

            lastSpawnPosition = GetAxisPosition(position);

            if (showGizmos)
                Debug.Log($"生成平台在位置: {position}");
        }
    }

    void CleanupPlatforms()
    {
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            if (activePlatforms[i] == null)
            {
                activePlatforms.RemoveAt(i);
                continue;
            }

            float platformPosition = GetAxisPosition(activePlatforms[i].transform.position);
            float cameraPosition = GetAxisPosition(mainCamera.transform.position);

            if (Mathf.Abs(platformPosition - cameraPosition) > despawnDistance)
            {
                platformPool.ReturnObject(activePlatforms[i]);
                activePlatforms.RemoveAt(i);
            }
        }
    }

    PlatformType GetRandomPlatformType()
    {
        if (platformTypes.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var type in platformTypes)
        {
            totalWeight += type.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var type in platformTypes)
        {
            currentWeight += type.weight;
            if (randomValue <= currentWeight)
            {
                return type;
            }
        }

        return platformTypes[0];
    }

    Vector3 GetFurthestPlatformPosition()
    {
        if (activePlatforms.Count == 0)
            return mainCamera.transform.position;

        Vector3 furthestPosition = activePlatforms[0].transform.position;
        foreach (var platform in activePlatforms)
        {
            if (GetAxisPosition(platform.transform.position) > GetAxisPosition(furthestPosition))
            {
                furthestPosition = platform.transform.position;
            }
        }
        return furthestPosition;
    }

    Vector3 GetStartPosition()
    {
        Vector3 cameraPos = mainCamera.transform.position;
        float startOffset = -spawnDistance * 0.5f;

        switch (generationAxis)
        {
            case ScrollAxis.Horizontal:
                return new Vector3(cameraPos.x + startOffset, cameraPos.y, cameraPos.z);
            case ScrollAxis.Vertical:
                return new Vector3(cameraPos.x, cameraPos.y + startOffset, cameraPos.z);
            default:
                return cameraPos;
        }
    }

    float GetAxisPosition(Vector3 position)
    {
        switch (generationAxis)
        {
            case ScrollAxis.Horizontal:
                return position.x;
            case ScrollAxis.Vertical:
                return position.y;
            default:
                return position.x;
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || mainCamera == null) return;

        Vector3 cameraPos = mainCamera.transform.position;

        // 绘制生成区域
        Gizmos.color = spawnZoneColor;
        switch (generationAxis)
        {
            case ScrollAxis.Horizontal:
                Gizmos.DrawWireCube(
                    new Vector3(cameraPos.x + spawnDistance * 0.5f, cameraPos.y, cameraPos.z),
                    new Vector3(spawnDistance, 10f, 1f)
                );
                break;
            case ScrollAxis.Vertical:
                Gizmos.DrawWireCube(
                    new Vector3(cameraPos.x, cameraPos.y + spawnDistance * 0.5f, cameraPos.z),
                    new Vector3(10f, spawnDistance, 1f)
                );
                break;
        }

        // 绘制回收区域
        Gizmos.color = despawnZoneColor;
        switch (generationAxis)
        {
            case ScrollAxis.Horizontal:
                Gizmos.DrawWireCube(
                    new Vector3(cameraPos.x - despawnDistance * 0.5f, cameraPos.y, cameraPos.z),
                    new Vector3(despawnDistance, 10f, 1f)
                );
                break;
            case ScrollAxis.Vertical:
                Gizmos.DrawWireCube(
                    new Vector3(cameraPos.x, cameraPos.y - despawnDistance * 0.5f, cameraPos.z),
                    new Vector3(10f, despawnDistance, 1f)
                );
                break;
        }
    }
}

// 简化的对象池实现
[System.Serializable]
public class SimpleObjectPool
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public List<GameObject> inactiveObjects = new List<GameObject>();
    }

    public List<Pool> pools = new List<Pool>();

    public void AddPrefab(GameObject prefab, int initialSize = 5)
    {
        // 检查是否已存在该预制体的池
        foreach (var pool in pools)
        {
            if (pool.prefab == prefab) return;
        }

        Pool newPool = new Pool { prefab = prefab };
        pools.Add(newPool);

        // 预创建对象
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateNewObject(prefab);
            obj.SetActive(false);
            newPool.inactiveObjects.Add(obj);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        // 查找对应的对象池
        Pool targetPool = null;
        foreach (var pool in pools)
        {
            if (pool.prefab == prefab)
            {
                targetPool = pool;
                break;
            }
        }

        // 如果没有找到池，创建一个
        if (targetPool == null)
        {
            AddPrefab(prefab);
            targetPool = pools[pools.Count - 1];
        }

        // 从池中获取对象
        if (targetPool.inactiveObjects.Count > 0)
        {
            GameObject obj = targetPool.inactiveObjects[0];
            targetPool.inactiveObjects.RemoveAt(0);
            return obj;
        }
        else
        {
            // 池为空，创建新对象
            return CreateNewObject(prefab);
        }
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);

        // 找到对应的对象池
        foreach (var pool in pools)
        {
            string cleanPrefabName = pool.prefab.name;
            string cleanObjName = obj.name.Replace("(Clone)", "").Trim();

            if (cleanObjName == cleanPrefabName)
            {
                pool.inactiveObjects.Add(obj);
                return;
            }
        }

        // 如果没有找到对应的池，销毁对象
        GameObject.Destroy(obj);
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = GameObject.Instantiate(prefab);
        obj.name = prefab.name; // 保持名称简洁
        return obj;
    }
}