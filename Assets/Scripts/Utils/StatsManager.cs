using System;
using UnityEngine;

/// <summary>
/// Manages player stats (health, money) that persist across scene transitions.
/// Singleton pattern ensures single instance accessible via StatsManager.Instance.
/// </summary>
public class StatsManager : MonoBehaviour
{
    private static StatsManager _instance;
    private static StatsBarUI _statsUI;

    public static StatsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing StatsManager in scene first
                _instance = FindAnyObjectByType<StatsManager>();

                // If none exists, create one dynamically
                if (_instance == null)
                {
                    GameObject go = new GameObject("StatsManager");
                    _instance = go.AddComponent<StatsManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Money Settings")]
    [SerializeField] private int currentMoney = 0;

    // Events for stat changes
    public event Action<int, int> OnHealthChanged; // oldValue, newValue
    public event Action<int, int> OnMoneyChanged;  // oldValue, newValue
    public event Action OnDeath;

    // Public properties
    public int MaxHealth => maxHealth;
    public int Health
    {
        get => currentHealth;
        set
        {
            int oldHealth = currentHealth;
            currentHealth = Mathf.Clamp(value, 0, maxHealth);

            if (oldHealth != currentHealth)
            {
                OnHealthChanged?.Invoke(oldHealth, currentHealth);

                if (currentHealth <= 0 && oldHealth > 0)
                {
                    OnDeath?.Invoke();
                }
            }
        }
    }

    public int Money
    {
        get => currentMoney;
        set
        {
            int oldMoney = currentMoney;
            currentMoney = Mathf.Max(0, value); // Money can't be negative

            if (oldMoney != currentMoney)
            {
                OnMoneyChanged?.Invoke(oldMoney, currentMoney);
            }
        }
    }

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Create UI if it doesn't exist
        if (_statsUI == null)
        {
            _statsUI = StatsBarUI.CreateInstance();
        }
    }

    /// <summary>
    /// Modifies health by the specified amount. Positive values heal, negative values damage.
    /// </summary>
    public void ModifyHealth(int amount)
    {
        Health += amount;
    }

    /// <summary>
    /// Modifies money by the specified amount. Positive values add money, negative values subtract.
    /// </summary>
    public void ModifyMoney(int amount)
    {
        Money += amount;
    }

    /// <summary>
    /// Sets the maximum health value and clamps current health if necessary.
    /// </summary>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = Mathf.Max(1, newMaxHealth);
        Health = currentHealth; // Triggers clamp if current > new max
    }

    /// <summary>
    /// Resets all stats to their default values.
    /// </summary>
    public void ResetStats()
    {
        Health = maxHealth;
        Money = 0;
    }

    /// <summary>
    /// Fully heals the player to max health.
    /// </summary>
    public void FullHeal()
    {
        Health = maxHealth;
    }
}
