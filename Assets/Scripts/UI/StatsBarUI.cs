using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays health and money stats as text in screen space.
/// Automatically created by StatsManager at runtime.
/// </summary>
public class StatsBarUI : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private int fontSize = 42;
    [SerializeField] private float textSpacing = 40f;
    [SerializeField] private Vector2 screenPosition = new Vector2(60f, -150f); // Offset from top-left

    [Header("Colors")]
    [SerializeField] private Color healthTextColor = Color.red;
    [SerializeField] private Color moneyTextColor = Color.yellow;

    // UI Components
    private Canvas canvas;
    private Text healthText;
    private Text moneyText;

    private void Awake()
    {
        CreateUI();
    }

    private void Start()
    {
        // Subscribe to stat changes
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.OnHealthChanged += OnHealthChanged;
            StatsManager.Instance.OnMoneyChanged += OnMoneyChanged;

            // Initialize with current values
            UpdateHealthText(StatsManager.Instance.Health, StatsManager.Instance.MaxHealth);
            UpdateMoneyText(StatsManager.Instance.Money);
        }
    }

    private void OnDestroy()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.OnHealthChanged -= OnHealthChanged;
            StatsManager.Instance.OnMoneyChanged -= OnMoneyChanged;
        }
    }

    private void CreateUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("StatsUI_Canvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Ensure it's on top

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create container for text
        GameObject container = new GameObject("StatsContainer");
        container.transform.SetParent(canvasObj.transform);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1); // Top-left
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0, 1);
        containerRect.anchoredPosition = screenPosition;
        containerRect.sizeDelta = new Vector2(400f, 200f);

        // Create Health Text
        healthText = CreateTextLabel(container.transform, "HealthText", 0, healthTextColor);

        // Create Money Text
        moneyText = CreateTextLabel(container.transform, "MoneyText", -textSpacing, moneyTextColor);

        DontDestroyOnLoad(canvasObj);
    }

    private Text CreateTextLabel(Transform parent, string name, float yOffset, Color textColor)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(0, 1);
        textRect.pivot = new Vector2(0, 1);
        textRect.anchoredPosition = new Vector2(0, yOffset);
        textRect.sizeDelta = new Vector2(400f, fontSize + 10);

        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = textColor;
        text.text = "";

        // Add outline for better readability
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        return text;
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        UpdateHealthText(newHealth, StatsManager.Instance.MaxHealth);
    }

    private void OnMoneyChanged(int oldMoney, int newMoney)
    {
        UpdateMoneyText(newMoney);
    }

    private void UpdateHealthText(int health, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {health} / {maxHealth}";
        }
    }

    private void UpdateMoneyText(int money)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: ${money}";
        }
    }

    /// <summary>
    /// Creates the StatsBarUI if it doesn't already exist.
    /// Called by StatsManager during initialization.
    /// </summary>
    public static StatsBarUI CreateInstance()
    {
        // Check if UI already exists
        StatsBarUI existing = FindAnyObjectByType<StatsBarUI>();
        if (existing != null)
        {
            return existing;
        }

        // Create new UI GameObject
        GameObject uiObj = new GameObject("StatsBarUI");
        StatsBarUI ui = uiObj.AddComponent<StatsBarUI>();
        DontDestroyOnLoad(uiObj);

        return ui;
    }
}
