using UnityEngine;
using System;

public class CollectionTween : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float arcHeight = 2f;

    private Vector3 startPosition;
    private Func<Vector3> getTargetPosition;
    private float elapsedTime = 0f;
    private bool isInitialized = false;

    public bool IsComplete { get; private set; } = false;

    public void Initialize(Func<Vector3> targetGetter)
    {
        startPosition = transform.position;
        getTargetPosition = targetGetter;
        isInitialized = true;
        elapsedTime = 0f;
        IsComplete = false;
    }

    private void Update()
    {
        if (!isInitialized || IsComplete)
            return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        // Apply curve
        float curveValue = curve.Evaluate(t);

        // Get fresh target position every frame (handles moving targets)
        Vector3 currentTarget = getTargetPosition();

        // Lerp position with arc
        Vector3 basePosition = Vector3.Lerp(startPosition, currentTarget, curveValue);

        // Add arc (parabola) - only during the middle of the animation
        // At t=0 and t=1, arc=0, ensuring we start and end at exact positions
        float arc = arcHeight * Mathf.Sin(t * Mathf.PI);
        basePosition.y += arc;

        transform.position = basePosition;

        // Optional: Scale down as it approaches (doesn't affect position since sprite is centered)
        float scale = Mathf.Lerp(1f, 0.5f, curveValue);
        transform.localScale = Vector3.one * scale;

        // Mark complete when done
        if (t >= 1f)
        {
            // Ensure we end exactly at current target position
            transform.position = currentTarget;
            IsComplete = true;
        }
    }
}
