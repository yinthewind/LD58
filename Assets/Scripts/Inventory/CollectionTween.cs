using UnityEngine;

public class CollectionTween : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float arcHeight = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float elapsedTime = 0f;
    private bool isInitialized = false;

    public bool IsComplete { get; private set; } = false;

    public void Initialize(Vector3 target)
    {
        startPosition = transform.position;
        targetPosition = target;
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

        // Lerp position with arc
        Vector3 basePosition = Vector3.Lerp(startPosition, targetPosition, curveValue);

        // Add arc (parabola)
        float arc = arcHeight * Mathf.Sin(t * Mathf.PI);
        basePosition.y += arc;

        transform.position = basePosition;

        // Optional: Scale down as it approaches
        float scale = Mathf.Lerp(1f, 0.5f, curveValue);
        transform.localScale = Vector3.one * scale;

        // Mark complete when done
        if (t >= 1f)
        {
            IsComplete = true;
        }
    }
}
