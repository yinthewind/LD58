using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    [Header("泡泡设置")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private int bubbleCount = 10;
    [SerializeField] private float spawnAreaWidth = 10f;
    [SerializeField] private float spawnAreaHeight = 5f;
    
    [Header("漂浮参数")]
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float minFloatHeight = 1f;
    [SerializeField] private float maxFloatHeight = 3f;
    [SerializeField] private float minFloatAngle = -30f;
    [SerializeField] private float maxFloatAngle = 30f;
    
    [Header("大小设置")]
    [SerializeField] private float minBubbleSize = 0.5f;
    [SerializeField] private float maxBubbleSize = 1.5f;
    
    [Header("发散范围")]
    [SerializeField] private float spreadRange = 2f;
    
    private List<Bubble> bubbles = new List<Bubble>();
    private List<GameObject> bubbleObjects = new List<GameObject>();

    void Start()
    {
        GenerateBubbles();
    }

    void GenerateBubbles()
    {
        // 清除现有的泡泡
        foreach (var bubbleObj in bubbleObjects)
        {
            if (bubbleObj != null)
                Destroy(bubbleObj);
        }
        bubbles.Clear();
        bubbleObjects.Clear();

        for (int i = 0; i < bubbleCount; i++)
        {
            CreateBubble();
        }
    }

    void CreateBubble()
    {
        if (bubblePrefab == null)
        {
            Debug.LogWarning("泡泡预制体未分配！");
            return;
        }

        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
            Random.Range(-spawnAreaHeight / 2, spawnAreaHeight / 2),
            0
        );

        GameObject bubbleObj = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity, transform);
        bubbleObjects.Add(bubbleObj);

        Bubble bubble = new Bubble();
        
        // 设置泡泡属性
        bubble.gameObject = bubbleObj;
        bubble.startPosition = spawnPosition;
        bubble.speed = Random.Range(minSpeed, maxSpeed);
        bubble.floatHeight = Random.Range(minFloatHeight, maxFloatHeight);
        bubble.floatAngle = Random.Range(minFloatAngle, maxFloatAngle) * Mathf.Deg2Rad;
        bubble.currentTime = Random.Range(0f, Mathf.PI * 2); // 随机起始相位
        bubble.spreadOffset = Random.insideUnitCircle * spreadRange;
        
        // 设置泡泡大小
        float bubbleSize = Random.Range(minBubbleSize, maxBubbleSize);
        bubbleObj.transform.localScale = Vector3.one * bubbleSize;

        bubbles.Add(bubble);
    }

    void Update()
    {
        UpdateBubbles();
    }

    void UpdateBubbles()
    {
        foreach (var bubble in bubbles)
        {
            if (bubble.gameObject == null) continue;

            // 更新泡泡位置 - 使用正弦函数创建漂浮效果
            bubble.currentTime += Time.deltaTime * bubble.speed;
            
            float verticalOffset = Mathf.Sin(bubble.currentTime) * bubble.floatHeight;
            float horizontalOffset = Mathf.Cos(bubble.currentTime) * Mathf.Tan(bubble.floatAngle) * bubble.floatHeight;
            
            Vector3 newPosition = bubble.startPosition + 
                                new Vector3(horizontalOffset, verticalOffset, 0) + 
                                (Vector3)bubble.spreadOffset;
            
            bubble.gameObject.transform.position = newPosition;
        }
    }

    [System.Serializable]
    public class Bubble
    {
        public GameObject gameObject;
        public Vector3 startPosition;
        public float speed;
        public float floatHeight;
        public float floatAngle; // 弧度
        public float currentTime;
        public Vector2 spreadOffset;
    }

    // 在Inspector中修改参数时重新生成泡泡
    void OnValidate()
    {
        if (Application.isPlaying && bubbleObjects.Count > 0)
        {
            GenerateBubbles();
        }
    }

    // 在Scene视图中显示生成区域
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaWidth, spawnAreaHeight, 0));
        
        // 显示发散范围
        Gizmos.color = Color.yellow;
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
                Random.Range(-spawnAreaHeight / 2, spawnAreaHeight / 2),
                0
            ) + transform.position;
            Gizmos.DrawWireSphere(pos, 0.2f);
        }
    }
}