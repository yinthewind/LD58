using System.Collections.Generic;
using UnityEngine;

public class PlayerAttract : MonoBehaviour
{
    [Header("吸引设置")]
    public float attractRadius = 3f;      // 吸引范围半径
    public float attractForce = 5f;       // 吸引力大小
    public string foodTag = "Food";       // 食物标签

    [Header("收集进度条设置")]
    public bool useCollectionProgress = false;  // 是否使用收集进度条
    public float maxCollectionValue = 100f;     // 最大收集值
    public float currentCollectionValue = 0f;   // 当前收集值
    public float foodValue = 10f;               // 每个食物的价值

    [Header("FUNGUS集成")]
    public Fungus.Flowchart flowchart;          // Fungus流程图
    public string triggerBlockName = "OnCollectionComplete"; // 触发块名称

    private List<Transform> attractedFood = new List<Transform>();
    private Collider2D[] results = new Collider2D[10];
    private CollectionProgressBar progressBar;  // 进度条控制器

    void Start()
    {
        // 查找进度条控制器
        if (useCollectionProgress)
        {
            progressBar = FindObjectOfType<CollectionProgressBar>();
            if (progressBar == null)
            {
                Debug.LogWarning("未找到CollectionProgressBar组件，请确保场景中有进度条UI");
            }
            else
            {
                progressBar.SetMaxValue(maxCollectionValue);
                progressBar.UpdateProgress(currentCollectionValue);
            }
        }
    }

    void Update()
    {
        FindFoodInRadius();
        AttractFood();
    }

    void FindFoodInRadius()
    {
        // 使用物理检测范围内的食物
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, attractRadius, results);
        
        for (int i = 0; i < count; i++)
        {
            if (results[i].CompareTag(foodTag))
            {
                Transform food = results[i].transform;
                if (!attractedFood.Contains(food))
                {
                    attractedFood.Add(food);
                }
            }
        }

        // 移除超出范围或已销毁的食物
        for (int i = attractedFood.Count - 1; i >= 0; i--)
        {
            if (attractedFood[i] == null || 
                Vector2.Distance(transform.position, attractedFood[i].position) > attractRadius * 1.2f)
            {
                attractedFood.RemoveAt(i);
            }
        }
    }

    void AttractFood()
    {
        foreach (Transform food in attractedFood)
        {
            if (food == null) continue;

            Vector2 direction = (transform.position - food.position).normalized;
            float distance = Vector2.Distance(transform.position, food.position);
            
            // 距离越近吸引力越小（避免抖动）
            float forceMultiplier = Mathf.Clamp01(distance / attractRadius);
            Vector2 force = direction * attractForce * forceMultiplier * Time.deltaTime;

            food.Translate(force);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞对象是否是食物
        if (collision.CompareTag(foodTag))
        {
            // 处理收集逻辑
            HandleCollection();
            
            Destroy(collision.gameObject);
            Debug.Log("获得食物!");
        }
    }

    void HandleCollection()
    {
        if (!useCollectionProgress) return;

        // 增加收集值
        currentCollectionValue += foodValue;

        // 限制不超过最大值
        currentCollectionValue = Mathf.Min(currentCollectionValue, maxCollectionValue);

        // 更新进度条
        if (progressBar != null)
        {
            progressBar.UpdateProgress(currentCollectionValue);
        }

        // 检查是否完成收集
        if (currentCollectionValue >= maxCollectionValue)
        {
            OnCollectionComplete();
        }
    }

    void OnCollectionComplete()
    {
        Debug.Log("收集完成！");

        // 触发FUNGUS流程图
        if (flowchart != null && !string.IsNullOrEmpty(triggerBlockName))
        {
            flowchart.ExecuteBlock(triggerBlockName);
        }
        else
        {
            Debug.LogWarning("未设置FUNGUS流程图或触发块名称");
        }
    }

    // 在Scene视图中显示吸引范围（仅在选中时）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractRadius);
    }

    // 公共方法，用于外部控制收集值
    public void AddCollectionValue(float value)
    {
        if (!useCollectionProgress) return;

        currentCollectionValue += value;
        currentCollectionValue = Mathf.Min(currentCollectionValue, maxCollectionValue);

        if (progressBar != null)
        {
            progressBar.UpdateProgress(currentCollectionValue);
        }

        if (currentCollectionValue >= maxCollectionValue)
        {
            OnCollectionComplete();
        }
    }

    // 重置收集进度
    public void ResetCollection()
    {
        currentCollectionValue = 0f;
        if (progressBar != null)
        {
            progressBar.UpdateProgress(currentCollectionValue);
        }
    }
}