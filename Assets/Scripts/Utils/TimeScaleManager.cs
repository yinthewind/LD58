using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages game pause by freezing physics while keeping UI and Fungus interactive.
/// Pauses Rigidbody2D physics instead of using Time.timeScale to avoid breaking UI systems.
/// </summary>
public class TimeScaleManager : MonoBehaviour
{
    private static TimeScaleManager _instance;
    public static TimeScaleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("TimeScaleManager");
                _instance = go.AddComponent<TimeScaleManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private bool _isPaused = false;
    private readonly List<Rigidbody2D> _rigidbodies = new();

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Stops game physics. Player and rigidbodies will freeze, but UI and Fungus remain interactive.
    /// </summary>
    public void StopTime()
    {
        if (_isPaused) return;

        _rigidbodies.Clear();

        // Find all Rigidbody2D components and disable simulation
        Rigidbody2D[] rigidbodies = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
        foreach (Rigidbody2D rb in rigidbodies)
        {
            _rigidbodies.Add(rb);
            rb.simulated = false;
        }

        _isPaused = true;
    }

    /// <summary>
    /// Resumes game physics.
    /// </summary>
    public void ResumeTime()
    {
        if (!_isPaused) return;

        // Re-enable simulation for all rigidbodies
        foreach (Rigidbody2D rb in _rigidbodies)
        {
            if (rb != null)
            {
                rb.simulated = true;
            }
        }

        _rigidbodies.Clear();
        _isPaused = false;
    }

    /// <summary>
    /// Toggles between paused and resumed state.
    /// </summary>
    public void ToggleTime()
    {
        if (_isPaused)
            ResumeTime();
        else
            StopTime();
    }
}
