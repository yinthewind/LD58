using UnityEngine;
using System.Collections.Generic;

public class BackgroundScroller : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundLayer
    {
        public Transform transform;
        public float scrollSpeed = 1f;
        public Vector2 direction = Vector2.right;
    }

    [Header("滚动设置")]
    public ScrollAxis scrollAxis = ScrollAxis.Horizontal;
    public List<BackgroundLayer> backgroundLayers = new List<BackgroundLayer>();

    [Header("调试")]
    public bool debugMode = false;

    private Camera mainCamera;
    private Vector2 cameraPreviousPosition;
    private Dictionary<Transform, float> backgroundWidths = new Dictionary<Transform, float>();

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraPreviousPosition = mainCamera.transform.position;
        }

        // 预计算每个背景的宽度
        foreach (var layer in backgroundLayers)
        {
            if (layer.transform != null)
            {
                SpriteRenderer spriteRenderer = layer.transform.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    float width = spriteRenderer.bounds.size.x;
                    backgroundWidths[layer.transform] = width;
                }
            }
        }
    }

    void Update()
    {
        if (mainCamera == null) return;

        Vector2 cameraDelta = (Vector2)mainCamera.transform.position - cameraPreviousPosition;
        
        foreach (var layer in backgroundLayers)
        {
            if (layer.transform != null)
            {
                ScrollBackground(layer, cameraDelta);
            }
        }

        cameraPreviousPosition = mainCamera.transform.position;
    }

    void ScrollBackground(BackgroundLayer layer, Vector2 cameraDelta)
    {
        Vector2 effectiveDirection = GetEffectiveDirection(layer.direction);
        Vector2 scrollMovement = Vector2.zero;

        // 根据选择的轴应用移动
        switch (scrollAxis)
        {
            case ScrollAxis.Horizontal:
                scrollMovement.x = cameraDelta.x * effectiveDirection.x * layer.scrollSpeed;
                break;
            case ScrollAxis.Vertical:
                scrollMovement.y = cameraDelta.y * effectiveDirection.y * layer.scrollSpeed;
                break;
            case ScrollAxis.Both:
                scrollMovement = new Vector2(
                    cameraDelta.x * effectiveDirection.x * layer.scrollSpeed,
                    cameraDelta.y * effectiveDirection.y * layer.scrollSpeed
                );
                break;
        }

        // 应用滚动
        layer.transform.Translate(scrollMovement, Space.World);

        // 检查是否需要重置位置
        CheckAndResetPosition(layer);
    }

    void CheckAndResetPosition(BackgroundLayer layer)
    {
        if (mainCamera == null) return;

        if (backgroundWidths.ContainsKey(layer.transform))
        {
            float backgroundWidth = backgroundWidths[layer.transform];
            Vector3 cameraPos = mainCamera.transform.position;
            Vector3 bgPos = layer.transform.position;

            float cameraDistance = 0f;
            float resetThreshold = 0f;

            switch (scrollAxis)
            {
                case ScrollAxis.Horizontal:
                    cameraDistance = bgPos.x - cameraPos.x;
                    resetThreshold = backgroundWidth * 1.5f;
                    if (Mathf.Abs(cameraDistance) > resetThreshold)
                    {
                        float resetDirection = Mathf.Sign(cameraDistance) * -1f;
                        layer.transform.Translate(Vector3.right * resetDirection * backgroundWidth * 2f, Space.World);
                        
                        if (debugMode)
                            Debug.Log($"重置背景位置: {layer.transform.name}");
                    }
                    break;

                case ScrollAxis.Vertical:
                    cameraDistance = bgPos.y - cameraPos.y;
                    resetThreshold = backgroundWidth * 1.5f;
                    if (Mathf.Abs(cameraDistance) > resetThreshold)
                    {
                        float resetDirection = Mathf.Sign(cameraDistance) * -1f;
                        layer.transform.Translate(Vector3.up * resetDirection * backgroundWidth * 2f, Space.World);
                    }
                    break;
            }
        }
    }

    Vector2 GetEffectiveDirection(Vector2 originalDirection)
    {
        switch (scrollAxis)
        {
            case ScrollAxis.Horizontal:
                return new Vector2(originalDirection.x, 0);
            case ScrollAxis.Vertical:
                return new Vector2(0, originalDirection.y);
            default:
                return originalDirection;
        }
    }

    // 编辑器方法：添加新的背景层
    [ContextMenu("添加新背景层")]
    public void AddNewBackgroundLayer()
    {
        GameObject newBg = new GameObject($"BackgroundLayer_{backgroundLayers.Count}");
        newBg.transform.SetParent(transform);
        newBg.AddComponent<SpriteRenderer>();

        BackgroundLayer newLayer = new BackgroundLayer
        {
            transform = newBg.transform,
            scrollSpeed = 0.5f,
            direction = Vector2.right
        };

        backgroundLayers.Add(newLayer);
    }
}